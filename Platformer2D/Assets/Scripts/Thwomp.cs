using UnityEngine;

public enum EThwompState : byte
{
    Unknown,
    Waiting,
    Falling,
    Holding,
    Ascending
}

public class Thwomp : Enemy
{
    public EnemySettings settings;
    public float bottomY = 0.0f;

    private EThwompState state = EThwompState.Unknown;
    private Vector2 startingLocation = Vector2.zero;
    private float fallSpeed = 0.0f;
    private float holdTimer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingLocation = transform.position;

        // Set the enemy type
        enemyType = EEnemyType.Thwomp;

        SetState(EThwompState.Waiting);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EThwompState.Waiting)
        {
            float marioLocationX = Game.Instance.MarioGameObject.transform.position.x;

            if (marioLocationX > transform.position.x - settings.ThwompTriggerDistance && marioLocationX < transform.position.x + settings.ThwompTriggerDistance)
            {
                SetState(EThwompState.Falling);
            }
        }
        else if (state == EThwompState.Falling)
        {
            float locationY = transform.position.y + fallSpeed * Time.deltaTime * Game.Instance.LocalTimeScale;
            if (locationY <= bottomY)
            {
                locationY = bottomY;
                SetState(EThwompState.Holding);
            }
            transform.position = new Vector2(transform.position.x, locationY);
        }
        else if (state == EThwompState.Holding)
        {
            holdTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (holdTimer <= 0.0f)
            {
                holdTimer = 0.0f;
                SetState(EThwompState.Ascending);
            }
        }
        else if (state == EThwompState.Ascending)
        {
            float locationY = transform.position.y + fallSpeed * Time.deltaTime * Game.Instance.LocalTimeScale;
            transform.position = new Vector2(transform.position.x, locationY);

            if (transform.position.y >= startingLocation.y)
            {
                transform.position = startingLocation;
                SetState(EThwompState.Waiting);
            }
        }
    }

    void SetState(EThwompState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EThwompState.Waiting)
            {
                fallSpeed = 0.0f;
                transform.position = startingLocation;
            }
            else if (state == EThwompState.Falling)
            {
                fallSpeed = settings.ThwompFallSpeed;
            }
            else if (state == EThwompState.Holding)
            {
                fallSpeed = 0.0f;
                holdTimer = settings.ThwompHoldTimer;
                Game.Instance.GetMarioCamera.ApplyCameraShake();
            }
            else if (state == EThwompState.Ascending)
            {
                fallSpeed = -settings.ThwompFallSpeed / 2.0f;
            }
        }
    }
}
