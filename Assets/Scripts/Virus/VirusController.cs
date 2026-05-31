using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VirusController : MonoBehaviour {
    [Header("Jump")]
    [SerializeField] private float maxHoldLength = 1f;
    [SerializeField] private float minJumpForce = 1f;
    [SerializeField] private float maxJumpForce = 5f;
    [SerializeField] private float jumpAngle = 45f;

    [SerializeField] private float minSneezeForce = 30f;
    [SerializeField] private float maxSneezeForce = 45f;
    [SerializeField] private float sneezeAngle = 30f;

    [Header("Survive")]
    [SerializeField] private Slider surviveSlider = null;
    [SerializeField] private float surviveTime = 5f;
    [SerializeField] private float surviveDepletionRate = 1f;
    [SerializeField] private float surviveAdditionRate = 1.5f;

    [Header("References")]
    [SerializeField] private CinemachineCamera virusCam = null;
    [SerializeField] private CinemachineCamera npcCam = null;

    [SerializeField] private ParticleSystem slimeBurst = null;

    [SerializeField] private Animator animator; 

    private bool inBody => human != null;
    private float jumpTimer = 0f;
    private bool isGrounded = false;
    private float surviveTimer = 0f;
    private bool prevIsGrounded = false; 

    private Collider col = null;
    private Rigidbody rb = null;
    public HumanManager human = null;
    private Transform lastHuman = null;

    public AudioClip[] landSplat;
    public AudioClip takeHuman;
    public AudioClip takeoff;
    private AudioSource audioSource;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        surviveTimer = surviveTime;
    }

    private void Update() {
        CheckIsGrounded();
        CheckStartControl();
        Jump();
    }

    private void LateUpdate() {
        if (inBody) {
            transform.position = human.hidePoint.position;
            surviveTimer += Time.deltaTime * surviveAdditionRate;
        } else {
            surviveTimer -= Time.deltaTime * surviveDepletionRate;
            if (surviveTimer <= 0f) {
                SceneManager.LoadScene("EndScreen");
            }
        }

        surviveSlider.value = surviveTimer / surviveTime;

        if (isGrounded) {
            if (!prevIsGrounded) {
                animator.SetBool("IsGrounded", true);

                int randomIndex = Random.Range(0, landSplat.Length);
                audioSource.PlayOneShot(landSplat[randomIndex]);
            }

            prevIsGrounded = true; 
        }
        else {
            animator.SetBool("IsGrounded", false); 
            prevIsGrounded = false; 
            
            Vector3 normalizedSpeed = rb.linearVelocity.normalized * 3;

            transform.rotation = Quaternion.LookRotation(Vector3.up, normalizedSpeed); 
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
            animator.SetBool("ChargingJump", true); 
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
                    StatManager.Instance.IncreaseJumps();
                    audioSource.PlayOneShot(takeoff);
                }
            } else {
                Vector3 forward = human.transform.forward;

                ExitBody();

                float force = Mathf.Lerp(minSneezeForce, maxSneezeForce, Mathf.Clamp01(jumpTimer / maxHoldLength));
                ApplyForce(force, sneezeAngle, forward);
                StatManager.Instance.IncreaseJumps();
            }

            animator.SetBool("ChargingJump", false); 
            animator.SetTrigger("Jump"); 
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

        this.human = human;
        npcCam.Follow = human.transform;
        npcCam.LookAt = human.transform;
         
        lastHuman = human.transform;

        human.infectionLevel.SetHostingVirus(true);

        GameplayUIManager.Instance.ToggleActions(false);

        virusCam.gameObject.SetActive(false);
        npcCam.gameObject.SetActive(true);

        if (human.infectionLevel.CurrentInfectionLevel != 0) {
            StatManager.Instance.IncreaseInfections();
        }

        audioSource.PlayOneShot(takeHuman);
    }

    public void ExitBody() {
        rb.useGravity = true;

        if (human.aiNavigation.currentState == HumanAIStates.ControlledByPlayer) {
            human.aiNavigation.SetState(HumanAIStates.Stationary);
        }
        if (!human.infectionLevel.IsDead) {
            StartCoroutine(human.aiNavigation.StationaryTimer());
        }

        human.infectionLevel.SetHostingVirus(false);
        human = null;

        GameplayUIManager.Instance.ToggleActions(true);

        virusCam.gameObject.SetActive(true);
        npcCam.gameObject.SetActive(false);

        audioSource.PlayOneShot(takeoff);
    }

    public void ExitBodyBackwards() {
        rb.useGravity = true;

        Vector3 backward = -human.transform.forward;

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

        ApplyForce(maxJumpForce, sneezeAngle, backward); 
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision");
        lastHuman = null;
        transform.rotation = Quaternion.FromToRotation(transform.up, collision.contacts[0].normal); 

        switch (LayerMask.LayerToName(collision.gameObject.layer)) {
            case "Default":
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                slimeBurst.Play();
                break;
            case "NonStickable":
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                slimeBurst.Play();
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (lastHuman != null && other.transform.parent != null && other.transform.parent == lastHuman) {
            return;
        }

        switch (LayerMask.LayerToName(other.gameObject.layer)) {
            case "HumanTrigger":
                HumanManager human = other.transform.parent.GetComponent<HumanManager>();
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

    public bool IsInBody() {
        return inBody;
    }
}