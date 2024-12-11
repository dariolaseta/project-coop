using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button audioSettingsButton;
    [SerializeField] private Button videoSettingsButton;
    [SerializeField] private Button gameSettingsButton;
    [SerializeField] private Button backButton;
    
    [SerializeField] private GameObject audioSettingsGameObject;
    [SerializeField] private GameObject videoSettingsGameObject;
    [SerializeField] private GameObject gameSettingsGameObject;
    [SerializeField] private GameObject pauseMenuGameObject;

    private void Awake()
    {
        audioSettingsButton.onClick.AddListener(ShowAudioSettings);
        videoSettingsButton.onClick.AddListener(ShowVideoSettings);
        gameSettingsButton.onClick.AddListener(ShowGameSettings);
        backButton.onClick.AddListener(GoBack);
    }

    private void GoBack()
    {
        pauseMenuGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ShowAudioSettings()
    {
        if (audioSettingsGameObject.activeSelf) return;
        
        audioSettingsGameObject.SetActive(true);
        videoSettingsGameObject.SetActive(false);
        gameSettingsGameObject.SetActive(false);
    }

    private void ShowVideoSettings()
    {
        if (videoSettingsGameObject.activeSelf) return;
        
        videoSettingsGameObject.SetActive(true);
        audioSettingsGameObject.SetActive(false);
        gameSettingsGameObject.SetActive(false);
    }

    private void ShowGameSettings()
    {
        if (gameSettingsGameObject.activeSelf) return;
        
        gameSettingsGameObject.SetActive(true);
        audioSettingsGameObject.SetActive(false);
        videoSettingsGameObject.SetActive(false);
    }
}
