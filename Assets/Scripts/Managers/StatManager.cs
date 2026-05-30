using UnityEngine;

public class StatManager : MonoBehaviour {
    private int jumps = 0;
    private int kills = 0;
    private int infections = 0;
    private float time = 0f;

    public static StatManager Instance = null;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncreaseJumps() {
        jumps++;
    }

    public void IncreaseKills() {
        kills++;
    }

    public void IncreaseInfections() {
        infections++;
    }

    public int GetJumps() {
        return jumps;
    }

    public int GetKills() {
        return kills;
    }

    public int GetInfections() {
        return infections;
    }

    private void Update() { 
        time += Time.deltaTime;
    }

    public void GetTime(out int minutes, out float seconds) {
        Debug.Log(time);
        minutes = (int)Mathf.Floor(time / 60f);
        seconds = time % 60f;
        Debug.Log(seconds);
    }

    public void ResetValues() {
        time = 0f;
        jumps = 0;
        infections = 0;
        kills = 0;
    }
}