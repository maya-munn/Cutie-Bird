using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighScoreText : MonoBehaviour
{
    Text highscoreText;

    private void OnEnable()
    {
        highscoreText = GetComponent<Text>();
        highscoreText.text = "High Score: " + PlayerPrefs.GetInt("Highscore").ToString();
    }
}
