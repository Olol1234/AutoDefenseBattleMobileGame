using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public PlayerData Data;
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
            Debug.Log("Save path: " + savePath);

            Load();
        }
        else Destroy(gameObject);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Saved to: " + savePath);
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Loaded save file");
        }
        else
        {
            Debug.Log("No save found, creating new");
            Data = new PlayerData();
            Save();
        }
    }

    // Optional helper
    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);

        Data = new PlayerData();
        Save();
    }
}
