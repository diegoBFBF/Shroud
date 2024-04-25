using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class MaterialTester : MonoBehaviour
{
    
    public Material targetMaterial; 
    [SerializeField]
    TMP_Text text;
    public float height = 3;

    [SerializeField]
    Renderer[] renderers;

    // MaterialPropertyBlock mpb = new MaterialPropertyBlock();


    void Update()
    {
        var input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y + Input.GetAxis("Vertical");;
        
        text.text = ""+input;
        targetMaterial.SetFloat("_Opacity", input * height);
        
        // targetMaterial.SetFloat("_opacity", input* height);
        // foreach(var r in renderers){
        //     r.SetPropertyBlock(mpb);
        // }
    }
}

