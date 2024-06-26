using System;
using System.Collections;
using UnityEngine;
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
    GameObject notePrefab;

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
        Vector3 prevPosition = penPosition();
        while (drawing)
        {
            if(Vector3.Distance(prevPosition, penPosition()) > minPointDistance)
            {
                AddPoint(penPosition());
                prevPosition = penPosition();
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

        currentLine.SetPosition(0, penPosition());
        currentLine.SetPosition(1, penPosition());
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


    public void ConfirmNote()
    {
        if (currentSketchContainer.childCount <= 0) return;

        Transform newContainer = Instantiate(notePrefab).transform;
        newContainer.gameObject.name = "new note";
        newContainer.position = currentSketchContainer.position;
        newContainer.rotation = currentSketchContainer.rotation;
        foreach (Transform c in currentSketchContainer)
        {
            if (c == currentSketchContainer) continue;
            c.SetParent(newContainer);
        }

        NoteToolManager.Instance.SketchNoteCreated(newContainer);

    }

    Vector3 penPosition() 
    { 
        Vector3 newPos = transform.InverseTransformPoint(penTip.position);
        newPos = new Vector3(newPos.x, 0, newPos.z);
        return newPos;
    }
}
