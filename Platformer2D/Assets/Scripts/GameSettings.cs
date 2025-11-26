using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Mario/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float DefaultGameDuration = 300.0f;
    public float DestroyActorAtY = -8.0f;

    public float BlackOverlayFadeInOutDuration = 0.7f;
    public float BlackOverlayFadeHoldDuration = 0.1f;
}
