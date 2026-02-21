using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinManager : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            GoToMainMenu();
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
