using UnityEngine;
using UnityEngine.XR;

public class DroneControl : MonoBehaviour
{
    [Header("Drone Physics Settings")]
    [SerializeField] private float _maxThrustForce = 15f;
    [SerializeField] private float _pitchTorque = 8f;
    [SerializeField] private float _rollTorque = 8f;
    [SerializeField] private float _yawTorque = 4f;
    [SerializeField] private float _drag = 0.1f;
    [SerializeField] private float _angularDrag = 2f;

    [Header("Rotor References")]
    [SerializeField] private Transform _frontLeftRotor;
    [SerializeField] private Transform _frontRightRotor;
    [SerializeField] private Transform _backLeftRotor;
    [SerializeField] private Transform _backRightRotor;

    [Header("Auto Stabilization")]
    [SerializeField] private bool _enableAutoStabilization = true;
    [SerializeField] private float _stabilizationForce = 2f;

    private Rigidbody _rb;
    private XRInputManager _input;

    [SerializeField] private float _targetHeight = 10f;
    private bool _isActive = false;

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
        _input = XRInputManager.Instance;

        _rb.drag = _drag;
        _rb.angularDrag = _angularDrag;
        _rb.useGravity = true;
    }

    private void Start() { _input.OnPrimaryButtonPressed += ToggleDrone; }

    private void OnDestroy()
    {
        if (_input != null) _input.OnPrimaryButtonPressed -= ToggleDrone;
    }

    private void FixedUpdate()
    {
        if (_input == null || !_isActive) return;

        ApplyDronePhysics();
        ApplyStabilization();
    }

    private void ApplyDronePhysics()
    {
        Vector2 leftStick = _input.GetStickInput(XRNode.LeftHand);
        Vector2 rightStick = -(_input.GetStickInput(XRNode.RightHand));

        float baseThrust = (leftStick.y + 1f) * 0.3f;

        float thrust = baseThrust * _maxThrustForce;
        float pitch = rightStick.y * _pitchTorque * 0.1f;
        float roll = rightStick.x * _rollTorque * 0.1f;
        float yaw = leftStick.x * _yawTorque * 0.1f;
        float manualYaw = leftStick.x * _yawTorque * 0.1f;

        float fl = thrust + pitch + roll - manualYaw;
        float fr = thrust + pitch - roll + manualYaw;
        float bl = thrust - pitch + roll + manualYaw;
        float br = thrust - pitch - roll - manualYaw;

        ApplyMotorForce(_frontLeftRotor, fl);
        ApplyMotorForce(_frontRightRotor, fr);
        ApplyMotorForce(_backLeftRotor, bl);
        ApplyMotorForce(_backRightRotor, br);

        UpdateRotors(fl, fr, bl, br);

        if (Mathf.Abs(leftStick.x) > 0.1f)
        {
            ApplyTurnVisuals(leftStick.x);
        }
    }

    private void ApplyTurnVisuals(float turnInput)
    {
        float tiltAngle = turnInput * 10f;
        Quaternion targetTilt = Quaternion.Euler(0, 0, -tiltAngle);

        transform.rotation = Quaternion.Slerp(transform.rotation,
                                            transform.rotation * targetTilt,
                                            Time.deltaTime * 3f);
    }

    private void ApplyMotorForce(Transform rotor, float power)
    {
        if (rotor != null)
        {
            Vector3 force = rotor.up * Mathf.Clamp(power, 0, _maxThrustForce) * 0.25f;
            _rb.AddForceAtPosition(force, rotor.position, ForceMode.Force);
        }
    }

    private void ApplyStabilization()
    {
        if (!_enableAutoStabilization) return;

        float currentHeight = transform.position.y;
        float heightDifference = _targetHeight - currentHeight;

        if (heightDifference > 0.1f)
        {
            float liftForce = Mathf.Clamp(heightDifference * 2f, 0, 5f);
            _rb.AddForce(Vector3.up * liftForce, ForceMode.Force);
        }

        Vector3 currentAngularVelocity = _rb.angularVelocity;
        Vector3 stabilizationTorque = -currentAngularVelocity * _stabilizationForce;
        _rb.AddTorque(stabilizationTorque, ForceMode.Force);

        if (currentHeight > 10f)
        {
            Vector3 downwardForce = Vector3.down * (currentHeight - 10f) * 3f;
            _rb.AddForce(downwardForce, ForceMode.Force);
        }
    }

    private void UpdateRotors(float fl, float fr, float bl, float br)
    {
        float rotorSpeed = 500f;

        if (_frontLeftRotor)
            _frontLeftRotor.Rotate(0, fl * rotorSpeed * Time.deltaTime, 0);
        if (_frontRightRotor)
            _frontRightRotor.Rotate(0, fr * rotorSpeed * Time.deltaTime, 0);
        if (_backLeftRotor)
            _backLeftRotor.Rotate(0, bl * rotorSpeed * Time.deltaTime, 0);
        if (_backRightRotor)
            _backRightRotor.Rotate(0, br * rotorSpeed * Time.deltaTime, 0);
    }

    private void ToggleDrone()
    {
        _isActive = !_isActive;

        if (_isActive)
        {
            Debug.Log("Drone activated.");
            _targetHeight = transform.position.y + 2f;
        }
        else Debug.Log("Drone deactivated.");
    }

    public bool IsDroneActive() { return _isActive; }
}
