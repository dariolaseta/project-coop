using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiagnosticTools : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject diagnosticPanel;
    
    [SerializeField] private TMP_Text fpsCounterText;
    [SerializeField] private TMP_Text pingText;
    [SerializeField] private TMP_Text packetLossText;
    [SerializeField] private TMP_Text gpuText;
    [SerializeField] private TMP_Text cpuText;
    
    [SerializeField] private InputActionReference openDiagnosticsAction;

    private int lastFrameIndex;

    private float[] frameDeltaTimeArray;

    private void Awake()
    {
        openDiagnosticsAction.action.Enable();
        openDiagnosticsAction.action.performed += OnDiagnosticOpen;
        
        frameDeltaTimeArray = new float[50];
    }

    private void OnDestroy()
    {
        openDiagnosticsAction.action.Disable();
        openDiagnosticsAction.action.performed -= OnDiagnosticOpen;
    }

    private void Start()
    {
        cpuText.text = $"CPU: {SystemInfo.processorType}";
        gpuText.text = $"GPU: {SystemInfo.graphicsDeviceType}";
        
        diagnosticPanel.SetActive(false);
    }

    private void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;
        
        fpsCounterText.text = "FPS: " + Mathf.RoundToInt(CalculateFPS());
    }
    
    private void OnDiagnosticOpen(InputAction.CallbackContext obj)
    {
        bool isOpen = !diagnosticPanel.activeSelf;
        
        diagnosticPanel.SetActive(isOpen);
    }

    private float CalculateFPS()
    {
        float total = 0;

        foreach (float delta in frameDeltaTimeArray)
        {
            total += delta;
        }

        return frameDeltaTimeArray.Length / total;
    }
}
