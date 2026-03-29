using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    // Note: Use OnTriggerEnter(Collider other) if you are making a 3D game
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that collided with this trigger is the Player
        if (collision.CompareTag("Player"))
        {
            // Call the method using the Singleton instance
            if (TransitionLevel.instance != null)
            {
                Destroy(collision.gameObject);
                UnlockNewLevel();
                TransitionLevel.instance.FaidInUI();
            }
            else
            {
                Debug.LogWarning("TransitionLevel instance not found in the scene!");
            }
        }

        void UnlockNewLevel()
        {
            if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
            {
                PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
                PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
                PlayerPrefs.Save();
            }
        }
    }
}