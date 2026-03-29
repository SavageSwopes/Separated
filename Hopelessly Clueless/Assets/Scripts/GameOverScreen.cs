using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen instance;

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject background;
    [SerializeField] private TMP_Text gameOverText;

    [Header("Fade Settings")]
    [SerializeField] private float delayBeforeFade = 1f;
    [SerializeField] private float fadeDuration = 1f;
    private int currentSceneIndex;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (background.activeInHierarchy)
        {
            background.SetActive(false);
        }
        canvasGroup.alpha = 0f;

        // NEW: Turn off button interactions while the screen is hidden
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void TriggerDeathScreen()
    {
        background.SetActive(true);
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }

        StartCoroutine(WaitAndFade());
    }

    private IEnumerator WaitAndFade()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // NEW: Turn on button interactions now that the screen is fully visible
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void RestartLevelButton()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}