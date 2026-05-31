using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private GameObject sprayParticles;
    [SerializeField] private VirusController controller;

    private void OnTriggerEnter(Collider other) {
        HumanAINavigation human = other.GetComponent<HumanAINavigation>(); 

        if (human.GetComponent<HumanInfectionLevel>().IsHostingVirus) {
            StartCoroutine(SprayCoroutine()); 
        }
    }

    private IEnumerator SprayCoroutine() {
        sprayParticles.SetActive(true); 
        yield return new WaitForSeconds(0.5f); 
        controller.ExitBody(); 
        yield return new WaitForSeconds(2f); 
        sprayParticles.SetActive(false); 

    }
}
