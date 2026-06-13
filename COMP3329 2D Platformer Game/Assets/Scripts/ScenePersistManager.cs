using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsKeyholderDefeated { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset keyholder defeated state on scene load
        IsKeyholderDefeated = false;
        Debug.Log("Scene loaded, IsKeyholderDefeated reset to false");
    }

    public void SetKeyholderDefeated()
    {
        IsKeyholderDefeated = true;
        Debug.Log("Keyholder defeated! A door opened somewhere...");
        if (PromptManager.Instance != null)
        {
            PromptManager.Instance.ShowKeyholderDefeatedPrompt();
        }
        else
        {
            Debug.LogError("PromptManager instance is null!");
        }
    }
}