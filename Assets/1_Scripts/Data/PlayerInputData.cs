using UnityEngine;
using Fusion;

public struct PlayerInputData : INetworkInput
{
    public const uint BUTTON_JUMP = 1 << 0;

    public const uint BUTTON_FORWARD = 1 << 1;
    public const uint BUTTON_BACKWARD = 1 << 2;
    public const uint BUTTON_LEFT = 1 << 3;
    public const uint BUTTON_RIGHT = 1 << 4;

    public Vector2 RotationDelta;
    public uint buttons;

    public bool IsPressed(uint button) => (buttons & button) == button;

    public bool IsReleased(uint button) => IsPressed(button) == false;
}
