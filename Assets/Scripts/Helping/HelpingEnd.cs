using UnityEngine;

public class HelpingEnd : MonoBehaviour
{
    [SerializeField] private GameObject _separateMilitary;
    [SerializeField] private GameObject _folowingMilitary;
    [SerializeField] private float _coef;

    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            if (_folowingMilitary.activeSelf)
            {
                MovingManager.Instance.ResetSpeed();
                if (_separateMilitary is not null) _separateMilitary.SetActive(true);
                if (_folowingMilitary is not null) _folowingMilitary.SetActive(false);
                Debug.Log("Assistance is done.");
                this.enabled = false;
            }
        }
    }
}
