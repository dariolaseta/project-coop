using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
        }
    }
}
