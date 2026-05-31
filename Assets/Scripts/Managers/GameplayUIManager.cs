using UnityEngine;

public class GameplayUIManager : MonoBehaviour {
    [SerializeField] private GameObject ui = null;
    [SerializeField] private GameObject pauseMenu = null;

    [SerializeField] private GameObject virusActions = null;
    [SerializeField] private GameObject humanActions = null;

    private bool pauseToggle = true;

    public static GameplayUIManager Instance = null;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() {
        Time.timeScale = 0f;
    }

    private void Update() {
        if (InputManager.Instance.buttonInputs["Pause"].Down) {
            pauseToggle = !pauseToggle;

            ui.SetActive(!pauseToggle);
            pauseMenu.SetActive(pauseToggle);

            Time.timeScale = pauseToggle ? 0f : 1f;

            InputManager.Instance.TogglePause(pauseToggle);
        }
    }

    public void ToggleActions(bool virus) { 
        virusActions.SetActive(virus);
        humanActions.SetActive(!virus);
    }
}