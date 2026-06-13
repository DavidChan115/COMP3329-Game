using UnityEngine;
using UnityEngine.SceneManagement;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }
    [SerializeField] private GameObject keyholderDefeatedPrompt; // Assigned in Inspector as a fallback

    private bool isPromptShown = false; // Track if the prompt is currently shown

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene reloads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        // Reinitialize on every scene load or awake
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializePrompt();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset state and reinitialize prompt when a new scene loads
        isPromptShown = false;
        if (keyholderDefeatedPrompt != null)
        {
            keyholderDefeatedPrompt.SetActive(false);
        }
        InitializePrompt();
    }

    private void InitializePrompt()
    {
        // Try to use the assigned reference first
        if (keyholderDefeatedPrompt != null && keyholderDefeatedPrompt.scene == SceneManager.GetActiveScene())
        {
            keyholderDefeatedPrompt.SetActive(false);
            Debug.Log("Prompt initialized and set to inactive: " + keyholderDefeatedPrompt.name);
        }
        else
        {
            // Attempt to find the prompt in the current scene
            keyholderDefeatedPrompt = GameObject.Find("KeyholderDefeatedPanel"); // Match your panel name
            if (keyholderDefeatedPrompt != null)
            {
                keyholderDefeatedPrompt.SetActive(false);
                Debug.Log("Prompt found and initialized: " + keyholderDefeatedPrompt.name);
            }
            else
            {
                Debug.LogError("keyholderDefeatedPrompt not found in the scene!");
            }
        }
    }

    public void ShowKeyholderDefeatedPrompt()
    {
        if (keyholderDefeatedPrompt != null && !isPromptShown)
        {
            keyholderDefeatedPrompt.SetActive(true);
            isPromptShown = true;
            Debug.Log("Prompt activated: " + keyholderDefeatedPrompt.name);
            StartCoroutine(HidePromptAfterDelay(3f));
        }
        else if (isPromptShown)
        {
            Debug.LogWarning("Prompt is already shown, skipping activation.");
        }
        else
        {
            Debug.LogError("Cannot show prompt: keyholderDefeatedPrompt is null!");
            InitializePrompt(); // Attempt to recover
        }
    }

    private System.Collections.IEnumerator HidePromptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (keyholderDefeatedPrompt != null && isPromptShown)
        {
            keyholderDefeatedPrompt.SetActive(false);
            isPromptShown = false;
            Debug.Log("Prompt deactivated: " + keyholderDefeatedPrompt.name);
        }
    }
}