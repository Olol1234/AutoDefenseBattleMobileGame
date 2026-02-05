using UnityEngine;
using UnityEngine.SceneManagement;

public class StageEnd : MonoBehaviour
{
    public void RetryStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
