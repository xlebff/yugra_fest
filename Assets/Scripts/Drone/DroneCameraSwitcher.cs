using UnityEngine;

enum State { PLAYER, DRONE }

public class DroneCameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _droneCamera;
    private State _state = State.PLAYER;

    public void ChangeCamera()
    {
        if (_state == State.PLAYER) ToDrone();
        else ToPlayer();
    }

    private void ToDrone()
    {
        _playerCamera.enabled = false;
        _droneCamera.enabled = true;
        _state = State.DRONE;
    }

    private void ToPlayer()
    {
        _playerCamera.enabled = true;
        _droneCamera.enabled = false;
        _state = State.PLAYER;
    }
}
