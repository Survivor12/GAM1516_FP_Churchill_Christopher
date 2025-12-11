using UnityEngine;

public enum ERotoDiscDirection : byte
{
    Clockwise,
    CounterClockwise
}

public class RotoDisc : Enemy
{
    public EnemySettings settings;
    public ERotoDiscDirection direction = ERotoDiscDirection.Clockwise;
    public Transform target;

    private float orbitSpeed;
    private Vector3 rotationAxis = Vector3.forward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orbitSpeed = settings.RotoDiscOrbitSpeed;

        enemyType = EEnemyType.RotoDisc;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction != ERotoDiscDirection.CounterClockwise)
        {
            transform.RotateAround(target.position, rotationAxis, -orbitSpeed * Time.deltaTime * Game.Instance.LocalTimeScale);
            transform.Rotate(rotationAxis * orbitSpeed * Time.deltaTime * Game.Instance.LocalTimeScale);
        }
        else
        {
            transform.RotateAround(target.position, rotationAxis, orbitSpeed * Time.deltaTime * Game.Instance.LocalTimeScale);
            transform.Rotate(rotationAxis * -orbitSpeed * Time.deltaTime * Game.Instance.LocalTimeScale);
        }
    }
}
