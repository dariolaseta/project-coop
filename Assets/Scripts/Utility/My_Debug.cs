using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class My_Debug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button clearPlayerPrefsButton;

    [SerializeField] private GameObject debugUI;
    
    [SerializeField] private InputActionReference showDebugButtonAction;
    
    private bool isDebugButtonActive = false;

    private void Awake()
    {
        showDebugButtonAction.action.Enable();
        showDebugButtonAction.action.performed += OnInputPressed;
    }

    private void OnInputPressed(InputAction.CallbackContext obj)
    {
        isDebugButtonActive = !isDebugButtonActive;
        
        debugUI.SetActive(isDebugButtonActive);
    }

    private void OnDestroy()
    {
        showDebugButtonAction.action.Disable();
        showDebugButtonAction.action.performed -= OnInputPressed;
    }

    private void Start()
    {
        clearPlayerPrefsButton.onClick.AddListener(DeleteAllPlayerPrefs);
        
        debugUI.SetActive(false);
    }

    private void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("All PlayerPrefs Deleted");
    }
}
