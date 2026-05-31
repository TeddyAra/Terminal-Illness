using UnityEngine;

public class GameplayUIManager : MonoBehaviour {
    [SerializeField] private GameObject slider = null;
    [SerializeField] private GameObject pauseMenu = null;

    private bool pauseToggle = true;

    private void Start() {
        Time.timeScale = 0f;
    }

    private void Update() {
        if (InputManager.Instance.buttonInputs["Pause"].Down) {
            pauseToggle = !pauseToggle;

            slider.SetActive(!pauseToggle);
            pauseMenu.SetActive(pauseToggle);

            Time.timeScale = pauseToggle ? 0f : 1f;

            InputManager.Instance.TogglePause(pauseToggle);
        }
    }
}