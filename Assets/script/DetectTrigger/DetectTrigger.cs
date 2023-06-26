using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrigger : MonoBehaviour
{
    public bool detected = false;
    public int object_count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DummyCar") || other.CompareTag("Q_car"))
        {
            detected = true;
            object_count += 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DummyCar") || other.CompareTag("Q_car"))
        {
            object_count -= 1;
            if (object_count == 0)
            {
                detected = false;
            }
        }
    }
}