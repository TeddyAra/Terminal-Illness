using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class HumanAINavigation : MonoBehaviour
{
    [SerializeField] private float randomWalkingPointMaxDistance;
    [SerializeField] private float minStationaryTime; 
    [SerializeField] private float maxStationaryTime;
    [SerializeField] private float stoppingDistance;

    public HumanAIStates currentState = HumanAIStates.Wandering;

    NavMeshAgent navMeshAgent;



    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();

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
            Debug.Log(hit.position); 

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
        Debug.Log("SettingStationary"); 
        navMeshAgent.isStopped = true; 
        SetState(HumanAIStates.Stationary);
        float stationaryTime = Random.Range(minStationaryTime, maxStationaryTime); 
        yield return new WaitForSeconds(stationaryTime);
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
                break; 
        }
    }

}
