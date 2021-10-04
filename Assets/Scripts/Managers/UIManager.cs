using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] GameObject menuScreen;

    [SerializeField] Transform unlockedParent;

    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI unitText;

    UnitsManager unitsMan;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        unitsMan = GetComponent<UnitsManager>();

        PopulateUnlockedSlots(LevelSetter.Instance.unitsUnlocked);
    }

    private void Update()
    {
        moneyText.text = GameManager.Instance.money.ToString();
        unitText.text = UnitsManager.Agent.Count.ToString();
    }

    void PopulateUnlockedSlots (List<Unit> unlockedUnits)
    {
        for ( int i = 0; i < unlockedUnits.Count; i ++)
        {
            unlockedParent.GetChild(i).gameObject.SetActive(true);
            unlockedParent.GetChild(i).GetComponent<Image>().sprite = unlockedUnits[i].icon;
            unlockedParent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = unlockedUnits[i].cost.ToString();
        }
    }

    public void DisplayWinScreen ()
    {
        winScreen.SetActive(true);
    }

    public void DisplayLoseScreen ()
    {
        loseScreen.SetActive(true);
    }

    #region UI Buttons
    public void StartLevel ()
    {
        GameManager.Instance.gamePhase++;
    }

    public void StartCombat()
    {
        unitsMan.doJob = true;
        DrawingManager.Instance.canDraw = false;

        LeanTween.delayedCall(2, () => GameManager.Instance.gamePhase++);
    }

    public void NextLevel ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RetryLevel ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu ()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseGame ()
    {
        Time.timeScale = 0;
        menuScreen.SetActive(true);
    }

    public void ResumeGame ()
    {
        Time.timeScale = 1;
        menuScreen.SetActive(false);
    }
    #endregion
}
