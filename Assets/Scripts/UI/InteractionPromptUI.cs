using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private TextMeshProUGUI _promptText; 

    private void Start()
    {        
        _uiPanel.SetActive(false);
    }
   
    public bool isDisplayed = false;

    public void SetUp(string promptText)
    {
        _uiPanel.SetActive(true);
        _promptText.text = promptText;
        isDisplayed = true;
    }

    public void Close()
    {
        _uiPanel.SetActive(false);
        isDisplayed = false;
    }
}
