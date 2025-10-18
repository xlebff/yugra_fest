using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class XRInputManager : MonoBehaviour
{
    public static XRInputManager Instance { get; private set; }

    // События для кнопок
    public event System.Action OnPrimaryButtonPressed;
    public event System.Action OnSecondaryButtonPressed;
    public event System.Action OnTriggerPressed;
    public event System.Action OnGripPressed;
    public event System.Action OnMenuButtonPressed;

    // Стики (аналоговый ввод)
    public Vector2 LeftStick => _leftStick;
    public Vector2 RightStick => _rightStick;
    public float LeftTrigger => _leftTrigger;
    public float RightTrigger => _rightTrigger;

    private InputDevice _leftController;
    private InputDevice _rightController;
    private Vector2 _leftStick;
    private Vector2 _rightStick;
    private float _leftTrigger;
    private float _rightTrigger;

    // Состояния кнопок (для предотвращения спама)
    private bool _leftPrimaryWasPressed;
    private bool _rightPrimaryWasPressed;
    private bool _leftSecondaryWasPressed;
    private bool _rightSecondaryWasPressed;
    private bool _leftTriggerWasPressed;
    private bool _rightTriggerWasPressed;
    private bool _leftGripWasPressed;
    private bool _rightGripWasPressed;
    private bool _leftMenuWasPressed;
    private bool _rightMenuWasPressed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeControllers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeControllers()
    {
        var leftHandDevices = new List<InputDevice>();
        var rightHandDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

        if (leftHandDevices.Count > 0) _leftController = leftHandDevices[0];
        if (rightHandDevices.Count > 0) _rightController = rightHandDevices[0];

        Debug.Log($"Left controller: {_leftController.name}, Right controller: {_rightController.name}");
    }

    private void Update()
    {
        if (!_leftController.isValid || !_rightController.isValid)
        {
            InitializeControllers();
            return;
        }

        UpdateButtonInput();
        UpdateStickInput();
        UpdateTriggerInput();
    }

    private void UpdateButtonInput()
    {
        // Primary Button (A/X)
        UpdateButtonState(CommonUsages.primaryButton, ref _leftPrimaryWasPressed, ref _rightPrimaryWasPressed, OnPrimaryButtonPressed);

        // Secondary Button (B/Y)
        UpdateButtonState(CommonUsages.secondaryButton, ref _leftSecondaryWasPressed, ref _rightSecondaryWasPressed, OnSecondaryButtonPressed);

        // Trigger Button
        UpdateButtonState(CommonUsages.triggerButton, ref _leftTriggerWasPressed, ref _rightTriggerWasPressed, OnTriggerPressed);

        // Grip Button
        UpdateButtonState(CommonUsages.gripButton, ref _leftGripWasPressed, ref _rightGripWasPressed, OnGripPressed);

        // Menu Button
        UpdateButtonState(CommonUsages.menuButton, ref _leftMenuWasPressed, ref _rightMenuWasPressed, OnMenuButtonPressed);
    }

    private void UpdateButtonState(InputFeatureUsage<bool> button,
                                 ref bool leftWasPressed, ref bool rightWasPressed,
                                 System.Action action)
    {
        // Left controller
        if (_leftController.TryGetFeatureValue(button, out bool leftPressed) && leftPressed)
        {
            if (!leftWasPressed)
            {
                action?.Invoke();
                leftWasPressed = true;
            }
        }
        else
        {
            leftWasPressed = false;
        }

        // Right controller
        if (_rightController.TryGetFeatureValue(button, out bool rightPressed) && rightPressed)
        {
            if (!rightWasPressed)
            {
                action?.Invoke();
                rightWasPressed = true;
            }
        }
        else
        {
            rightWasPressed = false;
        }
    }

    private void UpdateStickInput()
    {
        // Чтение стиков
        _leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out _leftStick);
        _rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out _rightStick);
    }

    private void UpdateTriggerInput()
    {
        // Аналоговые значения триггеров (0-1)
        _leftController.TryGetFeatureValue(CommonUsages.trigger, out _leftTrigger);
        _rightController.TryGetFeatureValue(CommonUsages.trigger, out _rightTrigger);
    }

    // Публичные методы для проверки состояний
    public bool IsPrimaryButtonPressed(XRNode hand = XRNode.RightHand)
    {
        return GetButtonState(CommonUsages.primaryButton, hand);
    }

    public bool IsSecondaryButtonPressed(XRNode hand = XRNode.RightHand)
    {
        return GetButtonState(CommonUsages.secondaryButton, hand);
    }

    public bool IsTriggerPressed(XRNode hand = XRNode.RightHand)
    {
        return GetButtonState(CommonUsages.triggerButton, hand);
    }

    public bool IsGripPressed(XRNode hand = XRNode.RightHand)
    {
        return GetButtonState(CommonUsages.gripButton, hand);
    }

    public bool IsMenuButtonPressed(XRNode hand = XRNode.RightHand)
    {
        return GetButtonState(CommonUsages.menuButton, hand);
    }

    public float GetTriggerValue(XRNode hand = XRNode.RightHand)
    {
        return hand == XRNode.LeftHand ? _leftTrigger : _rightTrigger;
    }

    public Vector2 GetStickInput(XRNode hand = XRNode.LeftHand)
    {
        return hand == XRNode.LeftHand ? _leftStick : _rightStick;
    }

    private bool GetButtonState(InputFeatureUsage<bool> button, XRNode hand)
    {
        var controller = hand == XRNode.LeftHand ? _leftController : _rightController;
        if (controller.isValid && controller.TryGetFeatureValue(button, out bool pressed))
            return pressed;
        return false;
    }
}