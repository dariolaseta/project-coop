using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetcodeUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    
    [SerializeField] private Camera uiCamera;

    private void Awake()
    {
        hostButton.onClick.AddListener(StartHosting);
        clientButton.onClick.AddListener(StartClient);
    }

    private void Hide()
    {
        uiCamera.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void StartHosting()
    {
        Debug.Log("Starting host");
        NetworkManager.Singleton.StartHost();
        
        Hide();
    }

    private void StartClient()
    {
        Debug.Log("Starting client");
        NetworkManager.Singleton.StartClient();
        
        Hide();
    }
}
