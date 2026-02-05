using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class FortressEdgeTargets : MonoBehaviour
{
    public int samplesPerPath = 400;
    public bool onlyVisibleInMainCamera = true;

    [Tooltip("Viewport margin (0.05 = ignore near screen edges).")]
    [Range(0f, 0.2f)]
    public float viewportMargin = 0.03f;
    public bool onlyAboveFortressCenter = true;

    PolygonCollider2D poly;
    Camera cam;
    List<Vector2> cachedPoints = new List<Vector2>();

    void Awake()
    {
        poly = GetComponent<PolygonCollider2D>();
        cam = Camera.main;
        RebuildCache();
    }

    public void RebuildCache()
    {
        cachedPoints.Clear();
        if (poly == null) return;

        int pathCount = poly.pathCount;
        for (int p = 0; p < pathCount; p++)
        {
            Vector2[] path = poly.GetPath(p);
            if (path == null || path.Length < 2) continue;

            // Convert local path points to world points
            Vector2[] world = new Vector2[path.Length];
            for (int i = 0; i < path.Length; i++)
                world[i] = poly.transform.TransformPoint(path[i]);

            // Compute total perimeter length
            float perimeter = 0f;
            for (int i = 0; i < world.Length; i++)
            {
                Vector2 a = world[i];
                Vector2 b = world[(i + 1) % world.Length];
                perimeter += Vector2.Distance(a, b);
            }

            if (perimeter <= 0.0001f) continue;

            // Sample evenly along perimeter
            for (int s = 0; s < samplesPerPath; s++)
            {
                float distAlong = (perimeter * s) / samplesPerPath;
                Vector2 pt = PointAtDistance(world, distAlong);

                if (PassesFilters(pt))
                    cachedPoints.Add(pt);
            }
        }

        // Safety: if filtering removed everything, fall back to unfiltered points
        if (cachedPoints.Count == 0)
        {
            Debug.LogWarning("FortressEdgeTargets: No points after filtering. Disabling filters fallback.");
            onlyVisibleInMainCamera = false;
            onlyAboveFortressCenter = false;

            // Rebuild without filters
            cachedPoints.Clear();
            int pathCount2 = poly.pathCount;
            for (int p = 0; p < pathCount2; p++)
            {
                Vector2[] path = poly.GetPath(p);
                Vector2[] world = new Vector2[path.Length];
                for (int i = 0; i < path.Length; i++)
                    world[i] = poly.transform.TransformPoint(path[i]);

                float perimeter = 0f;
                for (int i = 0; i < world.Length; i++)
                    perimeter += Vector2.Distance(world[i], world[(i + 1) % world.Length]);

                for (int s = 0; s < samplesPerPath; s++)
                {
                    float distAlong = (perimeter * s) / samplesPerPath;
                    cachedPoints.Add(PointAtDistance(world, distAlong));
                }
            }
        }

        Debug.Log($"FortressEdgeTargets cached points: {cachedPoints.Count}");
    }

    bool PassesFilters(Vector2 worldPoint)
    {
        if (onlyAboveFortressCenter)
        {
            if (worldPoint.y < transform.position.y) return false;
        }

        if (onlyVisibleInMainCamera && cam != null)
        {
            Vector3 v = cam.WorldToViewportPoint(worldPoint);
            float m = viewportMargin;

            // visible & in front of camera
            if (v.z < 0f) return false;
            if (v.x < m || v.x > 1f - m) return false;
            if (v.y < m || v.y > 1f - m) return false;
        }

        return true;
    }

    Vector2 PointAtDistance(Vector2[] loop, float dist)
    {
        for (int i = 0; i < loop.Length; i++)
        {
            Vector2 a = loop[i];
            Vector2 b = loop[(i + 1) % loop.Length];
            float seg = Vector2.Distance(a, b);
            if (seg <= 0.000001f) continue;

            if (dist <= seg)
            {
                float t = dist / seg;
                return Vector2.Lerp(a, b, t);
            }
            dist -= seg;
        }
        return loop[0];
    }

    public Vector2 GetRandomEdgePoint()
    {
        if (cachedPoints == null || cachedPoints.Count == 0)
        {
            // last resort
            return transform.position;
        }
        return cachedPoints[Random.Range(0, cachedPoints.Count)];
    }
}
