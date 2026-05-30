using UnityEngine;

public class MedicAINavigation : HumanAINavigation
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
                if (playerController.human?.humanInfectionLevel.CurrentInfectionLevel >= 50) {
                    SetState(HumanAIStates.ChasingPlayer); 
                }
                
            }
        }
    }

    public override void ChasePlayer() {
        SetDestination(transform.position);

        if (DistanceToPlayer() <= sprayDistance) {
            playerController.ExitBody(); 
            SetState(HumanAIStates.Wandering);
            playerController.human.humanInfectionLevel.CurrentInfectionLevel = 0f; 
        }
    }
}
