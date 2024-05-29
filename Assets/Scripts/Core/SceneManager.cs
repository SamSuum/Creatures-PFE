using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public void ExitToDesktop()
    {
        Application.Quit();
    }
    public void ExitToMainMenu()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void LoadScene (int index)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }
}
