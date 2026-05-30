using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUIManager : MonoBehaviour {
    [SerializeField] private TMP_Text statusText = null;
    [SerializeField] private TMP_Text timeText = null;
    [SerializeField] private TMP_Text jumpsText = null;
    [SerializeField] private TMP_Text infectionsText = null;
    [SerializeField] private TMP_Text killsText = null;

    private void Start() {
        statusText.text = $"You {(StatManager.Instance.GetWinStatus() ? "won" : "lost")}!";

        int minutes = 0;
        float seconds = 0;
        StatManager.Instance.GetTime(out minutes, out seconds);
        string secondsString = (seconds < 10 ? "0" : "") + seconds.ToString();
        timeText.text = $"Time: {minutes}:{secondsString}";

        jumpsText.text = $"Jumps: {StatManager.Instance.GetJumps()}";
        infectionsText.text = $"Infections: {StatManager.Instance.GetInfections()}";
        killsText.text = $"Kills: {StatManager.Instance.GetKills()}";
    }

    public void Restart() {
        StatManager.Instance.ResetValues();
        SceneManager.LoadScene("Level");
    }
}