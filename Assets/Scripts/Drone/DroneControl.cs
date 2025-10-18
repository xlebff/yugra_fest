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

    // Целевая высота для автобалансировки
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

    private void Start() { if (_input != null) _input.OnPrimaryButtonPressed += ToggleDrone; }

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
        // Получаем ввод с контроллеров
        Vector2 leftStick = _input.GetStickInput(XRNode.LeftHand);
        Vector2 rightStick = _input.GetStickInput(XRNode.RightHand);

        // БАЛАНСИРОВКА СИЛ - уменьшаем коэффициенты
        float baseThrust = (leftStick.y + 1f) * 0.3f;

        // Реальные управления как в Mode 2
        float thrust = baseThrust * _maxThrustForce;
        float pitch = rightStick.y * _pitchTorque * 0.1f;
        float roll = rightStick.x * _rollTorque * 0.1f;
        float yaw = leftStick.x * _yawTorque * 0.1f;

        // РАСПРЕДЕЛЕНИЕ СИЛ ПО МОТОРАМ (как в реальном дроне)
        float fl = thrust + pitch + roll - yaw;
        float fr = thrust + pitch - roll + yaw;
        float bl = thrust - pitch + roll + yaw;
        float br = thrust - pitch - roll - yaw;

        // Применяем силы в ЛОКАЛЬНЫХ координатах
        ApplyMotorForce(_frontLeftRotor, fl);
        ApplyMotorForce(_frontRightRotor, fr);
        ApplyMotorForce(_backLeftRotor, bl);
        ApplyMotorForce(_backRightRotor, br);

        // Визуальное вращение роторов
        UpdateRotors(fl, fr, bl, br);
    }

    private void ApplyMotorForce(Transform rotor, float power)
    {
        if (rotor != null)
        {
            // Сила применяется ВВЕРХ от ротора
            Vector3 force = rotor.up * Mathf.Clamp(power, 0, _maxThrustForce) * 0.25f;
            _rb.AddForceAtPosition(force, rotor.position, ForceMode.Force);
        }
    }

    private void ApplyStabilization()
    {
        if (!_enableAutoStabilization) return;

        // 1. Стабилизация высоты
        float currentHeight = transform.position.y;
        float heightDifference = _targetHeight - currentHeight;

        if (heightDifference > 0.1f)
        {
            // Плавно поднимаемся к целевой высоте
            float liftForce = Mathf.Clamp(heightDifference * 2f, 0, 5f);
            _rb.AddForce(Vector3.up * liftForce, ForceMode.Force);
        }

        // 2. Стабилизация углов (автовыравнивание)
        Vector3 currentAngularVelocity = _rb.angularVelocity;
        Vector3 stabilizationTorque = -currentAngularVelocity * _stabilizationForce;
        _rb.AddTorque(stabilizationTorque, ForceMode.Force);

        // 3. Ограничение максимальной высоты
        if (currentHeight > 10f)
        {
            Vector3 downwardForce = Vector3.down * (currentHeight - 10f) * 3f;
            _rb.AddForce(downwardForce, ForceMode.Force);
        }
    }

    private void UpdateRotors(float fl, float fr, float bl, float br)
    {
        float rotorSpeed = 500f; // УМЕНЬШЕНО!

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
            // Плавный взлет до целевой высоты
            _targetHeight = transform.position.y + 2f;
        }
        else Debug.Log("Drone deactivated.");
    }

    public bool IsDroneActive() { return _isActive; }
}