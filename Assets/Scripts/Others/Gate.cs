using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private GameObject sprayParticles;
    [SerializeField] private VirusController controller;
    [SerializeField] private BoxCollider gateBlock; 

    private float timer;


    private void Update() {
        timer += Time.deltaTime; 
    }
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out HumanAINavigation h)) {
            if (h.currentState != HumanAIStates.ControlledByPlayer) {
                return;
            }
        }
        if (timer < 2f) {
            return;
        }

        StartCoroutine(SprayCoroutine()); 
    }

    private IEnumerator SprayCoroutine() {
        timer = 0f; 
        if (controller.IsInBody()) {
            Debug.Log($"In body: {controller.IsInBody()}"); 

            sprayParticles.SetActive(true); 
            gateBlock.enabled = false;
            yield return new WaitForSeconds(0.5f); 
            controller.ExitBodyBackwards(); 
            yield return new WaitForSeconds(2f); 
            sprayParticles.SetActive(false); 
            gateBlock.enabled = true;
        }   
    }
}
