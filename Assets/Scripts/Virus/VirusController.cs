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

    [SerializeField] private ParticleSystem slimeBurst = null;

    private bool inBody => human != null;
    private float jumpTimer = 0f;
    private bool isGrounded = false;
    private bool ignoreCollisions = false;

    private Collider col = null;
    private Rigidbody rb = null;
    private HumanManager human = null;
    private Transform lastHuman = null;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update() {
        CheckIsGrounded();
        CheckStartControl();
        Jump();
    }

    private void LateUpdate() {
        if (inBody) {
            transform.position = human.hidePoint.position;
        }
    }

    private void CheckIsGrounded() {
        isGrounded = Physics.OverlapSphere(transform.position, 1.1f, 1<<LayerMask.NameToLayer("Default")).Length > 0;
    }

    private void CheckStartControl() {
        if (!inBody || 
            human.aiNavigation.currentState == HumanAIStates.ControlledByPlayer ||
            human.aiNavigation.currentState == HumanAIStates.Dead) 
        {
            return;
        }

        if (InputManager.Instance.buttonInputs["Control"].Down) {
            human.humanControlledNavigation.npcCam = npcCam.transform;
            human.aiNavigation.SetState(HumanAIStates.ControlledByPlayer);
        }
    }

    private void Jump() {
        if (inBody && human.infectionLevel.IsDead) {
            Vector3 forward = human.transform.forward;

            ExitBody();
            ApplyForce(minSneezeForce, sneezeAngle, forward);
        }

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

        StartCoroutine(IgnoreCollisions());
        col.excludeLayers = 1 << LayerMask.NameToLayer("Human");
        rb.excludeLayers = 1 << LayerMask.NameToLayer("Human");

        this.human = human;
        Debug.Log("Set");
        lastHuman = human.transform;
        npcCam.Follow = human.transform;
        npcCam.LookAt = human.transform;

        human.infectionLevel.SetHostingVirus(true);

        virusCam.gameObject.SetActive(false);
        npcCam.gameObject.SetActive(true);
    }

    private void ExitBody() {
        rb.useGravity = true;

        if (human.aiNavigation.currentState == HumanAIStates.ControlledByPlayer) {
            human.aiNavigation.SetState(HumanAIStates.Stationary);
        }
        if (!human.infectionLevel.IsDead) {
            StartCoroutine(human.aiNavigation.StationaryTimer());
        }

        human.infectionLevel.SetHostingVirus(false);
        human = null;

        virusCam.gameObject.SetActive(true);
        npcCam.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform == lastHuman || ignoreCollisions) {
            return;
        }

        switch (LayerMask.LayerToName(collision.gameObject.layer)) {
            case "Default":
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                slimeBurst.Play();
                break;
            case "Human":
                HumanManager human = collision.gameObject.GetComponent<HumanManager>();
                if (human.infectionLevel.IsDead) {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    //rb.useGravity = false;
                    slimeBurst.Play();
                } else {
                    EnterBody(human);
                }
                break;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.transform == lastHuman || ignoreCollisions) {
            StartCoroutine(ResetLastHuman());
        }
    }

    private IEnumerator ResetLastHuman() {
        yield return null;
        Debug.Log("Reset");
        lastHuman = null;
        StartCoroutine(IgnoreCollisions());
        col.excludeLayers = 0;
        rb.excludeLayers = 0;
    }

    private IEnumerator IgnoreCollisions() {
        ignoreCollisions = true;
        yield return new WaitForFixedUpdate();
        ignoreCollisions = false;
    }
}