using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || !collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        isDead = true;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}