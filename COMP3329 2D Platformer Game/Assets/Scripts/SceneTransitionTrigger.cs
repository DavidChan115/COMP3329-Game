using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "BossScene"; // Set in Inspector
    [SerializeField] private GameObject promptCanvas; // Optional UI prompt

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.CompareTag("Player"))
        {
            // Check if the keyholder has been defeated
            if (GameManager.Instance.IsKeyholderDefeated)
            {
                // Load the boss scene
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                // Show a prompt if the keyholder is not defeated
                if (promptCanvas != null)
                {
                    promptCanvas.SetActive(true);
                    StartCoroutine(HidePromptAfterDelay(2f));
                }
                Debug.Log("Defeat the keyholder to unlock the door!");
            }
        }
    }

    private System.Collections.IEnumerator HidePromptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }
}