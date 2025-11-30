using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Transition")]
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private string fadeOutTrigger = "FadeToBlack";
    [SerializeField] private string fadeInTrigger = "FadeIn";
    [SerializeField] private float transitionTime = 1f;

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

    // Private coroutine that handles the fade
    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        // Fade out
        transitionAnim.SetTrigger(fadeOutTrigger);
        yield return new WaitForSeconds(transitionTime);

        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone) yield return null;

        // Fade in
        transitionAnim.SetTrigger(fadeInTrigger);
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        transitionAnim.SetTrigger(fadeOutTrigger);
        yield return new WaitForSeconds(transitionTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;

        transitionAnim.SetTrigger(fadeInTrigger);
    }
}