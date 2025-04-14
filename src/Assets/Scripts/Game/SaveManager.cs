using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public bool hasSave = false;
    public ProgressionData progression = new ProgressionData();
    public AbilityData abilities = new AbilityData();
}

[System.Serializable]
public class ProgressionData
{
    public string currentLevel = "Level_1";
    public LevelClaimData levelsClaimed = new LevelClaimData();
}

[System.Serializable]
public class LevelClaimData
{
    public bool Level_1 = false;
    public bool Level_2 = false;
    public bool Level_3 = false;
    public bool Level_4 = false;
    public bool Level_5 = false;
}

[System.Serializable]
public class AbilityData
{
    public bool hasSword = false;
    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasGrapple = false;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath;
    private event System.Action abilitiesUpdated;
    private event System.Action<string> levelClaimed;


    public SaveData CurrentSave { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save.json";
            LoadGame();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CurrentSave = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentSave = new SaveData();
        }
    }

    public void SaveGame()
    {
        CurrentSave.hasSave = true;
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(savePath, json);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);

        CurrentSave = new SaveData();
    }

    public void ResetSave()
    {
        CurrentSave = new SaveData();
    }

    public void SetCurrentLevel(string levelName)
    {
        CurrentSave.progression.currentLevel = levelName;
        SaveGame();
    }


    public void AddAbilitiesUpdatedListener(System.Action callback) => abilitiesUpdated += callback;
    public void RemoveAbilitiesUpdatedListener(System.Action callback) => abilitiesUpdated -= callback;
    public void TriggerAbilitiesUpdated() => abilitiesUpdated?.Invoke();

    public void AddLevelClaimedListener(System.Action<string> callback) => levelClaimed += callback;
    public void RemoveLevelClaimedListener(System.Action<string> callback) => levelClaimed -= callback;
    public void TriggerLevelClaimed(string levelId) => levelClaimed?.Invoke(levelId);


}
