using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HumanInfectionLevel : MonoBehaviour {
    [SerializeField] private float maxInfection = 100f;
    [SerializeField] private float minInfection = 0f;
    [SerializeField] private float normalInfectionRate = 1f;
    [SerializeField] private float controlledInfectionRate = 2f;
    [SerializeField] private float controlledSprintingInfectionRate = 2.5f; 
    [SerializeField] private SkinnedMeshRenderer humanRenderer;

    [SerializeField] private HumanManager manager = null;

    [SerializeField] private GameObject particlesParent;
    [SerializeField] private GameObject humanRagdollPrefab;
    [SerializeField] private GameObject[] particleObjects; 

    

    private HumanAINavigation aiNavigation = null;

    private MaterialPropertyBlock humanPropertyBlock;

    private float currentInfectionLevel;
    public float CurrentInfectionLevel {
        get {
            return currentInfectionLevel;
        }
        set {
            currentInfectionLevel = value; 

            currentInfectionLevel = Mathf.Clamp(currentInfectionLevel, minInfection, maxInfection);

            humanPropertyBlock.SetFloat("_InfectionRate", currentInfectionLevel / maxInfection); 
            UpdateParticles(currentInfectionLevel);

            humanRenderer.SetPropertyBlock(humanPropertyBlock);

            if (currentInfectionLevel >= maxInfection && !IsDead) {
                IsDead = true;
            }
        }
    }

    public bool IsHostingVirus { get; private set; } = false;

    public bool IsDead {
        get {
            return isDead;
        }
        set {
            StatManager.Instance.IncreaseKills();
            if (TryGetComponent(out TargetAIHuman s)) {
                StatManager.Instance.ToggleWinStatus();
                SceneManager.LoadScene("EndScreen");
                return;
            }

            StartCoroutine(SpawnRagdollDelayed()); 

            isDead = value;
            aiNavigation.SetState(HumanAIStates.Dead);
            //gameObject.layer = LayerMask.NameToLayer("Default");
            manager.humanControlledNavigation.rb.isKinematic = true;


        }
    }

    private bool isDead = false;

    private void Awake() {
        humanPropertyBlock = new MaterialPropertyBlock();
        aiNavigation = manager.aiNavigation;
    }

    private void Update() {
        if (IsHostingVirus) {
            float infectionRate = aiNavigation.currentState == HumanAIStates.ControlledByPlayer ? controlledInfectionRate : normalInfectionRate;
            if (manager.humanControlledNavigation.sprintInput) {
                infectionRate = controlledSprintingInfectionRate; 
            }
            CurrentInfectionLevel += infectionRate * Time.deltaTime; 
        }
    }

    private IEnumerator SpawnRagdollDelayed() {
        yield return new WaitForSeconds(0.5f); 
        Instantiate(humanRagdollPrefab, transform.position, humanRagdollPrefab.transform.rotation); 

        gameObject.SetActive(false);
    }

    public void SetHostingVirus(bool isHostingVirus) {
        IsHostingVirus = isHostingVirus;
    }
    
    private void UpdateParticles(float infectionRate) {
        if (manager.infectionLevel.currentInfectionLevel >= 25.0f) {
            particleObjects[0].SetActive(true);
            particleObjects[1].SetActive(true);
        }
        if (manager.infectionLevel.currentInfectionLevel >= 50.0f) {
            particleObjects[2].SetActive(true);
            particleObjects[3].SetActive(true);
        }
        if (manager.infectionLevel.currentInfectionLevel >= 75.0f) {
            particleObjects[4].SetActive(true);
            particleObjects[5].SetActive(true);
        }
        
    }
}