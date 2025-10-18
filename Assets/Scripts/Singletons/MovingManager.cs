using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovingManager : MonoBehaviour
{
    public static MovingManager Instance { get; private set; }

    private LocomotionSystem _locomotionSystem;
    private ContinuousMoveProviderBase _continuousMove;
    private SnapTurnProviderBase _snapTurn;

    private float _initSpeed;

    private void OnValidate()
    {
        _locomotionSystem = _locomotionSystem ?? FindObjectOfType<LocomotionSystem>();
        _continuousMove = _continuousMove ?? FindObjectOfType<ContinuousMoveProviderBase>();
        _snapTurn = _snapTurn ?? FindObjectOfType<SnapTurnProviderBase>();
        _initSpeed = _continuousMove.moveSpeed;

        if (_locomotionSystem is null)
            Debug.LogWarning("Error during components search.", this);
    }

    public ContinuousMoveProviderBase GetContinuousMoveProvider() => _continuousMove;

    public float ReduceSpeed(float coef) => _continuousMove.moveSpeed += coef;

    public float ResetSpeed() => _continuousMove.moveSpeed = _initSpeed;

    public void MoveEnabling()
    {
        if (_locomotionSystem is not null)
            _locomotionSystem.enabled = true;

        if (_continuousMove is not null)
            _continuousMove.enabled = true;

        if (_snapTurn is not null)
            _snapTurn.enabled = true;

        Debug.Log("Moving has been enabled.");
    }

    public void MoveDisabling()
    {
        if (_locomotionSystem is not null)
            _locomotionSystem.enabled = false;

        if (_continuousMove is not null)
            _continuousMove.enabled = false;

        if (_snapTurn is not null)
            _snapTurn.enabled = false;

        Debug.Log("Moving has been disabled.");
    }
}
