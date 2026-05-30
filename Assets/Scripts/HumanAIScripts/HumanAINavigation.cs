using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class HumanAINavigation : MonoBehaviour {
    [SerializeField] private float randomWalkingPointMaxDistance;
    [SerializeField] private float minStationaryTime; 
    [SerializeField] private float maxStationaryTime;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float deathBombRadius = 5f;
    [SerializeField] private LayerMask humanLayer; 

    public HumanAIStates currentState = HumanAIStates.Wandering;

    [SerializeField] private NavMeshAgent navMeshAgent;

    private void Awake() {
        SpawnHuman();
    }

    private void Update() {
        HandleHumanStates();
    }

    public void SpawnHuman() {
        gameObject.SetActive(true); 
        transform.position = PickRandomWalkingSpot(); 

        SetState(HumanAIStates.Wandering);

        if (currentState == HumanAIStates.Wandering) {
            PickNextWalkingPoint();
        }
    }

    public void SetDestination(Vector3 destinationPoint) {
        navMeshAgent.SetDestination(destinationPoint);
        navMeshAgent.isStopped = false; 
    }

    private Vector3 PickRandomWalkingSpot() {
        NavMeshHit hit;
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * randomWalkingPointMaxDistance;

        if (NavMesh.SamplePosition(randomPoint, out hit, randomWalkingPointMaxDistance, NavMesh.AllAreas)){ 
            return hit.position;     
        }
        
        return Vector3.zero;
    }

    private void HandleHumanStates() {
        switch (currentState) {
            case HumanAIStates.Wandering:
                if (navMeshAgent.remainingDistance < stoppingDistance) {
                    StartCoroutine(StationaryTimer()); 
                }
                break; 
            case HumanAIStates.Stationary:
                
                break; 
            case HumanAIStates.ControlledByPlayer:

                break; 
            case HumanAIStates.Dead: 
                break;
        }
    }

    private IEnumerator StationaryTimer() {
        navMeshAgent.isStopped = true; 
        SetState(HumanAIStates.Stationary);

        float stationaryTime = Random.Range(minStationaryTime, maxStationaryTime); 
        yield return new WaitForSeconds(stationaryTime);

        if (currentState == HumanAIStates.Dead) {
            yield break;

        }
        PickNextWalkingPoint();
        SetState(HumanAIStates.Wandering);
        navMeshAgent.isStopped = false; 
    }

    public virtual void PickNextWalkingPoint() {
        SetDestination(PickRandomWalkingSpot());
    }

    public void SetState(HumanAIStates newState) {
        currentState = newState;

        switch (currentState) {
            case HumanAIStates.Stationary:
                navMeshAgent.isStopped = true; 
                navMeshAgent.ResetPath();
                break; 
            case HumanAIStates.Wandering: 

                break;
            case HumanAIStates.ControlledByPlayer:
                navMeshAgent.ResetPath();
                break; 
            case HumanAIStates.Dead:
                navMeshAgent.ResetPath(); 
                DeathBomb();
                break; 
        }
    }

    private void DeathBomb() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, deathBombRadius, humanLayer); 
        foreach (var col in colliders) {
            if (col.gameObject == this)
                continue; 

            col.GetComponent<HumanAINavigation>().SetScarePoint(transform.position);
        }
    }

    public void SetScarePoint(Vector3 origin) {
        navMeshAgent.ResetPath(); 
        Vector3 originToTransform = transform.position - origin; 

        Vector3 newWalkPoint = originToTransform * 10; 

        SetState(HumanAIStates.Wandering); 

        SetDestination(newWalkPoint);
    }
}