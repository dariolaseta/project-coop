using System;
using System.Linq;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown inputDeviceDropdown;
    [SerializeField] private TMP_Dropdown outputDeviceDropdown;
    
    private string selectedInputDevice;
    private string selectedOutputDevice;
    
    private const string INPUT_DEVICE_KEY = "SelectedInputDevice";
    private const string OUTPUT_DEVICE_KEY = "SelectedOutputDevice";

    private void Start()
    {
        PopulateInputDeviceList();
        PopulateOutputDeviceList();
    }

    public void PopulateInputDeviceList()
    {
        inputDeviceDropdown.ClearOptions();

        var microphones = Microphone.devices.ToList();

        if (microphones.Count == 0)
        {
            Debug.Log("No microphone found");
            microphones.Add("Nessun microfono trovato.");
        }
        
        inputDeviceDropdown.AddOptions(microphones);
        
        string savedInputDevice = PlayerPrefs.GetString(INPUT_DEVICE_KEY, microphones[0]);
        int savedIndex = microphones.IndexOf(savedInputDevice);

        if (savedIndex == -1) savedIndex = 0;

        selectedInputDevice = microphones[savedIndex];
        
        inputDeviceDropdown.SetValueWithoutNotify(savedIndex);
        inputDeviceDropdown.RefreshShownValue();
        inputDeviceDropdown.onValueChanged.RemoveAllListeners();
        inputDeviceDropdown.onValueChanged.AddListener(OnInputDeviceSelected);
        
        SetActiveInputDevice(selectedInputDevice);
    }
    
    public void PopulateOutputDeviceList()
    {
        outputDeviceDropdown.ClearOptions();

        var outputDevices = VivoxService.Instance.AvailableOutputDevices.Select(d => d.DeviceName).ToList();

        if (outputDevices.Count == 0)
        {
            Debug.Log("No output device found");
            outputDevices.Add("Nessun dispositivo di output trovato");
        }

        outputDeviceDropdown.AddOptions(outputDevices);
        
        string savedOutputDevice = PlayerPrefs.GetString(OUTPUT_DEVICE_KEY, outputDevices[0]);
        int savedIndex = outputDevices.IndexOf(savedOutputDevice);

        if (savedIndex == -1) savedIndex = 0;
        
        selectedOutputDevice = outputDevices[savedIndex];
        
        outputDeviceDropdown.SetValueWithoutNotify(savedIndex);
        outputDeviceDropdown.RefreshShownValue();
        outputDeviceDropdown.onValueChanged.RemoveAllListeners();
        outputDeviceDropdown.onValueChanged.AddListener(OnOutputDeviceSelected);
        
        SetActiveOutputDevice(selectedOutputDevice);
    }

    private void OnOutputDeviceSelected(int index)
    {
        selectedOutputDevice = outputDeviceDropdown.options[index].text;
        
        PlayerPrefs.SetString(OUTPUT_DEVICE_KEY, selectedOutputDevice);
        PlayerPrefs.Save();
        
        SetActiveOutputDevice(selectedOutputDevice);
    }
    
    private async void SetActiveOutputDevice(string outputDeviceName)
    {
        try
        {
            await VivoxService.Instance.SetActiveOutputDeviceAsync(
                VivoxService.Instance.AvailableOutputDevices.First(device => device.DeviceName == outputDeviceName)
            );
            Debug.Log($"Output device set to: {outputDeviceName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to set output device: {e.Message}");
        }
    }

    private void OnInputDeviceSelected(int index)
    {
        selectedInputDevice = inputDeviceDropdown.options[index].text;
        
        PlayerPrefs.SetString(INPUT_DEVICE_KEY, selectedInputDevice);
        PlayerPrefs.Save();
        
        SetActiveInputDevice(selectedInputDevice);
    }

    private async void SetActiveInputDevice(string microphoneName)
    {
        try
        {
            await VivoxService.Instance.SetActiveInputDeviceAsync(
                VivoxService.Instance.AvailableInputDevices.First(device => device.DeviceName == microphoneName)
            );
            Debug.Log($"Microphone set to: {microphoneName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to set microphone: {e.Message}");
        }
    }
}
