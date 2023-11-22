using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AweseomeScreenShader : MonoBehaviour
{
    // Start is called before the first frame update
    public Shader awesomeShader = null;
    private Material m_renderMaterial;
    void Start()
    {
        if (awesomeShader == null)
        {
            Debug.LogError("no awesome shader.");
            m_renderMaterial = null;
            return;
        }
        m_renderMaterial = new Material(awesomeShader);
    }

    // Update is called once per frame
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_renderMaterial);
    }
}
