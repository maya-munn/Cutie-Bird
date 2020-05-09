using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{
    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    Text countdown;

    private void OnEnable()
    {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        //Display countdown text every second until 0
        int count = 3;
        for (int i = 0; i < 3; i++)
        {
            countdown.text = (count).ToString();
            count--;
            yield return new WaitForSeconds(1);
        }

        //Once this countdown has ended, create event
        OnCountdownFinished();
    }
}
