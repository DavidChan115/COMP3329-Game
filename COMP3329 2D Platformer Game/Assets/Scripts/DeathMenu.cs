using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // Ensure this is included for IEnumerator

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuCanvas;
    public Button restartButton;
    public Button exitButton;
    private Damageable playerDamageable;
    private float deathAnimationDuration = 1f; // Set this to match your death animation length

    private void Awake()
    {
        // Find the player and get its Damageable component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No Player found in the scene. Make sure it has tag 'Player'");
            return;
        }
        playerDamageable = player.GetComponent<Damageable>();
        if (playerDamageable == null)
        {
            Debug.LogError("Player does not have a Damageable component.");
        }
    }

    private void Start()
    {
        // Ensure the death menu is hidden at the start
        if (deathMenuCanvas != null)
        {
            deathMenuCanvas.SetActive(false);
        }

        // Add listeners to buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    private void OnEnable()
    {
        // Subscribe to the player's OnDeath event
        if (playerDamageable != null)
        {
            playerDamageable.OnDeath.AddListener(OnPlayerDeath);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnDeath event
        if (playerDamageable != null)
        {
            playerDamageable.OnDeath.RemoveListener(OnPlayerDeath);
        }
    }

    private void OnPlayerDeath()
    {
        // Start coroutine to wait for animation before showing menu
        StartCoroutine(ShowDeathMenu());
    }

    private IEnumerator ShowDeathMenu()
    {
        // Wait for the death animation to complete using unscaled time
        yield return new WaitForSecondsRealtime(deathAnimationDuration);

        // Freeze the game
        Time.timeScale = 0f;

        // Show the death menu
        if (deathMenuCanvas != null)
        {
            deathMenuCanvas.SetActive(true);
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ExitGame()
    {
        Time.timeScale = 1f; // Resume game time (optional)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
}