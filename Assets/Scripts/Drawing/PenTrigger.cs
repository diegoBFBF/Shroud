using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PenTrigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent penEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pen"))
        {
            penEvent.Invoke();
        }
    }
}
