using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileUpgradeAnim : MonoBehaviour
{
    public static HomingMissileUpgradeAnim Instance;

    void Awake()
    {
        Instance = this;
    }
    
    public void PlayUpgradeAnimation()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Animator anim = GetComponent<Animator>();

        if (sr != null && anim != null)
        {
            // 1. Make the sprite visible
            sr.enabled = true; 
            
            anim.SetTrigger("PlayUpgrade");             
            StartCoroutine(HideAfterDelay(sr, 0.5f)); 
        }
    }

    private IEnumerator HideAfterDelay(SpriteRenderer sr, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        sr.enabled = false;
    }
}