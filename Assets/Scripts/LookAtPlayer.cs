using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField]
    Transform headTransform;

    [SerializeField]
    bool lockXAxis;
    [SerializeField]
    bool lockYAxis;
    [SerializeField]
    bool lockZAxis;

    [SerializeField]
    bool reverseDirection;

    public bool lookAtPlayer = true;

    [SerializeField]
    float rotationSpeed = 5f; // You can adjust this value for the desired smoothness

    Quaternion targetRotation;
    
    // Update is called once per frame
    void Update()
    {
        if (!lookAtPlayer) return;
       
        // Calculate the target rotation to look at the player's head
        Vector3 lookDirection = headTransform.position - transform.position;
        if (reverseDirection) lookDirection = transform.position - headTransform.position;
        targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        // Apply axis locks
        Vector3 eulerAngles = targetRotation.eulerAngles;
        if (lockXAxis) eulerAngles.x = transform.eulerAngles.x;
        if (lockYAxis) eulerAngles.y = transform.eulerAngles.y;
        if (lockZAxis) eulerAngles.z = transform.eulerAngles.z;
        targetRotation = Quaternion.Euler(eulerAngles);

        // Smoothly interpolate towards the target rotation using slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetLookAtPlayer(bool newValue)
    {
        lookAtPlayer = newValue;
    }
}

