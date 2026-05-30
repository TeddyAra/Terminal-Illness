using UnityEngine;

public class StatManager : MonoBehaviour {
    private bool winStatus = false;
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

    public void ToggleWinStatus() {
        winStatus = true;
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

    public bool GetWinStatus() {
        return winStatus;
    }

    private void Update() { 
        time += Time.deltaTime;
    }

    public void GetTime(out int minutes, out float seconds) {
        minutes = (int)Mathf.Floor(time / 60f);
        seconds = (int)Mathf.Floor(time % 60f);
    }

    public void ResetValues() {
        winStatus = false;
        time = 0f;
        jumps = 0;
        infections = 0;
        kills = 0;
    }
}