using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class VirusController : MonoBehaviour {
    [Header("Jump")]
    [SerializeField] private float maxHoldLength = 1f;
    [SerializeField] private float minJumpForce = 1f;
    [SerializeField] private float maxJumpForce = 5f;
    [SerializeField] private float jumpAngle = 45f;

    [SerializeField] private float minSneezeForce = 30f;
    [SerializeField] private float maxSneezeForce = 45f;
    [SerializeField] private float sneezeAngle = 30f;

    [Header("References")]
    [SerializeField] private CinemachineCamera virusCam = null;
    [SerializeField] private CinemachineCamera npcCam = null;

    private bool inBody => human != null;
    private float jumpTimer = 0f;
    private bool leftBody = false;
    private bool isGrounded => colliderCount > 0;
    private int colliderCount = 0;

    private Rigidbody rb = null;
    private HumanManager human = null;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        CheckStartControl();
        Jump();
    }

    private void LateUpdate() {
        if (inBody) {
            transform.position = human.hidePoint.position;
        }
    }

    private void CheckStartControl() {
        if (!inBody || 
            human.aiNavigation.currentState == HumanAIStates.ControlledByPlayer ||
            human.aiNavigation.currentState == HumanAIStates.Dead) 
        {
            return;
        }

        if (InputManager.Instance.buttonInputs["Control"].Down) {
            human.humanControlledNavigati0n.npcCam = npcCam.transform;
            human.aiNavigation.SetState(HumanAIStates.ControlledByPlayer);
        }
    }

    private void Jump() {
        if (InputManager.Instance.buttonInputs["Jump"].Down) {
            jumpTimer = 0f;
        }

        if (InputManager.Instance.buttonInputs["Jump"].Held && (inBody || isGrounded)) {
            jumpTimer += Time.deltaTime;
        }

        if (InputManager.Instance.buttonInputs["Jump"].Up) {
            if (jumpTimer == 0f) {
                return;
            }

            if (!inBody) {
                if (isGrounded) {
                    Vector3 forward = transform.position - virusCam.transform.position;
                    forward.y = 0;
                    forward.Normalize();

                    float force = Mathf.Lerp(minJumpForce, maxJumpForce, Mathf.Clamp01(jumpTimer / maxHoldLength));
                    ApplyForce(force, jumpAngle, forward);
                }
            } else {
                Vector3 forward = human.transform.forward;

                ExitBody();

                float force = Mathf.Lerp(minSneezeForce, maxSneezeForce, Mathf.Clamp01(jumpTimer / maxHoldLength));
                ApplyForce(force, sneezeAngle, forward);
            }
        }
    }

    private void ApplyForce(float force, float angle, Vector3 direction) {
        rb.useGravity = true;
        Vector3 right = Vector3.Cross(direction, Vector3.up);

        Vector3 jumpForce = Quaternion.AngleAxis(-angle, right) * direction;
        rb.AddForce(jumpForce * force, ForceMode.Impulse);
    }

    private void EnterBody(HumanManager human) {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        leftBody = true;
        rb.excludeLayers = LayerMask.NameToLayer("Human");

        this.human = human;
        npcCam.Follow = human.transform;
        npcCam.LookAt = human.transform;

        human.infectionLevel.SetHostingVirus(true);

        virusCam.gameObject.SetActive(false);
        npcCam.gameObject.SetActive(true);
    }

    private void ExitBody() {
        rb.useGravity = true;

        human.infectionLevel.SetHostingVirus(false);
        human = null;

        virusCam.gameObject.SetActive(true);
        npcCam.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other) {
        switch (LayerMask.LayerToName(other.gameObject.layer)) {
            case "Default":
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                break;
            case "Human":
                HumanManager human = other.gameObject.GetComponent<HumanManager>();
                EnterBody(human);
                break;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (!leftBody) {
            leftBody = true;
            rb.excludeLayers = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Default") {
            colliderCount++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Default") {
            colliderCount--;
        }
    }
}