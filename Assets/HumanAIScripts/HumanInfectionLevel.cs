using UnityEngine;

public class HumanInfectionLevel : MonoBehaviour
{
    [SerializeField] private float maxInfection = 100f;
    [SerializeField] private float minInfection = 0f;
    [SerializeField] private float infectionRate = 1f;
    [SerializeField] private MeshRenderer humanRenderer;
    HumanAINavigation humanAINavigation; 

    private MaterialPropertyBlock humanPropertyBlock;

    public bool isInfected = false; 

    public float CurrentInfectionLevel {
        get {
            return currentInfectionLevel;
        }
        set {
            currentInfectionLevel = value; 

            currentInfectionLevel = Mathf.Clamp(currentInfectionLevel, minInfection, maxInfection);

            humanPropertyBlock.SetFloat("_InfectionRate", currentInfectionLevel / maxInfection); 

            humanRenderer.SetPropertyBlock(humanPropertyBlock);

            if (currentInfectionLevel >= maxInfection)
                IsDead = true;
        }
    }
    public bool IsHostingVirus { get; private set; } = false;
    public bool IsDead {
        get {
            return IsDead;
        }
        set {
            isDead = value; 
            humanAINavigation.SetState(HumanAIStates.Dead);
        }
    }

    private bool isDead = false;
    private float currentInfectionLevel;

    private void Awake() {
        humanPropertyBlock = new MaterialPropertyBlock();
        humanAINavigation = GetComponent<HumanAINavigation>(); 
    }

    private void Update() {
        IsHostingVirus = isInfected; 

        if (IsHostingVirus) {
            CurrentInfectionLevel += infectionRate * Time.deltaTime; 
            Debug.Log(CurrentInfectionLevel);
        }
    }
   
    public void SetHostingVirus(bool isHostingVirus) {
        IsHostingVirus = isHostingVirus;
    }


}
