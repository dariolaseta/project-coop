using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text timerText;
    
    [Header("Timer Settings")]
    [SerializeField] private float startTime = 300f;
    
    private NetworkVariable<float> remainingTime = new NetworkVariable<float>();
    
    private bool timerStarted = false;

    private void Start()
    {
        if (IsServer)
            remainingTime.Value = startTime;
    }

    private void Update()
    {
        if (!timerStarted) return;
        
        if (IsServer && remainingTime.Value > 0)
        {
            remainingTime.Value -= Time.deltaTime;
            if (remainingTime.Value <= 0)
            {
                remainingTime.Value = 0;
                
                timerText.color = Color.red;
                //GameOver
                
                GameManager.Instance.GameOver();
            }
        }
        
        float displayTime = remainingTime.Value;
        
        int minutes = Mathf.FloorToInt(displayTime / 60);
        int seconds = Mathf.FloorToInt(displayTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}",  minutes, seconds);
    }

    public void StartTimer()
    {
        timerStarted = true;
    }
}
