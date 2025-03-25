using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FinalPlayerMovement : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;

    [SerializeField] private Transform groundCheck;
    
    [SerializeField] private Camera playerCamera;
    
    [SerializeField] PlayerLogic playerLogic;
    
    [SerializeField] PlayerVisual playerVisual;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;
    [SerializeField] private float jumpForce = 10f;
    
    [SerializeField] private float rayDistance = 1f;
    
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    
    private float rotationX = 0f;
    
    private bool isSprinting = false;

    private GameObject items;

    private void Start()
    {
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));
        
        if (!IsOwner) return;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        moveAction.action.Enable();
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMoveCanceled;
        
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;
        
        lookAction.action.Enable();
        lookAction.action.performed += OnCameraMove;
        lookAction.action.canceled += OnCameraMoveCanceled;
        
        sprintAction.action.Enable();
        sprintAction.action.performed += OnSprintPerformed;
        sprintAction.action.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        if (!IsOwner) return;
        
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMoveCanceled;
        moveAction.action.Disable();
        
        jumpAction.action.performed -= OnJump;
        jumpAction.action.Disable();
        
        lookAction.action.performed -= OnCameraMove;
        lookAction.action.canceled -= OnCameraMoveCanceled;
        lookAction.action.Disable();
        
        sprintAction.action.performed -= OnSprintPerformed;
        sprintAction.action.canceled -= OnSprintCanceled;
        sprintAction.action.Disable();
    }

    public override void OnNetworkSpawn()
    {
        items = transform.Find("Items")?.gameObject;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void Update()
    {
        MoveCamera();
    }
    
    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!IsOwner || playerLogic.CanMove()) return;

        if (!IsGrounded())
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                float angle = Vector3.Angle(contact.normal, Vector3.up);

                if (angle < 45f)
                {
                    rb.AddForce(contact.normal * 5f, ForceMode.Impulse);
                    break;
                }
            }
        }
    }


    private void MoveCamera()
    {
        if (!IsOwner || !playerLogic.CanMove()) return;
        
        rotationX += -lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
    }

    private void MoveCharacter()
    {
        if (!IsOwner || !playerLogic.CanMove()) return;
        
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        
        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        
        Vector3 direction = (cameraForward * moveDirection.y + cameraRight * moveDirection.x).normalized;
        
        float currentSpeed = isSprinting ? runSpeed : moveSpeed; 

        rb.linearVelocity = new Vector3(direction.x * currentSpeed, rb.linearVelocity.y, direction.z * currentSpeed);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        // TODO: Remove Held Objects
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        if (!IsOwner || !playerLogic.CanMove()) return;
        
        moveDirection = obj.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        
        moveDirection = Vector2.zero;
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (!IsOwner || !playerLogic.CanMove()) return;

        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCameraMove(InputAction.CallbackContext obj)
    {
        if (!IsOwner || !playerLogic.CanMove()) return;
        
        lookInput = obj.ReadValue<Vector2>();
    }

    private void OnCameraMoveCanceled(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        
        lookInput = Vector2.zero;
    }

    private void OnSprintPerformed(InputAction.CallbackContext obj)
    {
        if (!IsOwner || !playerLogic.CanMove()) return;
        
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        
        isSprinting = false;
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, rayDistance, groundLayer))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            return angle < 30f;
        }
        
        return false;
    }
}
