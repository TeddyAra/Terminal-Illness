using UnityEngine;

public class HumanControlledNavigation : MonoBehaviour {
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f; 
    [SerializeField] private float speedup = 0.1f;
    [SerializeField] private float drag = 0.9f;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private float gravity = -9.81f;
    public Rigidbody rb = null;

    [SerializeField] private HumanManager manager = null;

    private HumanAINavigation aiNavigation = null;
    [HideInInspector] public Transform npcCam = null;

    public bool sprintInput { get; private set; }

    private Vector3 velocity = Vector3.zero;

    private void Start() {
        aiNavigation = manager.aiNavigation;
    }

    private void FixedUpdate() {
        if (aiNavigation.currentState == HumanAIStates.ControlledByPlayer) {
            ControlHuman();
        }
    }

    private void ControlHuman() {
        Vector2 input = InputManager.Instance.vectorInputs["Move"].Input;
        sprintInput = InputManager.Instance.buttonInputs["Sprint"].Held;

        velocity.y = 0f;

        if (input.y == 0) {
            velocity *= drag;
        } else {
            velocity += transform.forward * (speedup * input.y);

            float clamp = input.y < 0f ? 0.5f : 1f;
            if (velocity.magnitude > maxSpeed * clamp) {
                velocity = velocity.normalized * maxSpeed * clamp;

                if (sprintInput) {
                    velocity *= sprintSpeedMultiplier; 
                }
            }
        }

        //velocity.y = gravity;
        rb.linearVelocity = velocity;

        if (npcCam == null) {
            return;
        }

        Vector3 forward = transform.position - npcCam.position;
        forward.y = 0f;
        forward.Normalize();

        Quaternion rotate = Quaternion.FromToRotation(transform.forward, forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rotate, rotateSpeed);
    }
}