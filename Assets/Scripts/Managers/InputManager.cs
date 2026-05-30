using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    [SerializeField] private InputActionAsset inputActions = null;

    public Dictionary<string, ButtonInputState> buttonInputs = new Dictionary<string, ButtonInputState>();
    public Dictionary<string, VectorInputState> vectorInputs = new Dictionary<string, VectorInputState>();

    public static InputManager Instance = null;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        InitializeInput();
    }

    private void InitializeInput() {
        foreach (InputAction action in inputActions.FindActionMap("Player")) {
            if (action.type == InputActionType.Button) {
                buttonInputs[action.name] = new ButtonInputState();
            } else if (action.type == InputActionType.Value) {
                vectorInputs[action.name] = new VectorInputState();
            }
        }
    }

    private void Update() {
        GatherInput();
    }

    private void GatherInput() {
        if (inputActions == null) {
            return;
        }

        foreach (InputAction action in inputActions.FindActionMap("Player")) {
            if (action.type == InputActionType.Button && buttonInputs.ContainsKey(action.name)) {
                buttonInputs[action.name].Update(action);
            } else if (action.type == InputActionType.Value && vectorInputs.ContainsKey(action.name)) {
                vectorInputs[action.name].Update(action);
            }
        }
    }
}