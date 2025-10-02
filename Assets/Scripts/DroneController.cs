using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    [Header("Drone Components")]
    [SerializeField] private Rigidbody drone;
    [SerializeField] private GameObject passengerObject;
    [SerializeField] private GameObject[] propellerEffects;

    [Header("Movement Settings")]
    public float forwardBackwardForce = 50f;
    public float tiltTorque = 50f;
    public float strafeForce = 50f;
    public float upDownForce = 50f;
    public float stabilizationForce = 10f;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty leftStickAction;
    [SerializeField] private InputActionProperty rightStickAction;
    [SerializeField] private InputActionProperty gripAction;

    // Input values
    private Vector2 leftStickInput;
    private Vector2 rightStickInput;
    private float gripValue;

    // State
    private bool isPassengerInside = false;
    private Vector3 droneRotation;

    private void OnValidate()
    {
        drone ??= GetComponent<Rigidbody>();
    }

    private void Start()
    {
        EnableInputActions();
    }

    private void EnableInputActions()
    {
        leftStickAction.action.Enable();
        rightStickAction.action.Enable();
        gripAction.action.Enable();
    }

    private void Update()
    {
        UpdateInputValues();
        UpdatePropellerEffects();
        UpdatePassengerPosition();
    }

    private void UpdateInputValues()
    {
        leftStickInput = leftStickAction.action.ReadValue<Vector2>();
        rightStickInput = rightStickAction.action.ReadValue<Vector2>();
        gripValue = gripAction.action.ReadValue<float>();
    }

    private void UpdatePropellerEffects()
    {
        bool propellersActive = gripValue >= 0.1f;
        foreach (var propeller in propellerEffects)
        {
            propeller.SetActive(propellersActive);
        }
    }

    private void UpdatePassengerPosition()
    {
        if (isPassengerInside)
        {
            passengerObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y - 0.55f,
                transform.position.z
            );
        }
    }

    private void FixedUpdate()
    {
        if (gripValue < 0.1f) return; // Only control when grip is pressed

        droneRotation = drone.transform.localEulerAngles;

        StabilizeDrone();
        HandleMovement();
    }

    private void StabilizeDrone()
    {
        StabilizeZAxis();
        StabilizeXAxis();

        // Add slight upward force to prevent fast height loss
        drone.AddForce(0, 9f, 0);
    }

    private void StabilizeZAxis()
    {
        if (droneRotation.z > 10f && droneRotation.z <= 180f)
            drone.AddRelativeTorque(0, 0, -stabilizationForce);
        else if (droneRotation.z > 180f && droneRotation.z <= 350f)
            drone.AddRelativeTorque(0, 0, stabilizationForce);
        else if (droneRotation.z > 1f && droneRotation.z <= 10f)
            drone.AddRelativeTorque(0, 0, -stabilizationForce * 0.3f);
        else if (droneRotation.z > 350f && droneRotation.z < 359f)
            drone.AddRelativeTorque(0, 0, stabilizationForce * 0.3f);
    }

    private void StabilizeXAxis()
    {
        if (droneRotation.x > 10f && droneRotation.x <= 180f)
            drone.AddRelativeTorque(-stabilizationForce, 0, 0);
        else if (droneRotation.x > 180f && droneRotation.x <= 350f)
            drone.AddRelativeTorque(stabilizationForce, 0, 0);
        else if (droneRotation.x > 1f && droneRotation.x <= 10f)
            drone.AddRelativeTorque(-stabilizationForce * 0.3f, 0, 0);
        else if (droneRotation.x > 350f && droneRotation.x < 359f)
            drone.AddRelativeTorque(stabilizationForce * 0.3f, 0, 0);
    }

    private void HandleMovement()
    {
        HandleTiltRotation();
        HandleForwardBackward();
        HandleStrafe();
        HandleUpDown();
    }

    private void HandleTiltRotation()
    {
        // Right stick horizontal for tilt (left/right)
        if (Mathf.Abs(rightStickInput.x) > 0.1f)
        {
            drone.AddRelativeTorque(0, rightStickInput.x * tiltTorque, 0);
        }
    }

    private void HandleForwardBackward()
    {
        // Right stick vertical for forward/backward
        if (Mathf.Abs(rightStickInput.y) > 0.1f)
        {
            drone.AddRelativeForce(0, 0, rightStickInput.y * forwardBackwardForce);
            drone.AddRelativeTorque(rightStickInput.y * 10f, 0, 0);
        }
    }

    private void HandleStrafe()
    {
        // Left stick horizontal for strafe (left/right)
        if (Mathf.Abs(leftStickInput.x) > 0.1f)
        {
            drone.AddRelativeForce(leftStickInput.x * strafeForce, 0, 0);
            drone.AddRelativeTorque(0, 0, -leftStickInput.x * 10f);
        }
    }

    private void HandleUpDown()
    {
        // Left stick vertical for up/down
        if (Mathf.Abs(leftStickInput.y) > 0.1f)
        {
            float verticalForce = leftStickInput.y > 0 ? upDownForce : upDownForce * -1f;
            drone.AddRelativeForce(0, verticalForce, 0);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GRYZ"))
        {
            // You can add input actions for enter/exit if needed
            // For now keeping the F/G keys for demonstration
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                EnterDrone();
            }
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                ExitDrone();
            }
        }
    }

    private void EnterDrone()
    {
        isPassengerInside = true;
        // Additional enter logic here
    }

    private void ExitDrone()
    {
        isPassengerInside = false;
        // Additional exit logic here
    }

    private void OnDestroy()
    {
        // Clean up input actions
        leftStickAction.action?.Disable();
        rightStickAction.action?.Disable();
        gripAction.action?.Disable();
    }
}