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

    private bool inBody => npcController != null;
    private float jumpTimer = 0f;
    private bool leftBody = false;

    private Rigidbody rb = null;
    private NPCController npcController = null;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        Jump();
    }

    private void LateUpdate() {
        if (inBody) {
            transform.position = npcController.hidePoint.position;
        }
    }

    private void Jump() {
        if (InputManager.Instance.buttonInputs["Jump"].Down) {
            jumpTimer = 0;
        }

        if (InputManager.Instance.buttonInputs["Jump"].Held) {
            jumpTimer += Time.deltaTime;
        }

        if (InputManager.Instance.buttonInputs["Jump"].Up) {
            if (!inBody) {
                Vector3 forward = transform.position - virusCam.transform.position;
                forward.y = 0;
                forward.Normalize();

                float force = Mathf.Lerp(minJumpForce, maxJumpForce, Mathf.Clamp01(jumpTimer / maxHoldLength));
                ApplyForce(force, jumpAngle, forward);
            } else {
                Vector3 forward = npcController.transform.forward;

                ExitBody();

                float force = Mathf.Lerp(minSneezeForce, maxSneezeForce, Mathf.Clamp01(jumpTimer / maxHoldLength));
                ApplyForce(force, sneezeAngle, forward);
            }
        }
    }

    private void ApplyForce(float force, float angle, Vector3 direction) {
        Vector3 right = Vector3.Cross(direction, Vector3.up);

        Vector3 jumpForce = Quaternion.AngleAxis(-angle, right) * direction;
        rb.AddForce(jumpForce * force, ForceMode.Impulse);
    }

    private void EnterBody(NPCController controller) {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        leftBody = true;
        rb.excludeLayers = LayerMask.NameToLayer("NPC");

        npcController = controller;
        npcCam.Follow = controller.transform;
        npcCam.LookAt = controller.transform;

        virusCam.gameObject.SetActive(false);
        npcCam.gameObject.SetActive(true);
    }

    private void ExitBody() {
        rb.useGravity = true;

        npcController = null;

        virusCam.gameObject.SetActive(true);
        npcCam.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        switch (LayerMask.LayerToName(collision.gameObject.layer)) {
            case "Default":
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                break;
            case "NPC":
                NPCController controller = collision.gameObject.GetComponent<NPCController>();
                EnterBody(controller);
                break;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (!leftBody) {
            leftBody = true;
            rb.excludeLayers = 0;
        }
    }
}