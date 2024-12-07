using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;
    [SerializeField] private float defaultHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchSpeed = 3f;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference runAction;
    [SerializeField] private InputActionReference crouchAction;
    
    [SerializeField] private AudioClip footstepSound;

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

    private void OnEnable() 
    {
        
        EnableInputActions();
    }

    private void OnDisable() 
    {

        DisableInputActions();
    }

    private void BindInputActions() 
    {

        moveAction.action.performed += ctx => moveDirection = ctx.ReadValue<Vector3>();
        moveAction.action.canceled += ctx => moveDirection = Vector3.zero;

        lookAction.action.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        lookAction.action.canceled += ctx => lookInput = Vector2.zero;

        runAction.action.performed += ctx => isRunning = true;
        runAction.action.canceled += ctx => isRunning = false;

        crouchAction.action.performed += ctx => ToggleCrouch();
    }

    private void EnableInputActions() 
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        runAction.action.Enable();
        crouchAction.action.Enable();
    }

    private void DisableInputActions() 
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        runAction.action.Disable();
        crouchAction.action.Disable();

        moveAction.action.started -= ctx => moveDirection = Vector3.zero;
        lookAction.action.started -= ctx => lookInput = Vector2.zero;
        runAction.action.started -= ctx => isRunning = false;
        crouchAction.action.started -= ctx => isCrouching = false;
    }

    private void ToggleCrouch() 
    {

        isCrouching = !isCrouching;
        characterController.height = isCrouching ? crouchHeight : defaultHeight;
        walkSpeed = isCrouching ? crouchSpeed : startWalkingSpeed;
        runSpeed = isCrouching ? crouchSpeed : startRunningSpeed;
    }

    private void MoveCharacter() 
    {

        // Determina la velocit� di movimento e la direzione
        Vector3 desiredMoveDirection = transform.TransformDirection(new Vector3(moveDirection.x, 0, moveDirection.z));
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        desiredMoveDirection *= currentSpeed;

        // Gestisce la gravit�
        if (!characterController.isGrounded) {
            moveDirection.y -= Gravity * Time.deltaTime;
        } else {
            moveDirection.y = 0;
        }

        // Applica il movimento
        characterController.Move((desiredMoveDirection + Vector3.up * moveDirection.y) * Time.deltaTime);

        // Rotazione della telecamera
        rotationX += -lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);

        // Gestione dell'animazione
        isMoving = moveDirection.sqrMagnitude > 0.01f;
        
        //anim.SetBool("isMoving", isMoving);
        //anim.speed = isRunning ? 15f : 1f;
    }

    private void CheckForState()
    {
        switch (GameController.Instance.State)
        {
            case GameState.FreeRoam:
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
    }

    private void ObtainComponent() 
    {

        playerCamera = Camera.main;

        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }
}
