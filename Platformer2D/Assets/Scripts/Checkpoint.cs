using UnityEngine;

public enum ECheckpointState : byte
{
    Unknown,
    Default,
    Animating,
    Mario
}

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    private ECheckpointState state = ECheckpointState.Unknown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        SetState(ECheckpointState.Default);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetState(ECheckpointState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == ECheckpointState.Default)
            {
                animator.Play("CheckpointDefault");
            }
            else if (state == ECheckpointState.Animating)
            {
                animator.Play("CheckpointAnimating");
            }
            else if (state == ECheckpointState.Mario)
            {
                animator.Play("CheckpointMario");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (state == ECheckpointState.Default)
            {
                SetState(ECheckpointState.Animating);

                Game.Instance.HandleCheckpoint(this);
            }
        }
    }

    private void OnAnimationFinished()
    {
        if (state == ECheckpointState.Animating)
        {
            SetState(ECheckpointState.Mario);
        }
    }
}
