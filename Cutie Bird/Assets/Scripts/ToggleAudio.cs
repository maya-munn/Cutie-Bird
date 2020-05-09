using UnityEngine;
using UnityEngine.UI;

public class ToggleAudio : MonoBehaviour
{
    public Button audioButton;
    public Button muteButton;

    public AudioSource tap;
    public AudioSource faint;
    public AudioSource score;
    public AudioSource music;

    public void Start()
    {
        audioButton.GetComponent<Button>();
        muteButton.GetComponent<Button>();
        muteButton.interactable = true;
        audioButton.interactable = false;

        tap.GetComponent<AudioSource>();
        faint.GetComponent<AudioSource>();
        score.GetComponent<AudioSource>();
        music.GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        audioButton.interactable = false;
        muteButton.interactable = true;
        tap.volume = 0.8f;
        faint.volume = 1f;
        score.volume = 1f;
        music.volume = 0.15f;
        TapController.playAudio = true;
    }

    public void MuteAudio()
    {
        audioButton.interactable = true;
        muteButton.interactable = false;
        tap.volume = 0f;
        faint.volume = 0f;
        score.volume = 0f;
        music.volume = 0f;
        TapController.playAudio = false;
    }
}
