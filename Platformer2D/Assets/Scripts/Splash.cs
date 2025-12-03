using UnityEngine;

public class Splash : MonoBehaviour
{
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Splash");
    }

    private void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}
