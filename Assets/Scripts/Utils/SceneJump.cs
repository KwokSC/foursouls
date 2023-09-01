using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    public void JumpToStart() {
        SceneManager.LoadScene(0);
    }

    public void JumpToMenu() {
        SceneManager.LoadScene(1);
    }

    public void JumpToCreate()
    {
        SceneManager.LoadScene(2);
    }

    public void JumpToJoin() {
        SceneManager.LoadScene(3);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
