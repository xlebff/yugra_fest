using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DialogueStarter : MonoBehaviour
{
    private Dialogue _dialogue;
    private LocomotionSystem _locomotionSystem;
    private ContinuousMoveProviderBase _continuousMove;
    private SnapTurnProviderBase _snapTurn;

    private const string PLAYER_TAG = "Player";

    private bool _isStarted = false;

    private void OnValidate()
    {
        _locomotionSystem = _locomotionSystem ?? FindObjectOfType<LocomotionSystem>();
        _continuousMove = _continuousMove ?? FindObjectOfType<ContinuousMoveProviderBase>();
        _snapTurn = _snapTurn ?? FindObjectOfType<SnapTurnProviderBase>();
        _dialogue = _dialogue ?? GetComponent<Dialogue>();

        if (_locomotionSystem is null)
            Debug.LogWarning("Error during components search.", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            if (!_isStarted)
            {
                Debug.Log("Starting the dialogue..");
                DialogueStart();
            }
            else Debug.Log("The dialogue has already been completed.");
        }
    }

    private void DialogueStart()
    {
        MoveDisabling();
        _dialogue.enabled = true;

        Debug.Log("The dialogue started.");
        _isStarted = true;
    }

    private void MoveDisabling()
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
