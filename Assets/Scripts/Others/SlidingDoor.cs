using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private float openingSpeed = 2f;
    [SerializeField] private GameObject doorPivot; 
    private Vector3 openPosition; 
    private Vector3 closedPosition;
    private List<GameObject> securityGuardsByDoor = new List<GameObject>(); 

    private void Awake() {
        openPosition = transform.position;
        closedPosition = doorPivot.transform.position;
    }

    void Update()
    {
        Vector3 targetPosition; 
        if (securityGuardsByDoor.Count != 0) {
            targetPosition = openPosition; 
        }
        else {
            targetPosition = closedPosition;
        }

        if (doorPivot.transform.position != targetPosition) {
            doorPivot.transform.position = Vector3.Lerp(doorPivot.transform.position, targetPosition, openingSpeed * Time.deltaTime);
        }
        
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Security")) {
            securityGuardsByDoor.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Security")) {
            securityGuardsByDoor.Remove(other.gameObject);
        }
    }
}
