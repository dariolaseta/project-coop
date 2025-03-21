using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerNameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField playerNameInputField;
    
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    [SerializeField] private List<Button> mainMenuButtons = new List<Button>();
    
    [SerializeField] private TMP_Text errorText;
    
    private string playerName;
    private const string PLAYER_PREFS_PLAYER_NAME = "PlayerName";
    
    private bool isPlayerNameSet = false;
    
    public bool IsPlayerNameSet => isPlayerNameSet;

    private void Start()
    {
        isPlayerNameSet = PlayerPrefs.HasKey(PLAYER_PREFS_PLAYER_NAME);
        
        confirmButton.onClick.AddListener(SetPlayerName);
        cancelButton.onClick.AddListener(Hide);
        
        CheckForPlayerName();
    }

    private void OnEnable()
    {
        CheckForPlayerName();
    }

    private void CheckForPlayerName()
    {
        errorText.gameObject.SetActive(false);
        
        bool hasName = PlayerPrefs.HasKey(PLAYER_PREFS_PLAYER_NAME);
        isPlayerNameSet = hasName;

        if (hasName)
        {
            string savedName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME);
            playerNameInputField.text = savedName;
        }
        else
        {
            playerNameInputField.text = "";
        }
    }
    
    private void SetPlayerName()
    {
        playerName = playerNameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            errorText.gameObject.SetActive(true);
            return;
        }
        
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, playerName);
        PlayerPrefs.Save();
        
        isPlayerNameSet = true;
        cancelButton.interactable = true;
        
        Hide();
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        
        if (!isPlayerNameSet)
            cancelButton.interactable = false;

        foreach (var button in mainMenuButtons)
        {
            button.interactable = false;
        }
    }

    public void Hide()
    {
        foreach (var button in mainMenuButtons)
        {
            button.interactable = true;
        }
        
        gameObject.SetActive(false);
    }
}
