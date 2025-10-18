using UnityEngine;

public class HelpingStart : MonoBehaviour
{
    [SerializeField] private GameObject _separateMilitary;
    [SerializeField] private GameObject _folowingMilitary;
    [SerializeField] private float _coef;

    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            MovingManager.Instance.ReduceSpeed(_coef);
            if (_separateMilitary is not null) _separateMilitary.SetActive(false);
            if (_folowingMilitary is not null) _folowingMilitary.SetActive(true);
            Debug.Log("Assistance is being provided.");
            this.enabled = false;
        }
    }
}
