using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Mario/EnemySettings")]
public class EnemySettings : ScriptableObject
{
    [Header("Piranha Plant")]
    public float PiranhaPlantOffsetY = 1.5f;
    public float PiranhaPlantAnimationDuration = 0.75f;
    public float PiranhaPlantActiveDuration = 2.5f;
    public float PiranhaPlantHiddenDurationMin = 2.0f;
    public float PiranhaPlantHiddenDurationMax = 4.0f;

    [Header("Goomba")]
    public float GoombaSpeed = 3.0f;
    public float GoombaSquishedDuration = 0.6f;

    [Header("Podoboo")]
    public float PodobooHiddenDurationMin = 2.0f;
    public float PodobooHiddenDurationMax = 4.0f;
    public float PodobooGravityScale = 1.0f;
    public float PodobooImpulse = 21.0f;

    [Header("Boo")]
    public float BooSpeed = 2.0f;
}
