using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class OLD_PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;
    [SerializeField] private float defaultHeight = 2f;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference runAction;
    
    [SerializeField] private AudioClip footstepSound;
    
    [SerializeField] private PlayerVisual playerVisual;
    
    private const float Gravity = 10f;
    private float rotationX = 0;
    private float startWalkingSpeed;
    private float startRunningSpeed;

    private bool isMoving = false;
    private bool isRunning = false;
    private bool isCrouching = false;

    private AudioSource audioSource;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 lookInput;

    private CharacterController characterController;
    private Animator anim;
    private Camera playerCamera;
    
    private PlayerLogic playerLogic;
    private GameObject items;

    private void Awake()
    {
        ObtainComponent();
    }

    private void Start()
    {
        Init();

        BindInputActions();
    }

    private void Update()
    {
        CheckForState();
    }

    private void OnDisable() 
    {
        if (IsOwner)
        {
            DisableInputActions();
        }
    }

    public override void OnNetworkSpawn()
    {
        items = transform.Find("Items")?.gameObject;

        if (IsOwner)
        {
            EnableInputActions();
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //TODO FIX
        //if (clientId == OwnerClientId && HasItems())
        //{
        //    MultiplayerManager.Instance.DestroyItem(GetActiveItemName());
        //}
    }

    private string GetActiveItemName()
    {
        if (items)
        {
            foreach (Transform child in items.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    return child.name;
                }
            }
        }
        
        return null;
    }

    private bool HasItems()
    {
        if (items)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private void BindInputActions() 
    {

        moveAction.action.performed += ctx =>
        {
            if (ctx.control.device is Gamepad)
            {
                moveDirection = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
            }
            else
            {
                moveDirection = ctx.ReadValue<Vector3>();
            }
        };
        moveAction.action.canceled += ctx => moveDirection = Vector3.zero;

        lookAction.action.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        lookAction.action.canceled += ctx => lookInput = Vector2.zero;

        runAction.action.performed += ctx => isRunning = true;
        runAction.action.canceled += ctx => isRunning = false;
    }

    private void EnableInputActions() 
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        runAction.action.Enable();
    }

    private void DisableInputActions() 
    {
        
        moveAction.action.Disable();
        lookAction.action.Disable();
        runAction.action.Disable();

        moveAction.action.started -= ctx => moveDirection = Vector3.zero;
        lookAction.action.started -= ctx => lookInput = Vector2.zero;
        runAction.action.started -= ctx => isRunning = false;
    }

    private void MoveCharacter()
    {

        if (!IsOwner || !playerLogic.CanMove()) return;

        Vector3 desiredMoveDirection = transform.TransformDirection(new Vector3(moveDirection.x, 0, moveDirection.z));
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        desiredMoveDirection *= currentSpeed;

        if (!characterController.isGrounded) {
            moveDirection.y -= Gravity * Time.deltaTime;
        } else {
            moveDirection.y = 0;
        }

        characterController.Move((desiredMoveDirection + Vector3.up * moveDirection.y) * Time.deltaTime);

        rotationX += -lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);

        isMoving = moveDirection.sqrMagnitude > 0.01f;
        
        anim.SetBool("isMoving", isMoving);
        anim.speed = isRunning ? 15f : 1f;
    }

    private void CheckForState()
    {
        switch (playerLogic.PlayerState)
        {
            case PlayerState.Freeroam:
                MoveCharacter();
                break;
        }
    }

    private void Init() 
    {
        startWalkingSpeed = walkSpeed;
        startRunningSpeed = runSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));
    }

    private void ObtainComponent() 
    {
        playerLogic = GetComponent<PlayerLogic>();
        
        playerCamera = GetComponentInChildren<Camera>();
        
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }
}
