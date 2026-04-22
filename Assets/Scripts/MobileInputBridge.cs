using UnityEngine;

public class MobileInputBridge : MonoBehaviour
{
    // Simple forwarding wrapper so UI can call methods even if Player is created at runtime.

    public void PressJump()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.PressJump();
    }

    public void PressDash()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.PressDash();
    }

    public void PressAttack()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.PressAttack();
    }

    // For simple left/right hold buttons: call on PointerDown and PointerUp (via EventTrigger)
    public void StartMoveLeft()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.StartMoveLeft();
    }

    public void StartMoveRight()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.StartMoveRight();
    }

    public void StopMove()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.StopMove();
    }

    // For virtual joystick you can call this every frame with a value -1..1
    public void SetMoveInput(float axis)
    {
        if (PlayerController.Instance != null) PlayerController.Instance.SetMoveInput(axis);
    }

    // Stop joystick (on release)
    public void StopMoveInput()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.StopMoveInput();
    }
}
