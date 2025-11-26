using UnityEngine;


public enum EDoorType : byte
{
    Unknown,
    Exit,
    Entrance
}

public class Door : MonoBehaviour
{
    public Door counterpart;
    public EDoorType type;

    public void DoorAction()
    {
        if (type == EDoorType.Exit)
        {
            Game.Instance.HandleDoor(counterpart);
        }
    }
}
