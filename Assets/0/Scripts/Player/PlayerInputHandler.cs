using UnityEngine;
using Zenject;
using System;

public class PlayerInputHandler : IInitializable, IDisposable
{
    public InputSystem_Actions inputActions;

    public Vector3 Get_MovementVector =>
     inputActions?.Player.Move.ReadValue<Vector2>() is Vector2 v
         ? new Vector3(v.x, 0f, v.y)
         : Vector3.zero;


    public Vector3 Get_MovementVectorSnapped
    {
        get
        {
            Vector3 dir = Get_MovementVector;

            dir.x = Mathf.Round(dir.x);
            dir.z = Mathf.Round(dir.z);

            if (dir != Vector3.zero)
                dir.Normalize();

            return dir;
        }
    }

    public bool Get_InteractPressed => inputActions?.Player.Interact.WasPerformedThisFrame() ?? false;
    public bool Get_AttackPressed => inputActions?.Player.Attack.WasPerformedThisFrame() ?? false;
    public bool Get_DashPressed => inputActions?.Player.Dash.WasPerformedThisFrame() ?? false;

    public void Initialize()
    {
        Enable();
    }

    public void Enable()
    {
        if (inputActions == null)
            inputActions = new InputSystem_Actions();

        inputActions.Player.Enable();
    }

    public void Disable()
    {
        if (inputActions == null) return;

        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    public void Dispose()
    {
        if (inputActions == null) return;

        inputActions.Dispose();
        inputActions = null;
    }
}