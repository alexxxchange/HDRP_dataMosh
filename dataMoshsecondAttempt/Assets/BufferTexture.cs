using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BufferTexture : MonoBehaviour
{
    [SerializeField] RenderTexture _bufferTexture;


    [ContextMenu("resizerBuffer")]

    public void ResizeBufferTexture()
    {
        if (_bufferTexture)
        {
            _bufferTexture.Release();
            _bufferTexture.width = (int)DataMoshPostProcessvolume.renderDimensions.x;
            _bufferTexture.height = (int)DataMoshPostProcessvolume.renderDimensions.y;

        }
    }
}
