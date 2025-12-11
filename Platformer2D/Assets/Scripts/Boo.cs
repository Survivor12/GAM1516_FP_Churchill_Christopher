using UnityEngine;

public enum EBooState : byte
{
    Unknown,
    Sleeping,
    Chasing
}

public class Boo : Enemy
{
    public EnemySettings settings;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private EBooState state = EBooState.Unknown;

    public EBooState State
    {
        get { return state; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        enemyType = EEnemyType.Boo;

        SetState(EBooState.Sleeping);
    }

    // Update is called once per frame
    void Update()
    {
        float marioDirection = Game.Instance.GetMarioState.DirectionScalar;
        float marioLocationX = Game.Instance.MarioGameObject.transform.position.x;

        if (marioDirection < 0.0f)
        {
            if (marioLocationX < transform.position.x)
            {
                spriteRenderer.flipX = false;
                SetState(EBooState.Chasing);
            }
            else
            {
                SetState(EBooState.Sleeping);
            }
        }
        else if (marioDirection > 0.0f)
        {
            if (marioLocationX > transform.position.x)
            {
                spriteRenderer.flipX = true;
                SetState(EBooState.Chasing);
            }
            else
            {
                SetState(EBooState.Sleeping);
            }
        }

        Move();
    }

    void SetState(EBooState newState)
    {
        if (state != newState)
        {
            state = newState;
            
            if (state == EBooState.Sleeping)
            {
                animator.Play("BooSleeping");
            }
            else if (state == EBooState.Chasing)
            {
                animator.Play("BooChasing");
            }
        }
    }

    void Move()
    {
        if (state == EBooState.Chasing)
        {
            float marioLocationX = Game.Instance.MarioGameObject.transform.position.x;
            float marioLocationY = Game.Instance.MarioGameObject.transform.position.y;
            float locationX = transform.position.x;
            float locationY = transform.position.y;

            if (transform.position.x < marioLocationX)
            {
                locationX = transform.position.x + settings.BooSpeed * Time.deltaTime * Game.Instance.LocalTimeScale;
            }
            else if (transform.position.x > marioLocationX)
            {
                locationX = transform.position.x - settings.BooSpeed * Time.deltaTime * Game.Instance.LocalTimeScale;
            }

            if (transform.position.y < marioLocationY)
            {
                locationY = transform.position.y + settings.BooSpeed * Time.deltaTime * Game.Instance.LocalTimeScale;
                if (locationY > marioLocationY)
                {
                    locationY = marioLocationY;
                }
            }
            else if (transform.position.y > marioLocationY)
            {
                locationY = transform.position.y - settings.BooSpeed * Time.deltaTime * Game.Instance.LocalTimeScale;
                if (locationY < marioLocationY)
                {
                    locationY = marioLocationY;
                }
            }

            transform.position = new Vector3(locationX, locationY, -1.0f);
        }
    }
}
