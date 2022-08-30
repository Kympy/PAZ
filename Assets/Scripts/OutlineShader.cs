using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineShader : MonoBehaviour
{
    private Renderer render;
    private Material outline;
    private List<Material> materials = new List<Material>();
    private void Start()
    {
        render = GetComponent<Renderer>();
        outline = new Material(Shader.Find("Custom/Outline"));
        materials.Clear();
        materials.Add(render.sharedMaterial);
    }
    private void OnMouseEnter()
    {
        materials.Add(outline);
        render.materials = materials.ToArray();
        
    }
    private void OnMouseExit()
    {
        materials.Remove(outline);
        render.materials = materials.ToArray();
    }
}
