using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonInputState {
    public bool Held;
    public bool Down;
    public bool Up;

    public event Action Changed;

    public ButtonInputState() { }

    public void Update(InputAction action) {
        Held = action.IsPressed();
        Down = action.WasPressedThisFrame();
        Up = action.WasReleasedThisFrame();

        if (Down || Up) {
            Changed?.Invoke();
        }
    }
}
