using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameState : byte
{
    Unknown,
    Gameplay,
    FadeOut,
    FadeHold,
    FadeIn,
    GameWon
};

public class Game : MonoBehaviour
{
    private static Game sInstance;

    public MarioCamera marioCamera;

    public GameSettings settings;
    public GameObject marioGameObject;
    public GameObject deadMarioPrefab;
    public GameObject mushroomPickupPrefab;
    public GameObject itemBoxCoinPrefab;
    public GameObject breakableBlockBitPrefab;
    public GameObject splashPrefab;

    private GameObject deadMario = null;

    private Door currentDoor = null;
    private Checkpoint currentCheckpoint = null;

    private Vector2 marioSpawnLocation = Vector2.zero;
    private float localTimeScale = 1.0f;
    private float timeRemaining = 0.0f;
    private float blackOverlayAlpha = 0.0f;
    private float fadeInOutTimer = 0.0f;
    private float fadeHoldTimer = 0.0f;
    private EGameState state = EGameState.Unknown;
    private bool isGameOver = false;
    private bool isGameWon = false;
    private bool endLevel = false;

    public GameSettings Settings
    {
        get { return settings; }
    }

    public static Game Instance
    {
        get { return sInstance; }
    }

    public MarioCamera GetMarioCamera
    {
        get { return marioCamera; }
    }

    public GameObject MarioGameObject
    {
        get { return marioGameObject; }
    }

    public Mario GetMario
    {
        get { return marioGameObject.GetComponent<Mario>(); }
    }

    public MarioState GetMarioState
    {
        get { return marioGameObject.GetComponent<MarioState>(); }
    }

    public MarioMovement GetMarioMovement
    {
        get { return marioGameObject.GetComponent<MarioMovement>(); }
    }

    public float LocalTimeScale
    {
        get { return localTimeScale; }
    }

    public float TimeRemaining
    {
        get { return timeRemaining; }
    }

    public float BlackOverlayAlpha
    {
        get { return blackOverlayAlpha; }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    public bool IsGameWon
    {
        get { return isGameWon; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Setup the static instance of the Game class
        if (sInstance != null && sInstance != this)
        {
            Destroy(this);
        }
        else
        {
            sInstance = this;
        }

        // Get Mario's spawn location
        marioSpawnLocation = marioGameObject.transform.position;

        // Set the timeRemaining variable to the setting's default game duration
        timeRemaining = settings.DefaultGameDuration;

        // Set the game's state to gameplay
        SetState(EGameState.FadeHold);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EGameState.Gameplay)
        {
            if (deadMario != null)
            {
                if (deadMario.transform.position.y < settings.DestroyActorAtY)
                {
                    Destroy(deadMario);
                    deadMario = null;

                    UnpauseActors();

                    SetState(EGameState.FadeOut);
                }
            }

            // Countdown the time remaining timer
            timeRemaining -= Time.deltaTime * LocalTimeScale;

            if (timeRemaining < 0.0f)
            {
                timeRemaining = 0.0f;
                GetMario.HandleDamage(true); // Mario is dead
            }
        }
        else if (state == EGameState.FadeOut)
        {
            fadeInOutTimer -= Time.deltaTime;

            float elapsed = Mathf.Max(0.0f, settings.BlackOverlayFadeInOutDuration - fadeInOutTimer);
            float alpha = elapsed / settings.BlackOverlayFadeInOutDuration;
            
            blackOverlayAlpha = alpha;

            if (fadeInOutTimer <= 0.0f)
            {
                fadeInOutTimer = 0.0f;
                if (endLevel)
                {
                    SetState(EGameState.GameWon);
                }
                else
                {
                    SetState(EGameState.FadeHold);
                }
            }
        }
        else if (state == EGameState.FadeHold)
        {
            fadeHoldTimer -= Time.deltaTime;

            if (fadeHoldTimer <= 0.0f)
            {
                fadeHoldTimer = 0.0f;
                SetState(EGameState.FadeIn);
            }
        }
        else if (state == EGameState.FadeIn)
        {
            fadeInOutTimer -= Time.deltaTime;

            float elapsed = Mathf.Max(0.0f, settings.BlackOverlayFadeInOutDuration - fadeInOutTimer);
            float alpha = 1.0f - (elapsed / settings.BlackOverlayFadeInOutDuration);

            blackOverlayAlpha = alpha;

            if (fadeInOutTimer <= 0.0f)
            {
                fadeInOutTimer = 0.0f;
                SetState(EGameState.Gameplay);
            }
        }
    }

    public void PauseActors()
    {
        localTimeScale = 0.0f;

        // get root objects in scene
        List<GameObject> gameObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(gameObjects);

        // iterate root objects and do something
        for (int i = 0; i < gameObjects.Count; ++i)
        {
            if (gameObjects[i].CompareTag("Mario"))
            {
                gameObjects[i].GetComponent<MarioMovement>().Pause();
            }
            else
            {
                Animator animator = gameObjects[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 0.0f;
                }
            }
        }
    }

    public void UnpauseActors()
    {
        localTimeScale = 1.0f;

        // get root objects in scene
        List<GameObject> gameObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(gameObjects);

        // iterate root objects and do something
        for (int i = 0; i < gameObjects.Count; ++i)
        {
            if (gameObjects[i].CompareTag("Mario"))
            {
                gameObjects[i].GetComponent<MarioMovement>().Unpause();
            }
            else
            {
                Animator animator = gameObjects[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 1.0f;
                }
            }
        }
    }

    public void HandleDoor(Door door)
    {
        currentDoor = door;
        SetState(EGameState.FadeOut);
    }

    public void HandleCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void HandleGameWon()
    {
        PauseActors();
        SetState(EGameState.FadeOut);
        endLevel = true;
    }

    public void MarioHasDied(bool spawnDeadMario)
    {
        // Get Mario's player state and decrease the Lives value by one
        MarioState marioState = GetMarioState;

        if (marioState != null)
        {
            if (marioState.Lives > 0)
            {
                marioState.Lives--;

                // Do we spawn dead mario or not?
                if (spawnDeadMario)
                {
                    SpawnDeadMario(marioGameObject.transform.position);
                }
                else
                {
                    // If not fade out immediately
                    SetState(EGameState.FadeOut);
                }

                OnMarioDied();
            }
            else
            {
                isGameOver = true;
                OnGameOver();
            }
        }
    }

    public void SpawnMushroomPickup(Vector2 location)
    {
        if (mushroomPickupPrefab != null)
        {
            GameObject mushroomObject = Instantiate(mushroomPickupPrefab, new Vector3(location.x, location.y, 1.0f), Quaternion.identity);
            MushroomPickup mushroomPickup = mushroomObject.GetComponent<MushroomPickup>();
            mushroomPickup.Spawn();
        }
    }

    public void SpawnItemBoxCoin(Vector2 location)
    {
        if (itemBoxCoinPrefab != null)
        {
            Instantiate(itemBoxCoinPrefab, new Vector3(location.x, location.y, 1.0f), Quaternion.identity);
        }
    }

    public void SpawnBreakableBlockBits(Vector2 location, Vector2 impulse, EBreakableBlockBitType type)
    {
        if (breakableBlockBitPrefab != null)
        {
            GameObject breakableBlockBitObject = Instantiate(breakableBlockBitPrefab, new Vector3(location.x, location.y, -1.0f), Quaternion.identity);
            BreakableBlockBit breakableBlockBit = breakableBlockBitObject.GetComponent<BreakableBlockBit>();
            breakableBlockBit.Spawn(type, impulse);
        }
    }

    private void SpawnDeadMario(Vector2 location)
    {
        if (deadMario == null)
        {
            PauseActors();

            if (deadMarioPrefab != null)
            {
                deadMario = Instantiate(deadMarioPrefab, new Vector3(location.x, location.y, -1.5f), Quaternion.identity);
            }
        }
    }

    public void SpawnSplash(Vector2 location)
    {
        if (splashPrefab != null)
        {
            Instantiate(splashPrefab, new Vector3(location.x, location.y, 1.0f), Quaternion.identity);
        }
    }

    private void OnGameplay()
    {

    }

    private void OnMarioDied()
    {

    }

    private void OnGameOver()
    {

    }

    private Vector3 GetSpawnLocation()
    {
        if (currentDoor != null)
        {
            return currentDoor.transform.position;
        }

        if (currentCheckpoint != null)
        {
            Vector3 location = currentCheckpoint.transform.position;
            location.z = 0.0f;
            return location;
        }

        return marioSpawnLocation;
    }

    private void SetState(EGameState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EGameState.Gameplay)
            {
                blackOverlayAlpha = 0.0f;
                OnGameplay();
            }
            else if (state == EGameState.FadeOut)
            {
                blackOverlayAlpha = 0.0f;
                fadeInOutTimer = settings.BlackOverlayFadeInOutDuration;
            }
            else if (state == EGameState.FadeHold)
            {
                blackOverlayAlpha = 1.0f;
                fadeHoldTimer = settings.BlackOverlayFadeHoldDuration;
            }
            else if (state == EGameState.FadeIn)
            {
                blackOverlayAlpha = 1.0f;

                if (GetMarioState.State == EMarioState.Dead)
                {
                    timeRemaining = settings.DefaultGameDuration;
                }

                // Respawn Mario
                Vector3 spawnLocation = GetSpawnLocation();
                GetMario.ResetMario(spawnLocation);

                // Set the Camera to mario's location
                marioCamera.SetCameraLocation(spawnLocation);

                // We are done transiting rooms, set the CurrentDoor pointer to null
                currentDoor = null;

                // Set the fade out timer
                fadeInOutTimer = settings.BlackOverlayFadeInOutDuration;
            }
            else if (state == EGameState.GameWon)
            {
                isGameWon = true;
            }
        }
    }
}
