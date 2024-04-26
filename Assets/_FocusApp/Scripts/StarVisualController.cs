using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class StarVisualController : MonoBehaviour
{
    [SerializeField]
    MeshRenderer starRenderer;
    [SerializeField]
    MeshRenderer glowRenderer;

    float glowScale = 0.3f;

    public void ScaleGlow(float scalePercent)
    {
        glowRenderer.transform.localScale = Vector3.one * scalePercent * glowScale;
        
        if (scalePercent <= 0)
        {
            if (glowRenderer.gameObject.activeInHierarchy) glowRenderer.gameObject.SetActive(false);
        }
        else
        {
            if (!glowRenderer.gameObject.activeInHierarchy) glowRenderer.gameObject.SetActive(true);
        }
    }

}
