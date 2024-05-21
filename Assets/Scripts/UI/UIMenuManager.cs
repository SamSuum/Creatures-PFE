using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject gameOverUI;

    [SerializeField] GameObject Main;
    [SerializeField] GameObject Settings;
    [SerializeField] GameObject Audio;
    [SerializeField] GameObject Help;

    [SerializeField] Actor player;
    [SerializeField] PlayerInputs input;


    [SerializeField] private Inventory _playerInventory;

    private void Start()
    {       
        if(pauseUI) pauseUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if(player)
        {
            Cursor.visible = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(player != null && input != null)
        {
            if (player.dead)
            {
                ToggleMenu(true, gameOverUI);
                Time.timeScale = .5f;
            }
            else if (input.pause)
            {
                ToggleMenu(!pauseUI.activeSelf, pauseUI);
            }

            if (input.pause) input.pause = false;
        }
        
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

    public void OpenSettings()
    {
        Main.SetActive(false);
        Settings.SetActive(true);
    }
    public void OpenHelp()
    {
        Main.SetActive(false);
        Help.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        Audio.SetActive(true);
    }

    public void Back()
    {
        Main.SetActive(true);
        Settings.SetActive(false);
        Help.SetActive(false);
    }



}
