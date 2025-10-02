using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueWelding : MonoBehaviour
{
   public static bool WeldingEnabled = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            WeldingEnabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            WeldingEnabled = false;
        }
    }
}
