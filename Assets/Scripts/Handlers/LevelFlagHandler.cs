using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlagHandler : MonoBehaviour
{
    public bool islocked = true;
    public bool isCompleted = false;

    [SerializeField] int levelIndex = 0;
    [SerializeField] public SkinnedMeshRenderer flagMaterial;
    [SerializeField] public Material unlockedFlagMaterial;
    [SerializeField] public Material lockedFlagMaterial;
    [SerializeField] public Material completedFlagMaterial;

    [SerializeField] public List<LevelFlagHandler> childrenLevels = new List<LevelFlagHandler>(10);

    private void Awake()
    {
        flagMaterial.material = lockedFlagMaterial;
    }

    public void StartLevel ()
    {
        SceneManager.LoadScene(2 + levelIndex);

        PlayerPrefs.SetInt("current_level", levelIndex);
    }

    public void CompleteLevel()
    {
        isCompleted = true;
        flagMaterial.material = completedFlagMaterial;

        UnlockChildrenLevels();

        //PlayerPrefs.SetInt()
        PlayerPrefs.SetInt("level_won", 0);
        PlayerPrefs.SetInt("arrow_level", PlayerPrefs.GetInt("levels_completed") + 1);
        PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") + 100);
        Debug.Log(PlayerPrefs.GetInt("arrow_level") + " ~ " + PlayerPrefs.GetInt("levels_completed"));

        UpdateCompletedLevelsSave();
    }

    void UnlockChildrenLevels ()
    {
        foreach (LevelFlagHandler level in childrenLevels)
            level.UnlockLevel();
    }

    public void UnlockLevel ()
    {
        this.islocked = false;
        flagMaterial.material = unlockedFlagMaterial;
    }

    void UpdateCompletedLevelsSave ()
    {
        string levelsCompletedAsString = PlayerPrefs.GetString("levelsCompletedAsString");

        if (!levelsCompletedAsString.Contains(levelIndex.ToString()))
            levelsCompletedAsString += levelIndex.ToString();
        

        PlayerPrefs.SetString("levelsCompletedAsString", levelsCompletedAsString);
    }

    public void ResetMaterial ()
    {
        flagMaterial.material = lockedFlagMaterial;
    }
}
