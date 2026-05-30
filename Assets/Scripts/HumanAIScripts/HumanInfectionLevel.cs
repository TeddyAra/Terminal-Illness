using System;
using UnityEngine;

public class HumanInfectionLevel : MonoBehaviour {
    [SerializeField] private float maxInfection = 100f;
    [SerializeField] private float minInfection = 0f;
    [SerializeField] private float normalInfectionRate = 1f;
    [SerializeField] private float controlledInfectionRate = 2f;
    [SerializeField] private MeshRenderer humanRenderer;

    [SerializeField] private HumanManager manager = null;

    [SerializeField] private GameObject particlesParent; 

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
            isDead = value;
            aiNavigation.SetState(HumanAIStates.Dead);
            gameObject.layer = LayerMask.NameToLayer("Default");
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
            CurrentInfectionLevel += infectionRate * Time.deltaTime; 
        }
    }

    public void SetHostingVirus(bool isHostingVirus) {
        IsHostingVirus = isHostingVirus;
    }
    
    private void UpdateParticles(float infectionRate) {
        ParticleSystem[] particles = particlesParent.GetComponentsInChildren<ParticleSystem>();

        Debug.Log($"Number of particles systems: {particles.Length}"); 

        foreach (ParticleSystem particle in particles) {
            var emission = particle.emission; 

            emission.rateOverTime = 5 * (infectionRate/maxInfection);          
            
        }
    }
}