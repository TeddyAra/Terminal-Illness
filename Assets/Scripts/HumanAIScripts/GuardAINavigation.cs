using UnityEngine;

public class GuardAINavigation : HumanAINavigation
{
    [SerializeField] private VirusController playerController;
    [SerializeField] private float distanceBeforeChase = 10.0f;
    [SerializeField] private float sprayDistance = 2.0f; 
    private float DistanceToPlayer() {
        return Vector3.Distance(transform.position, playerController.transform.position);
    }

    private void Update() {
        if (playerController.IsInBody()) {
            if (DistanceToPlayer() <=  distanceBeforeChase) {
                SetState(HumanAIStates.ChasingPlayer); 
            }
        }
    }

    public override void ChasePlayer() {
        SetDestination(transform.position);

        if (DistanceToPlayer() <= sprayDistance) {
            
        }
    }
}
