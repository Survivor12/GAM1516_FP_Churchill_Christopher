using UnityEngine;

public enum EBreakableBlockState : byte
{
    Unknown,
    Active,
    AnimUp,
    AnimDown,
    Breaking
}

public class BreakableBlock : MonoBehaviour
{
    public PickupSettings settings;
    private EBreakableBlockState state = EBreakableBlockState.Unknown;
    private Animator animator;
    private Vector2 start;
    private Vector2 target;
    private Vector2 initial;
    private float animationTimer;

    private const int kNumBits = 4;
    private static readonly EBreakableBlockBitType[] kBreakableBlockBitTypes = new[] { EBreakableBlockBitType.Left, EBreakableBlockBitType.Left, EBreakableBlockBitType.Right, EBreakableBlockBitType.Right };
    private static readonly Vector2[] kBreakableBlockBitOffsets = { new Vector2(-0.25f, 0.25f), new Vector2(-0.25f, -0.25f), new Vector2(0.25f, 0.25f), new Vector2(0.25f, -0.25f) };
    private static readonly Vector2[] kBreakableBlockBitImpulses = { new Vector2(-4.6875f, 10.9375f), new Vector2(-4.6875f, 9.375f), new Vector2(4.6875f, 10.9375f), new Vector2(4.6875f, 9.375f) };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        SetState(EBreakableBlockState.Active);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EBreakableBlockState.AnimUp)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / settings.ItemBoxAnimationDuration);
            float x = Mathf.Lerp(start.x, target.x, pct);
            float y = Mathf.Lerp(start.y, target.y, pct);
            transform.position = new Vector2(x, y);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EBreakableBlockState.AnimDown);
            }
        }
        else if (state == EBreakableBlockState.AnimDown)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / settings.ItemBoxAnimationDuration);
            float x = Mathf.Lerp(start.x, target.x, pct);
            float y = Mathf.Lerp(start.y, target.y, pct);
            transform.position = new Vector2(x, y);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EBreakableBlockState.Active);
            }
        }
    }

    private void SetState(EBreakableBlockState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EBreakableBlockState.Active)
            {
                animator.Play("BreakableBlock");
            }
            else if (state == EBreakableBlockState.AnimUp)
            {
                initial = transform.position;
                start = initial;
                target = start + new Vector2(0.0f, 0.25f);

                animationTimer = settings.ItemBoxAnimationDuration;
            }
            else if (state == EBreakableBlockState.AnimDown)
            {
                start = target;
                target = initial;

                animationTimer = settings.ItemBoxAnimationDuration;
            }
            else if (state == EBreakableBlockState.Breaking)
            {
                Vector2 location = transform.position;
                
                for (int i = 0; i < kNumBits; i++)
                {
                    Game.Instance.SpawnBreakableBlockBits(location + kBreakableBlockBitOffsets[i], kBreakableBlockBitImpulses[i], kBreakableBlockBitTypes[i]);
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y >= 0.8f)
            {
                if (state == EBreakableBlockState.Active)
                {
                    if (Game.Instance.GetMarioState.Form == EMarioForm.Small)
                    {
                        SetState(EBreakableBlockState.AnimUp);
                    }
                    else
                    {
                        //Stop mario from jumping when breaking a block
                        collision.gameObject.GetComponent<MarioMovement>().StopJumping();

                        SetState(EBreakableBlockState.Breaking);
                    }
                }
            }
        }
    }
}
