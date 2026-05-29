using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VectorInputState {
    public Vector2 Input;

    public VectorInputState() { }

    public void Update(InputAction action) {
        Input = action.ReadValue<Vector2>();
    }
}
