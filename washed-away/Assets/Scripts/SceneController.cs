using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Required for Image

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Simple Fade Transition")]
    [SerializeField] private Image fadeImage;  // Full-screen black UI Image (alpha 0 initially)
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        // Improved singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize fade image (ensure starts invisible)
        InitializeFadeImage();
    }

    private void InitializeFadeImage()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);  // Keep active but invisible
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    // Load next scene in build order
    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        LoadLevel(nextIndex);
    }

    // Load scene by build index
    public void LoadLevel(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Scene index {sceneIndex} is out of range!");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    // Load scene by name (most flexible!)
    public void LoadLevel(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).IsValid() || 
            Application.CanStreamedLevelBeLoaded(sceneName))
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings!");
        }
    }

    // Restart current scene
    public void RestartLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    // Unified coroutine: Works for both int and string
    private IEnumerator LoadSceneRoutine(object sceneIdentifier)
    {
        // Step 1: Fade OUT to black
        yield return StartCoroutine(FadeToBlack());

        // Step 2: Load scene asynchronously
        AsyncOperation asyncLoad;
        if (sceneIdentifier is int sceneIndex)
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        }
        else if (sceneIdentifier is string sceneName)
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        }
        else
        {
            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Step 3: Fade IN from black
        yield return StartCoroutine(FadeFromBlack());
    }

    // Fade to black (alpha 0 -> 1)
    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;  // Unscaled: Works even if time paused
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    // Fade from black (alpha 1 -> 0)
    private IEnumerator FadeFromBlack()
    {
        if (fadeImage == null) yield break;

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }
}