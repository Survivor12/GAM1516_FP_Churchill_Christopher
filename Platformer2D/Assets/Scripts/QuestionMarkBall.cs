using UnityEngine;

public class QuestionMarkBall : Pickup
{
    protected new Rigidbody2D rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        pickupType = EPickupType.QuestionMarkBall;
    }
}
