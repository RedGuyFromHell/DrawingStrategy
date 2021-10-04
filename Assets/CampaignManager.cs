using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    static CampaignManager _instance;
    public static CampaignManager Instance { get { return _instance; } }

    public List<LevelFlagHandler> levels = new List<LevelFlagHandler>(100);

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        InitLevels();
    }

    void InitLevels ()
    {
        ResetAllFlagMaterials();

        CheckAndUnlockLevel();
    }

    void SetAppropriateFlags ()
    {
        CheckForFirstPlaySessions();

        string levelsCompleted = PlayerPrefs.GetString("levelsCompletedAsString");

        PlayerPrefs.SetInt("money", 200);
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            string currentLevelIndex = levelsCompleted[i].ToString();
            levels[int.Parse(currentLevelIndex)].CompleteLevel();
        }
    }

    void CheckForFirstPlaySessions ()
    {
        if (PlayerPrefs.GetString("levelsCompletedAsString") == null)
            PlayerPrefs.SetString("levelsCompletedAsString", "0");
        else if (PlayerPrefs.GetString("levelsCompletedAsString").Length == 0)
            PlayerPrefs.SetString("levelsCompletedAsString", "0");
    }

    void ResetAllFlagMaterials ()
    {
        foreach (LevelFlagHandler level in levels)
        {
            level.ResetMaterial();
        }
    }

    void CheckAndUnlockLevel ()
    {
        //for (int i = 0; i < PlayerPrefs.GetInt("levels_completed"); i++)
        //    levels[0].flagMaterial.material = levels[0].completedFlagMaterial;
        
        SetAppropriateFlags();

        if (PlayerPrefs.GetInt("levels_completed") != 0 && PlayerPrefs.GetInt("level_won") != 0)
            levels[PlayerPrefs.GetInt("levels_completed") - 1].CompleteLevel();   //grants reward too
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLevelIfUnlocked(Input.mousePosition);
        }
    }

    void StartLevelIfUnlocked (Vector3 mousePos)
    {
        LayerMask flagMask = LayerMask.GetMask("Flags");
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, flagMask))
            if (!hit.transform.GetComponent<LevelFlagHandler>().islocked && !hit.transform.GetComponent<LevelFlagHandler>().isCompleted)
                hit.transform.GetComponent<LevelFlagHandler>().StartLevel();
    }
}
