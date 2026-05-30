using UnityEngine;
using UnityEngine.AI;

public class HumanControlledNavigation : MonoBehaviour {
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float speedup = 0.1f;
    [SerializeField] private float drag = 0.9f;
    [SerializeField] private Rigidbody rb = null;

    [SerializeField] private HumanManager manager = null;

    private HumanAINavigation aiNavigation = null;

    private Vector3 velocity = Vector3.zero;

    private void Start() {
        aiNavigation = manager.aiNavigation;
    }

    private void Update() {
        if (aiNavigation.currentState == HumanAIStates.ControlledByPlayer) {
            ControlHuman();
        }
    }

    private void ControlHuman() {
        Vector2 input = InputManager.Instance.vectorInputs["Move"].Input;

        if (input.y == 0) {
            velocity *= drag;
        } else {
            velocity += transform.forward * (speedup * input.y * Time.deltaTime);
            if (velocity.magnitude > maxSpeed) {
                velocity = velocity.normalized * maxSpeed;
            }
        }

        rb.linearVelocity = velocity;
    }
}
