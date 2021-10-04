using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Transform menuUnit;
    public static GameObject lastUnlockedUnit;

    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] List<Unit> totalUnits = new List<Unit>();
    [SerializeField] List<GameObject> totalUnitsPrefabs = new List<GameObject>();

    private void Awake()
    {
        Debug.Log(PlayerPrefs.GetInt("unlocked_units_count"));
        lastUnlockedUnit = GetPrefabFromUnit(totalUnits[PlayerPrefs.GetInt("unlocked_units_count") - 1]);
    }

    private void Start()
    {
        SetMenuUnit(lastUnlockedUnit);
    }

    private void Update()
    {
        menuUnit.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }

    void SetMenuUnit (GameObject unit)
    {
        Transform temp = Instantiate(lastUnlockedUnit, menuUnit).transform;
        temp.localScale = Vector3.one * 9;

        temp.Rotate(Vector3.up * 180);
    }

    GameObject GetPrefabFromUnit(Unit unit)
    {
        return totalUnitsPrefabs[PlayerPrefs.GetInt("unlocked_units_count") - 1];
    }

    #region Menu Buttons
    public void NewGame ()
    {
        SceneManager.LoadScene(2);
        PlayerPrefs.SetInt("levels_completed", 0);
        PlayerPrefs.SetInt("money", 200);
        PlayerPrefs.SetInt("arrow_level", 0);
    }

    public void CampaignMap ()
    {
        SceneManager.LoadScene(1);
    }

    public void Continue ()
    {
        //loads the newest scene from playerPrefs
    }
    #endregion
}
