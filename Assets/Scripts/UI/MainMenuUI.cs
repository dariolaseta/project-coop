using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button changeNameButton;
    
    [SerializeField] private GameObject playerNamePanel;
    
    [SerializeField] private ChangePlayerNameUI changePlayerNameUI;
    
    //DEBUG
    [SerializeField] private TMP_Text buildText;
    private const string BUILD_VER = "Pre Alpha v. ";

    private void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        optionsButton.onClick.AddListener(Options);
        quitButton.onClick.AddListener(QuitToDesktop);
        changeNameButton.onClick.AddListener(changePlayerNameUI.Show);
        
        if (!changePlayerNameUI.IsPlayerNameSet)
            changePlayerNameUI.Show();
        else
            changePlayerNameUI.Hide();
        
        buildText.text = BUILD_VER + Application.version;
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(1); //TODO ENUM
    }

    private void Options()
    {
        Debug.Log("Options");
    }

    private void QuitToDesktop()
    {
        Debug.Log("Quit...");
        Application.Quit();
    }
}
