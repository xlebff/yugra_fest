using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandController : MonoBehaviour
{
    public InputActionReference gripInputActionReference, triggerInputActionReference;
    private Animator _handAnimator;
    private float _gripValue, _triggerValue;

    private void Start()
    {
        _handAnimator = GetComponent<Animator>();
    }

    private void AnimateGrip()
    {
        _gripValue = gripInputActionReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Grip", _gripValue);
    }

    private void AnimateTrigger()
    {
        _triggerValue = triggerInputActionReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Trigger", _triggerValue);
    }

    private void Update()
    {
        AnimateGrip();
        AnimateTrigger();
    }
}
