using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Start()
    {
        readyButton.onClick.AddListener(CharacterSelectReady.Instance.SetPlayerReady);
    }
}
