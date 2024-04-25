using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField]
    Transform currentSketchContainer;

    [SerializeField]
    Transform noteBoardContainer;

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
            StopCoroutine(drawRoutine);
            onNotePadInteraction?.Invoke(false);
            drawRoutine = null;
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

            if (Mathf.Abs(transform.position.y - penTip.position.y) > 0.03)
            {
                onNotePadInteraction?.Invoke(false);
                drawRoutine = null;
                yield break;
            }
            yield return null;
        }

        yield return null;
    }

    void CreateLine()
    {
        GameObject newLine = Instantiate(linePrefab, currentSketchContainer);
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

    public void EraseSketch()
    {
        foreach(Transform c in currentSketchContainer)
        {
            Destroy(c.gameObject);
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.parent.position = newPosition;
        Debug.Log("NoteTaker - SetPosition");
    }
}
