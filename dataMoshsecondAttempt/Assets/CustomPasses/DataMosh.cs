
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class DataMosh : MonoBehaviour
{ 
    Camera cam;
    public Material DMmat; //datamosh material
    [SerializeField] RenderTexture _MotionVectorTex;


void Start()
{
        cam = GetComponent<Camera>();
        // Camera.main.depthTextureMode = DepthTextureMode.MotionVectors;
        DMmat.SetTexture("_MotionVectorTex", _MotionVectorTex);
    }

private void OnRenderImage(RenderTexture src, RenderTexture dest)
{

        Graphics.Blit(src, dest, DMmat);
}
}
