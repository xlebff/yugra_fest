using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Welding : MonoBehaviour
{
    [SerializeField] private Transform _targetObject;
    [SerializeField] private GameObject _welding;
    [SerializeField] private GameObject _burning;
    private void FixedUpdate()
    {
        if (Input.GetAxis("XRI_Right_Trigger") >= 0.9f && TrueWelding.WeldingEnabled)
        {
            Instantiate(_welding, new Vector3(_targetObject.position.x, _targetObject.position.y, _targetObject.position.z), Quaternion.identity);
            Instantiate(_burning, new Vector3(_targetObject.position.x, _targetObject.position.y, _targetObject.position.z), Quaternion.identity);

        }
    }
}
