using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitGame : MonoBehaviour
{
    [SerializeField] private int sceneToLoad;
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn)
            {
                string username = PlayerPrefs.GetString("Username");
                if (username == "")
                {
                    username = "Player";
                    PlayerPrefs.SetString("Username", username);
                }
                
                SceneManager.LoadSceneAsync(sceneToLoad);
            }
        }
    }

    private void OnDestroy()
    {
        AuthenticationService.Instance.SignedIn -= OnSignedIn;
    }

    private void OnSignedIn()
    {
        Debug.Log($"Token: {AuthenticationService.Instance.AccessToken}");
        Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
    }
}
