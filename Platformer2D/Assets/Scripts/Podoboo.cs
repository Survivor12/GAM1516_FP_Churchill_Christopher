using UnityEngine;

public enum EPodobooState : byte
{
    Unknown,
    Hiding,
    Active
}

public class Podoboo : Enemy
{
    public EnemySettings settings;
    private EPodobooState state = EPodobooState.Unknown;

    private new Rigidbody2D rigidbody;
    private Animator animator;
    private Vector2 start;
    private float holdTimer = 0.0f;
    bool flipped = false;

    public EPodobooState State
    {
        get { return state; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the enemy type
        enemyType = EEnemyType.Podoboo;

        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = settings.PodobooGravityScale;

        animator = GetComponent<Animator>();
        animator.Play("Podoboo");

        start = transform.position;

        SetState(EPodobooState.Hiding);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EPodobooState.Hiding)
        {
            holdTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (holdTimer <= 0.0f)
            {
                holdTimer = 0.0f;
                SetState(EPodobooState.Active);
            }
        }
        else if (state == EPodobooState.Active)
        {
            if (rigidbody.linearVelocityY > 0.0f)
            {
                Vector3 scale = transform.localScale;
                scale.y = 1.0f;
                transform.localScale = scale;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.y = -1.0f;
                transform.localScale = scale;
                if (flipped == false)
                {
                    Vector2 location = transform.position;
                    location.y += 1.0f;
                    transform.position = location;
                    flipped = true;
                }
            }

            if (transform.position.y < start.y)
            {
                SetState(EPodobooState.Hiding);
            }
        }
    }

    private void SetState(EPodobooState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EPodobooState.Hiding)
            {
                transform.position = start;
                holdTimer = UnityEngine.Random.Range(settings.PodobooHiddenDurationMin, settings.PodobooHiddenDurationMax);
            }
            else if (state == EPodobooState.Active)
            {
                flipped = false;
                transform.position = start;
                rigidbody.linearVelocityY = 0.0f;
                rigidbody.AddForce(new Vector2(0.0f, settings.PodobooImpulse), ForceMode2D.Impulse);
                Vector2 location = transform.position;
                location.y += 1.0f;

                Game.Instance.SpawnSplash(location);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lava") && rigidbody.linearVelocityY < 0)
        {
            Vector2 location = transform.position;
            location.y -= 1.0f;
            Game.Instance.SpawnSplash(location);
        }
    }
}
