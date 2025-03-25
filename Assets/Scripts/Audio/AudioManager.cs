using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private float audibleRadius = 20f;
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    
    private Dictionary<string, AudioClip> audioClipDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        audioClipDict = new Dictionary<string, AudioClip>();

        foreach (var clip in audioClips)
        {
            if (clip != null && !audioClipDict.ContainsKey(clip.name))
            {
                audioClipDict.Add(clip.name, clip);
            }
        }
    }

    public void PlaySpatialSfx(Vector3 position, string clipName)
    {
        if (!audioClipDict.ContainsKey(clipName))
        {
            Debug.Log($"Audio clip not found: {clipName}");
            return;
        }

        if (IsServer)
        {
            PlaySpatialSfxClientRpc(position, clipName);
        }
        else
        {
            PlaySpatialSfxServerRpc(position, clipName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaySpatialSfxServerRpc(Vector3 position, string clipName)
    {
        PlaySpatialSfxClientRpc(position, clipName);
    }

    [ClientRpc]
    private void PlaySpatialSfxClientRpc(Vector3 position, string clipName)
    {
        if (!audioClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            Debug.Log($"Audio clip not found on client: {clipName}");
            return;
        }

        Vector3 listenerPosition = GetListenerPosition();

        if (Vector3.Distance(listenerPosition, position) <= audibleRadius)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    private Vector3 GetListenerPosition()
    {
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            return listener.transform.position;
        }

        if (Camera.main != null)
        {
            return Camera.main.transform.position;
        }

        Debug.LogWarning("No AudioListener or main Camera found. Using origin as listener position.");
        return Vector3.zero;
    }
}