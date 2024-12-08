using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

interface IInteractable {
    void Interact();
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private float InteractRange;
    
    [SerializeField] private Transform InteractorSource;

    [SerializeField] private Image bigCursor;
    [SerializeField] private Image grabCursor;

    [SerializeField] InputActionReference interactAction;

    private void Start() {
        
        bigCursor = GameObject.FindGameObjectWithTag("BigCursor").GetComponent<Image>();
        grabCursor = GameObject.FindGameObjectWithTag("GrabCursor").GetComponent<Image>();

        bigCursor.enabled = false;
        grabCursor.enabled = false;
    }

    void OnEnable() {

        interactAction.action.started += OnInteractPerformed;
    }

    void OnDisable() {

        interactAction.action.started -= OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context) {

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange)) {

            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj) && GameController.Instance.State == GameState.FreeRoam) {

                interactObj.Interact();
            }
        }
    }

    void Update() {

        HandleInteraction();
    }

    private void HandleInteraction() {
        
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange)) {

            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj)) {

                UpdateCursor(hitInfo.collider.gameObject.tag);
            } else {

                ResetCursors();
            }
        } else {

            ResetCursors();
        }

        Debug.DrawRay(r.origin, r.direction * InteractRange, Color.green, 1f);
    }

    private void UpdateCursor(string interactObjTag) {

        switch (interactObjTag)
        {
            case "Pickup":
                grabCursor.enabled = true;
                break;
            
            default:
                grabCursor.enabled = false;
                bigCursor.enabled = false;
                break;
        }
    }

    private void ResetCursors() {

        grabCursor.enabled = false;
        bigCursor.enabled = false;
    }
}
