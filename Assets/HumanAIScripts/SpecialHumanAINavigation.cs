using System.Collections.Generic;
using UnityEngine;

public class SpecialHumanAINavigation : HumanAINavigation
{
    [SerializeField] private List<Transform> pointsToWalkTo = new List<Transform>();
    private int currentWalkingPointIndex = 0;


    public override void PickNextWalkingPoint() {
        SetDestination(pointsToWalkTo[currentWalkingPointIndex].position); 
        currentWalkingPointIndex++; 
        currentWalkingPointIndex %= pointsToWalkTo.Count;

    }
}
