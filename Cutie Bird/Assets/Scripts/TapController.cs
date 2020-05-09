using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;    //How fast to move from one rotation to the next
    public Vector3 startPos;

    //Audio Sources
    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource faintAudio;
    public static bool playAudio = true;

    private Rigidbody2D rigidBody;
    private Quaternion downRotation; //Secure rotation
    private Quaternion forwardRotation;

    private GameManager gameManager;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -60); //Taking a Vector 3 and turning into Quaternion
        forwardRotation = Quaternion.Euler(0, 0, 35);
        gameManager = GameManager.Instance;
        rigidBody.simulated = false;
    }

    private void Update()
    {
        //Don't do any updating if in game over state
        if (gameManager.GameOver) return;

        //Translates as tap for mobile devices
        if (Input.GetMouseButtonDown(0))
        {
            //Play tap audio
            if (playAudio) { tapAudio.Play(); }

            //Every time tapped, make bird rotate forward
            transform.rotation = forwardRotation;

            //Reset velocity
            rigidBody.velocity = Vector3.zero;

            //Move bird when tap input received
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        //Slowly move bird down and rotate it downwards no matter what
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    private void OnEnable()
    {
        //Subscribing 
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        //Unsubscribing
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameStarted()
    {
        //If velocity is not reset to 0, there is a build up of velocity from previous game
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
    }

    private void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    //*********************************************************//


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone")
        {
            //Register a score event
            OnPlayerScored(); //Event sent to GameManager

            //Play a sound
            if (playAudio) { scoreAudio.Play(); }
        }

        if (collision.gameObject.tag == "DeadZone")
        {
            //Freeze the bird when he hits a deadzone
            rigidBody.simulated = false;

            //Register a dead event
            OnPlayerDied(); //Event sent to GameManager
            //Play a sound
            if (playAudio) { faintAudio.Play(); }

            //Later implementation - Change the sprite when the bird hits a deadzone
        }
    }
}
