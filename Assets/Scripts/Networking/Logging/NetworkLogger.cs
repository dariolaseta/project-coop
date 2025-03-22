using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class NetworkLogEvent
{
    public string timestamp;
    public string message;
}

[Serializable]
public class NetworkLogCollection
{
    public List<NetworkLogEvent> events =  new List<NetworkLogEvent>();
}

public class NetworkLogger : MonoBehaviour
{
    public static NetworkLogger Instance { get; private set; }
    
    private List<NetworkLogEvent> logEvents = new List<NetworkLogEvent>();
    private string filePath;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        
        filePath = Application.persistentDataPath + "networkLogs.json";

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkLogger_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkLogger_OnClientDisconnectCallback;

            if (NetworkManager.Singleton.IsServer)
            {
                AddLog("[NetworkLogger] Server avviato");
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                AddLog("[NetworkLogger] Client avviato");
            }
        }
        else
        {
            AddLog("[NetworkLogger] Network Manager non trovato!");
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkLogger_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkLogger_OnClientDisconnectCallback;
        }

        SaveLogsToJson();
    }

    private void NetworkLogger_OnClientConnectedCallback(ulong clientId)
    {
        AddLog($"[NetworkLogger] Client con ID {clientId} si è connesso.");
    }

    private void NetworkLogger_OnClientDisconnectCallback(ulong clientId)
    {
        AddLog($"[NetworkLogger] Client con ID {clientId} si è disconnesso.");
    }

    public void LogNetworkEvent(string message)
    {
        AddLog($"[Network Event] {message}");
    }
    
    private void AddLog(string message)
    {
        var logEvent = new NetworkLogEvent
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            message = message
        };
        
        logEvents.Add(logEvent);
        
        SaveLogsToJson();
    }
    
    private void SaveLogsToJson()
    {
        NetworkLogCollection collection = new NetworkLogCollection { events = logEvents };
        string json = JsonUtility.ToJson(collection, true);

        try
        {
            File.WriteAllText(filePath, json);
            
            Debug.Log("Log salvato con successo.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Errore nel salvataggio del log: {e.Message}");
        }
    }
}
