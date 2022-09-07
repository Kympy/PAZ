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
        materials.Add(outline); // Add Outline
        render.materials = materials.ToArray(); // Show New Materials
    }
    private void OnMouseExit()
    {
        materials.Remove(outline); // Remove Outline
        render.materials = materials.ToArray(); // Show origin
    }
}
