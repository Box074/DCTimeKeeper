using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderManager : MonoBehaviour
{
    public Material mat;
    
    private void OnEnable()
    {
        bool Test(Material mat)
        {
            if(mat == null) return false;
            if(mat.shader == null) return false;
            if(!mat.shader.isSupported) return false;
            if(mat.shader.name.Contains("InternalErrorShader")) return false;
            return true;
        }
        if (mat == null)
        {
            mat = new Material(Shader.Find("Sprites/Default"));
        }
        foreach (var v in GetComponents<Renderer>())
        {
            if (!Test(v.sharedMaterial) && !Test(v.material)) v.sharedMaterial = mat;
        }
        foreach (var v in GetComponentsInChildren<Renderer>(true))
        {
            if (!Test(v.sharedMaterial) && !Test(v.material))
            {
                v.sharedMaterial = mat;
            }
        }
    }
}
