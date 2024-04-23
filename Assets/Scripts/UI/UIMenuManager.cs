using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject gameOverUI;
   
    [SerializeField] Actor player;
    [SerializeField] PlayerInputs input;


    [SerializeField] private Inventory _playerInventory;

    private void Start()
    {       
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);

        Cursor.visible = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (player.dead)
        {
            ToggleMenu(true, gameOverUI);
            Time.timeScale =.5f;
        }
        else if (input.pause)
        {
            ToggleMenu(!pauseUI.activeSelf, pauseUI);                
        }

        if(input.pause) input.pause = false;
    }

    public void ToggleMenu(bool enabled, GameObject UI)
    {
        UI.SetActive(enabled);
        HUD.SetActive(!enabled);
        Cursor.visible = enabled;
        Cursor.lockState = enabled? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = enabled ? 0 : 1;
        input.cursorInputForLook = !enabled;
    }

    public void ResumeGame()
    {
        ToggleMenu(false, pauseUI);
    }

  

   
}
