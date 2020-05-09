using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Threading;

public class GameManager : MonoBehaviour
{
    //Allows creation of certain events for our scripts to be notified of
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    //Static accessibility reference from other scripts
    public static GameManager Instance; 

    //References to UI pages
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public GameObject settingsPage;

    //Score text to keep track of
    public Text scoreText;
    private int score = 0;
    public GameObject gameScoreString;

    //Game status to keep track of
    private bool gameOver = true;
    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; } }

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown,
        Settings
    }

    //**********************************************//
    //Monetization

    string gameID = "3594306";
    public string placementID = "InGame";
    public bool testMode = true;

    private void Start()
    {
        Advertisement.Initialize(gameID, testMode);
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady (placementID))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(placementID);
    }

    //**********************************************//

    private void Awake()
    {
        Instance = this;
        //Hiding the game score number
        scoreText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //Subscribe to countdown text
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    private void OnDisable()
    {
        //Unsubscribe
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    private void OnCountdownFinished()
    {
        //Removing all UI layers while playing the game
        SetPageState(PageState.None);
        OnGameStarted(); //Event sent to TapController
        score = 0;
        gameOver = false;

        //Showing the score number
        scoreText.gameObject.SetActive(true);
    }

    private void OnPlayerDied()
    {
        //Hiding the score number
        scoreText.gameObject.SetActive(false);

        gameScoreString.GetComponent<Text>().text = "Score: " + score.ToString();

        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("Highscore");
        if (score > savedScore)
        {
            //New highscore
            PlayerPrefs.SetInt("Highscore", score); //Save highscore
        }
        SetPageState(PageState.GameOver);
    }

    private void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    //***********************************************//

    private void SetPageState(PageState state)
    {
        //Deactive or activate pages
        switch(state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                settingsPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                settingsPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                settingsPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                settingsPage.SetActive(false);
                break;
            case PageState.Settings:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                settingsPage.SetActive(true);
                break;
        }
    }

    //Public methods to create some events hooked up to the GameManager
    public void ConfirmGameOver()
    {
        //Activated when replay button is hit

        //Event to tell objects to reset
        OnGameOverConfirmed(); //Event sent to TapController
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        //Activated when play button is hit
        SetPageState(PageState.Countdown);
    }

    //Settings menu methods
    public void BackToHome()
    {
        SetPageState(PageState.Start);
    }

    public void SettingsDisplay()
    {
        SetPageState(PageState.Settings);
    }
}
