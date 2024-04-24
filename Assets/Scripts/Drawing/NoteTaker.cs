using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NoteTaker : MonoBehaviour
{
    [SerializeField]
    GameObject linePrefab;

    LineRenderer currentLine;

    [SerializeField]
    Transform penTip;

    [SerializeField]
    float minPointDistance = 0.01f;



    Vector3 penPosition => new Vector3(penTip.position.x, transform.position.y, penTip.position.z);

    public event Action<bool> onNotePadInteraction;//enter = true | exit = false

    IEnumerator drawRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pen"))
        {
            onNotePadInteraction?.Invoke(true);
            StartCoroutine(drawRoutine = DrawRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pen"))
        {
            onNotePadInteraction?.Invoke(false);
            StopCoroutine(drawRoutine);
        }
    }

    private void StartDrawing()
    {
        StartCoroutine(drawRoutine = DrawRoutine());
    }

    IEnumerator DrawRoutine()
    {
        CreateLine();
        bool drawing = true;
        Vector3 prevPosition = penPosition;
        while (drawing)
        {
            if(Vector3.Distance(prevPosition, penPosition) > minPointDistance)
            {
                AddPoint(penPosition);
                prevPosition = penPosition;
            }
            yield return null;
        }

        yield return null;
    }

    void CreateLine()
    {
        GameObject newLine = Instantiate(linePrefab);
        currentLine = newLine.GetComponent<LineRenderer>();

        currentLine.SetPosition(0, penPosition);
        currentLine.SetPosition(1, penPosition);
    }

    void AddPoint(Vector3 point)
    {
        currentLine.positionCount++;
        int positionIndex = currentLine.positionCount - 1;
        currentLine.SetPosition(positionIndex, point);
    }
}
