using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

[System.Serializable]
public class SaveData
{
    public List<string> identifiers = new List<string>();
    public List<string> values = new List<string>();
}

[CreateAssetMenu(menuName = "Toybox/SaveData/Manager", fileName = "SaveManager")]
public class SaveDataManager : ScriptableObject
{
    public List<GameValueBase> SaveDataValues = new List<GameValueBase>();

    private Dictionary<string, GameValueBase> gameValueMap = new Dictionary<string, GameValueBase>();

    // Start is called before the first frame update
    void Awake()
    {
        if (gameValueMap.Count == 0)
        {
            foreach (var value in SaveDataValues)
            {
                gameValueMap.Add(value.name, value);
            }
        }
    }

    public void Restore()
    {
        try
        {
            if (gameValueMap.Count == 0)
            {
                Awake();
            }
            var file = System.IO.File.ReadAllText(Application.persistentDataPath + "/gamesave.save");
            if (file != null)
            {
                SaveData sd = JsonConvert.DeserializeObject<SaveData>(file);
                int i = 0;
                foreach (var value in sd.identifiers)
                {
                    if (gameValueMap.ContainsKey(value))
                    {
                        gameValueMap[value].Restore(sd.values[i]);
                    }
                    i++;
                }
            }
        } catch (System.Exception e)
        {
            Debug.Log("No valid SaveGame found.");
        }
    }

    public void Save()
    {
        SaveData sd = new SaveData();
        foreach (var value in SaveDataValues)
        {
            var obj = value.Save();
            if (obj != null)
            {
                sd.identifiers.Add(value.name);
                sd.values.Add(JsonConvert.SerializeObject(obj));
            }
        }

        System.IO.FileStream file = System.IO.File.Create(Application.persistentDataPath + "/gamesave.save");
        System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
        var json = JsonConvert.SerializeObject(sd);
        writer.Write(json);
        writer.Flush();
        file.Close();
    }

    public bool Exists()
    {
        return System.IO.File.Exists(Application.persistentDataPath + "/gamesave.save");
    }

    public int ReadDay()
    {
        if (Exists())
        {
            try
            {
                if (gameValueMap.Count == 0)
                {
                    Awake();
                }
                var file = System.IO.File.ReadAllText(Application.persistentDataPath + "/gamesave.save");
                if (file != null)
                {
                    SaveData sd = JsonConvert.DeserializeObject<SaveData>(file);
                    int i = 0;
                    foreach (var value in sd.identifiers)
                    {
                        if (value == "SD_CurrentDay" && gameValueMap.ContainsKey(value))
                        {
                            return System.Convert.ToInt32(sd.values[i]);
                        }
                        i++;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("No valid SaveGame found.");

                return 0;
            }
        }

        return 0;
    }

    public void Clear()
    {
        if (Exists())
        {
            System.IO.File.Delete(Application.persistentDataPath + "/gamesave.save");
        }
    }
}
