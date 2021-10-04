using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetter : MonoBehaviour
{
    static LevelSetter _instance;
    public static LevelSetter Instance { get { return _instance; } }

    [SerializeField] public List<Unit> unitsTotal;
    [SerializeField] public List<Unit> unitsUnlocked;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public void UnlockUnit(int unitIndex)
    {
        unitsUnlocked.Add(unitsTotal[unitIndex]);

        PlayerPrefs.SetInt("unlocked_units_count", unitsUnlocked.Count);

        //fancy buy effect
    }
}


