using UnityEngine;
using UnityEngine.XR;

public class SimpleDroneControl : MonoBehaviour
{
    [Header("Simple Drone Settings")]
    public float moveSpeed = 5f;
    public float riseSpeed = 3f;
    public float rotationSpeed = 2f;
    public float maxHeight = 10f;
    public float minHeight = 1f;

    [Header("Auto Stabilization")]
    public bool autoStabilize = true;
    public float stabilizationSpeed = 3f;

    private Rigidbody rb;
    private XRInputManager input;
    private bool isActive = false;

    // Для плавного движения
    private Vector3 currentVelocity;
    private float currentRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = XRInputManager.Instance;

        // Настраиваем физику для плавности
        rb.drag = 1f;
        rb.angularDrag = 2f;
        rb.useGravity = false;

        // Подписка на события
        if (input != null)
        {
            input.OnPrimaryButtonPressed += ToggleDrone;
            input.OnSecondaryButtonPressed += ResetDrone;
        }
    }

    private void OnDestroy()
    {
        if (input != null)
        {
            input.OnPrimaryButtonPressed -= ToggleDrone;
            input.OnSecondaryButtonPressed -= ResetDrone;
        }
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        HandleSimpleControls();
        ApplyAutoStabilization();
        ApplyMovement();
    }

    private void HandleSimpleControls()
    {
        Vector2 leftStick = input.GetStickInput(XRNode.LeftHand);
        Vector2 rightStick = input.GetStickInput(XRNode.RightHand);

        // ЛЕВЫЙ СТИК - СКОРОСТЬ и ПОВОРОТ
        // Вертикально - скорость вперед/назад
        float forwardSpeed = leftStick.y * moveSpeed;

        // Горизонтально - поворот
        float turnSpeed = leftStick.x * rotationSpeed;

        // ПРАВЫЙ СТИК - ВЫСОТА и СТРАФ
        // Вертикально - вверх/вниз
        float verticalSpeed = rightStick.y * riseSpeed;

        // Горизонтально - влево/вправо
        float strafeSpeed = rightStick.x * moveSpeed * 0.7f;

        // Собираем все вместе
        Vector3 targetVelocity = new Vector3(strafeSpeed, verticalSpeed, forwardSpeed);

        // Плавная интерполяция скорости
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
        currentRotation = Mathf.Lerp(currentRotation, turnSpeed, Time.fixedDeltaTime * 3f);
    }

    private void ApplyMovement()
    {
        // Движение в ЛОКАЛЬНЫХ координатах дрона
        Vector3 localMove = transform.TransformDirection(currentVelocity);
        rb.velocity = new Vector3(localMove.x, currentVelocity.y, localMove.z);

        // Поворот
        Vector3 rotation = new Vector3(0, currentRotation, 0);
        rb.angularVelocity = rotation;

        // Ограничение высоты
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);
        transform.position = position;
    }

    private void ApplyAutoStabilization()
    {
        if (!autoStabilize) return;

        // Автоматическое выравнивание дрона
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                            Time.fixedDeltaTime * stabilizationSpeed);

        // Автостабилизация по высоте (если не управляют)
        float rightStickY = input.GetStickInput(XRNode.RightHand).y;
        if (Mathf.Abs(rightStickY) < 0.1f)
        {
            // Плавно поддерживаем текущую высоту
            float currentHeight = transform.position.y;
            float targetHeight = Mathf.Clamp(currentHeight, minHeight + 1f, maxHeight - 1f);

            if (Mathf.Abs(currentHeight - targetHeight) > 0.2f)
            {
                float heightCorrection = (targetHeight - currentHeight) * 0.5f;
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + heightCorrection, rb.velocity.z);
            }
        }
    }

    private void ToggleDrone()
    {
        isActive = !isActive;

        if (isActive)
        {
            Debug.Log("🚁 Дрон включен! Управление:");
            Debug.Log("Левый стик ↑↓ - вперед/назад");
            Debug.Log("Левый стик ←→ - поворот");
            Debug.Log("Правый стик ↑↓ - вверх/вниз");
            Debug.Log("Правый стик ←→ - влево/вправо");

            // Плавный взлет при включении
            if (transform.position.y < minHeight + 0.5f)
            {
                transform.position = new Vector3(transform.position.x, minHeight + 1f, transform.position.z);
            }
        }
        else
        {
            Debug.Log("Дрон выключен");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ResetDrone()
    {
        Debug.Log("Сброс дрона");

        // Возврат в начальную позицию (или над землей)
        Vector3 resetPosition = new Vector3(0, minHeight + 2f, 0);
        transform.position = resetPosition;
        transform.rotation = Quaternion.identity;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isActive = true;
    }
}