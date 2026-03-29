using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Xml.XPath;

public class TransitionLevel : MonoBehaviour
{
    public static TransitionLevel instance;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private  GameObject Background;
    [SerializeField] private int secondsToWait = 2;
    [SerializeField] private float fadeDuration = 1.5f;

    public void Awake()
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

    public void FaidInUI()
    {
        Background.SetActive(true);
        group.alpha = 0f;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(secondsToWait);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        group.alpha = 1f;
    }
    public void ContinueButton()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(currentSceneIndex + 1);
    }

    public void Menu()
    {
        SceneManager.LoadScene("Main Menu");
    }


}
