using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;

    [Header("UI References")]
    [Tooltip("Drag the Background GameObject containing your buttons here.")]
    [SerializeField] private GameObject gameOverBackground;

    private void Awake()
    {
        // Set up the singleton so the player script can call it easily
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Ensure the game over screen is completely hidden when the game starts
        if (gameOverBackground != null)
        {
            gameOverBackground.SetActive(false);
        }
    }

    /// <summary>
    /// Call this from your player's death script: SimpleGameOverScreen.instance.ShowDeathScreen();
    /// </summary>
    public void ShowDeathScreen()
    {
        if (gameOverBackground != null)
        {
            gameOverBackground.SetActive(true);
        }

        // Optional: If you want to freeze the game when they die, uncomment the line below
        // Time.timeScale = 0f; 
    }

    public void RestartLevelButton()
    {
        // Always reset time scale to normal before loading a scene, just in case!
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Assumes your main menu is at build index 0
    }
}
