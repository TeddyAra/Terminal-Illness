using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    [SerializeField] private GameObject humanPrefab;
    [SerializeField] private int numberOfHumansToSpawn = 10;

    private void Awake() {
        Initialize(numberOfHumansToSpawn); 
    }

    public void Initialize(int numberOfHumans) {
        for (int i = 0; i < numberOfHumans; i++) {
            Instantiate(humanPrefab, new Vector3(Random.Range(-500f, 500f), 50, Random.Range(-500f, 500f)), Quaternion.identity);
        }
    }

}
