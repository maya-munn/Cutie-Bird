using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    private class PoolObject
    {
        //Keep track of all of the objects of a certain type (Pipes, clouds, stars)
        //Responsible for determining if that object is in use.

        public Transform transform;
        public bool inUse;  //Determine if its available or not

        //Constructor
        public PoolObject(Transform t) { transform = t; }   //Not recommended coding style

        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        //Min and max height for spawns
        public float min;   
        public float max;
    }

    //Keep track of what type of pre-fab we will be spawning
    public GameObject prefab;
    public int poolSize; //Keeps track of how many objects to spawn
    public float shiftSpeed;
    public float spawnRate; //How often are these objects spawning

    //Position info
    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    //Responsible for setting up parallex objects
    public bool spawnImmediate; //True for everything except pipes, is like particle prewarm
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;  //Ensures pipes and other objects not being spawned in screen space

    private float spawnTimer;
    float targetAspect;
    private PoolObject[] poolObjects;
    GameManager game;

    private void Awake()
    {
        Configure();
    }

    private void Start()
    {
        //Init game
        game = GameManager.Instance;
    }

    private void OnEnable()
    {
        //Subscribe to game manager
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }
    private void OnDisable()
    {
        //Unsubscribe
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameOverConfirmed()
    {
        //Handling disposing pool objects and reconfiguring them

        //Dispose all of our objects
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000; //Put off screen
        }

        if (spawnImmediate) { SpawnImmediate(); }
    }

    private void Update()
    {
        //Dont need to be updating if game over
        if (game.GameOver) return;

        Shift(); //Shifting parallex objects

        //Increase spawn timer
        spawnTimer += Time.deltaTime;
        //Check if anything needs to be spawned
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    private void Configure()
    {
        //10 x 16 default - set
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;

        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            //Creating object, spawning prefab into game as gameobject
            GameObject gameObj = Instantiate(prefab) as GameObject;
            //Store the transform
            Transform t = gameObj.transform;
            //Set parent to current object
            t.SetParent(transform);

            //Set position off screen
            t.position = Vector3.one * 1000;

            //Initialise value within poolobjects array
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    private void Spawn()
    {
        //Everytime we spawn something, get first available pool object
        Transform t = GetPoolObject();
        if (t == null) return; //If true, this indicates pool size is too small

        //Define position we want parallex object to be placed in
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect; //Needs to be dependent on target aspect
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos; //t coming from one of our pool objects, set pos and consider it spawned
    }

    private void SpawnImmediate()
    {
        //Cares about customising (prewarming) position spawned immediately
        //Spawns two objects

        //Everytime we spawn something, get first available pool object
        Transform t = GetPoolObject();
        if (t == null) return; //If true, this indicates pool size is too small

        //Define position we want parallex object to be placed in
        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect; //Needs to be dependent on target aspect
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos; //t coming from one of our pool objects, set pos and consider it spawned

        Spawn();
    }

    private void Shift()
    {
        //How to move parallex objects
        for (int i = 0; i < poolObjects.Length; i++)
        {
            //Moving based on shift speed
            poolObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;

            //Check to see if object needs to dispose
            CheckDisposeObject(poolObjects[i]);
        }
    }

    private void CheckDisposeObject(PoolObject poolObject)
    {
        //Check to see if parallex object off screen
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect) //should spawn on the negative position off screen
        {
            poolObject.Dispose();
            //Set pos somewhere off screen
            poolObject.transform.position = Vector3.one * 1000; //Ridiculous value somewhere else
        }
    }

    private Transform GetPoolObject()
    {
        //Return pool objects transform

        //Get first available object in pool objects array
        for (int i = 0; i < poolObjects.Length; i++)
        {
            //If not in use
            if (!poolObjects[i].inUse)
            {
                //Make sure it cant be selected again until it goes off screen
                poolObjects[i].Use();   //Set it in use
                return poolObjects[i].transform;
            }
        }

        return null;
    }
}
