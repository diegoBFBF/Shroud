using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarTextController : MonoBehaviour
{
    [SerializeField]
    public Text titleText;

    Transform textContainer;

    [SerializeField]
    float minDisplayRange = 0.05f;
    [SerializeField]
    float maxDisplayRange = 0.15f;

    [SerializeField]
    float scaleSpeed = 5;

    [SerializeField]
    float heightMultiplier = 0.01f;

    private void Awake()
    {
        textContainer = titleText.transform.parent;
    }

    public void SetTitleText(string newText)
    {
        titleText.text = newText;
    }
    public string GetTitleText() { return titleText.text; }


    public void ScaleText(float distance)
    {
        if(distance == 0)
        {
            textContainer.localScale = Vector3.zero;
            textContainer.position = transform.position;
            return;
        }

        float newScale = Mathf.Lerp(0, 1, (maxDisplayRange - distance) / (maxDisplayRange - minDisplayRange));
        newScale = Mathf.Lerp(textContainer.localScale.x, newScale, scaleSpeed * Time.deltaTime);
        textContainer.localScale = Vector3.one * newScale;

        textContainer.position = transform.position + (Vector3.up * (newScale * 0.01f)) + (Vector3.up * heightMultiplier);
    }
}
