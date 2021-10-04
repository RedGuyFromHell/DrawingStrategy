using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    [SerializeField] public int money = 200;

    [HideInInspector] public int DamageModifier = 0;
    [HideInInspector] public int HealthModifier = 0;
    [HideInInspector] public int ArmorModifier = 0;
    //[HideInInspector] public int ResistanceModifier = 0;
    [HideInInspector] public int AttackSpeedModifier = 0;

    [SerializeField] public int gamePhase = 0;
    [SerializeField] public Transform Canvas;
    [SerializeField] public LevelSetter lvlSetter;

    public bool levelHasFinished = false;
    public bool playerHasWon = false;

    UnitsManager unitsMan;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        Canvas = GameObject.Find("Canvas").transform;
        unitsMan = GetComponent<UnitsManager>();

        LoadGameInfo();
    }

    void LoadGameInfo ()
    {
        DamageModifier = PlayerPrefs.GetInt("dmg_modifier");
        HealthModifier = PlayerPrefs.GetInt("hp_modifier");
        ArmorModifier = PlayerPrefs.GetInt("armor_modifier");
        if (PlayerPrefs.GetInt("money") != 0)
            money = PlayerPrefs.GetInt("money");
        else
            PlayerPrefs.SetInt("money", money);
    }

    private void Start()
    {
        PlayerPrefs.SetInt("unlocked_units_count", lvlSetter.unitsUnlocked.Count);
    }

    private void Update()
    {
        if (gamePhase == 1)
        {
            if (unitsMan.allyCount == 0)
            {
                playerHasWon = false;
                levelHasFinished = true;
                StartLoseSequence();

                //play sad noises
                gamePhase++;
            }
            if (unitsMan.enemyCount == 0)
            {
                playerHasWon = true;
                levelHasFinished = true;
                StartWinSequence();

                //play happy noises
                gamePhase++;
            }
        }
    }

    void StartWinSequence ()
    {
        LeanTween.delayedCall(1f, () => UIManager.Instance.DisplayWinScreen()).setOnComplete(
            () => LeanTween.delayedCall(3f, () => SceneManager.LoadScene(1)));

        Debug.Log("You won");
        EndLevelSequence();
        PlayerPrefs.SetInt("levels_completed", SceneManager.GetActiveScene().buildIndex - 1);
        PlayerPrefs.SetInt("level_won", 1);


        //reward player with prize
    }

    void StartLoseSequence ()
    {
        LeanTween.delayedCall(2f, () => UIManager.Instance.DisplayLoseScreen()).setOnComplete(
            () => LeanTween.delayedCall(3f, () => SceneManager.LoadScene(1)));

        EndLevelSequence();

        Debug.Log("You Lost");
        PlayerPrefs.SetInt("levels_completed", SceneManager.GetActiveScene().buildIndex - 2);
        PlayerPrefs.SetInt("level_won", 0);
    }

    void EndLevelSequence ()
    {
        LeanTween.delayedCall(.5f, () => UnitsManager.Agent.Clear());

        foreach (Unit unit in UnitsManager.Agent)
        {
            unit.StopAllCoroutines();
        }
    }
}
