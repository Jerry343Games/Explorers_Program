using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class PixelateCaptureManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _target = null;

    [SerializeField]
    private bool _useAnimation = true;

    [SerializeField]
    private AnimationClip[] _sourceClips = null;

    [SerializeField]
    private int _framesPerSecond = 30;

    [SerializeField]
    private Vector2Int _cellSize = new Vector2Int(100, 100);

    [SerializeField]
    private Vector2 _pivot = new Vector2(0.5f, 0.5f);

    [SerializeField]
    private int _currentFrame = 0;

    [SerializeField]
    private Camera _captureCamera = null;

    [SerializeField]
    private bool _createNormalMap = true;

    [SerializeField]
    private bool _pixelated = true;

    [SerializeField]
    private bool _createAutoMaterial = true;

    [SerializeField]
    private string _spriteSavePath = null;

    //[SerializeField]
    //private string _materialSavePath = null;

    private void OnValidate()
    {
        if (_captureCamera != null && _target != null)
            StartCoroutine(PreviewAnimation());
    }

    public void SampleAnimation(float time, AnimationClip animClip)
    {
        if (_sourceClips == null || _target == null)
        {
            Debug.LogWarning("SourceClip and Target should be set before sample animation!");
            return;
        }
        else
        {
            animClip.SampleAnimation(_target, time);
        }
    }

    public IEnumerator PreviewAnimation()
    {
        if (_sourceClips == null || _target == null)
        {
            Debug.LogWarning("CaptureCamera should be set before capturing animation!");
            yield break;
        }

        var gridCellCount = 1;
        var atlasSize = _cellSize;
        var atlasPos = new Vector2Int(0, atlasSize.y - _cellSize.y);

        if (atlasSize.x > 4096 || atlasSize.y > 4096)
        {
            Debug.LogErrorFormat("Error attempting to capture an animation with a length and" +
                "resolution that would produce a texture of size: {0}", atlasSize);
        }

        var diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
        if (!_pixelated)
        {
            diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
        }
        ClearAtlas(diffuseMap, Color.clear);

        var rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1,
            hideFlags = HideFlags.HideAndDontSave
        };
        if (!_pixelated)
        {
            rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Bilinear,
                antiAliasing = 1,
                hideFlags = HideFlags.HideAndDontSave
            };
        }
        _captureCamera.targetTexture = rtFrame;
        var cachedCameraColor = _captureCamera.backgroundColor;

        yield return null;

        _captureCamera.backgroundColor = Color.clear;
        _captureCamera.Render();
        Graphics.SetRenderTarget(rtFrame);
        diffuseMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
        diffuseMap.Apply();

        atlasPos.x += _cellSize.x;

        if ((_currentFrame + 1) % gridCellCount == 0)
        {
            atlasPos.x = 0;
            atlasPos.y -= _cellSize.y;
        }

        File.WriteAllBytes(Application.dataPath + "/Pixelate/Resources/Editor/preview.png", diffuseMap.EncodeToPNG());
        AssetDatabase.Refresh();

        var importer = Resources.Load<Texture2D>("Editor\\preview");

        if (_pixelated)
            importer.filterMode = FilterMode.Point;
        else
            importer.filterMode = FilterMode.Bilinear;

        Graphics.SetRenderTarget(null);
        _captureCamera.targetTexture = null;
        _captureCamera.backgroundColor = cachedCameraColor;
        DestroyImmediate(rtFrame);
    }

    public IEnumerator CaptureFrame(Action<Texture2D, Texture2D, bool, bool, bool, Vector2, Vector2, AnimationClip> onComplete)
    {
        if (_target == null)
        {
            Debug.LogWarning("CaptureCamera should be set before capturing animation!");
            yield break;
        }

        RenderPipelineAsset defaultGraphicsPipeline = GraphicsSettings.defaultRenderPipeline;
        RenderPipelineAsset defaultQualityPipeline = QualitySettings.renderPipeline;

        var atlasSize = _cellSize;
        var atlasPos = new Vector2Int(0, atlasSize.y - _cellSize.y);

        if (atlasSize.x > 4096 || atlasSize.y > 4096)
        {
            Debug.LogErrorFormat("Error attempting to capture an animation with a length and" +
                "resolution that would produce a texture of size: {0}", atlasSize);
        }

        var diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
        if (!_pixelated)
        {
            diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
        }
        ClearAtlas(diffuseMap, Color.clear);


        var normalMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
        if (!_pixelated)
        {
            normalMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
        }
        ClearAtlas(normalMap, new Color(0.5f, 0.5f, 1.0f, 0.0f));


        var rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1,
            hideFlags = HideFlags.HideAndDontSave
        };
        if (!_pixelated)
        {
            rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Bilinear,
                antiAliasing = 1,
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        var normalCaptureShader = Shader.Find("Hidden/ViewSpaceNormal");

        _captureCamera.targetTexture = rtFrame;
        var cachedCameraColor = _captureCamera.backgroundColor;

        try
        {
            var currentTime = (_currentFrame) * _sourceClips[0].length;
            SampleAnimation(currentTime, _sourceClips[0]);
            yield return null;

            QualitySettings.renderPipeline = defaultQualityPipeline;
            GraphicsSettings.defaultRenderPipeline = defaultGraphicsPipeline;

            _captureCamera.backgroundColor = Color.clear;
            _captureCamera.Render();
            Graphics.SetRenderTarget(rtFrame);
            diffuseMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
            diffuseMap.Apply();

            QualitySettings.renderPipeline = null;
            GraphicsSettings.defaultRenderPipeline = null;

            _captureCamera.backgroundColor = new Color(0.5f, 0.5f, 1.0f, 0.0f);
            _captureCamera.RenderWithShader(normalCaptureShader, "");
            Graphics.SetRenderTarget(rtFrame);
            normalMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
            normalMap.Apply();

            atlasPos.x += _cellSize.x;

            onComplete.Invoke(diffuseMap, normalMap, _createNormalMap, _useAnimation, _pixelated, _cellSize, new Vector2(0.5f, 0.5f), _sourceClips[0]);
        }
        finally
        {
            QualitySettings.renderPipeline = defaultQualityPipeline;
            GraphicsSettings.renderPipelineAsset = defaultGraphicsPipeline;
            Graphics.SetRenderTarget(null);
            _captureCamera.targetTexture = null;
            _captureCamera.backgroundColor = cachedCameraColor;
            DestroyImmediate(rtFrame);
        }
    }

    public IEnumerator CaptureAnimation(Action<Texture2D, Texture2D, bool, bool, bool, Vector2, Vector2, AnimationClip> onComplete)
    {
        if (_sourceClips == null || _target == null)
        {
            Debug.LogWarning("CaptureCamera should be set before capturing animation!");
            yield break;
        }

        RenderPipelineAsset defaultGraphicsPipeline = GraphicsSettings.defaultRenderPipeline;
        RenderPipelineAsset defaultQualityPipeline = QualitySettings.renderPipeline;

        foreach (AnimationClip animClip in _sourceClips)
        {
            var numFrames = (int)(animClip.length * _framesPerSecond);
            var gridCellCount = SqrtCeil(numFrames);
            var atlasSize = new Vector2Int(_cellSize.x * gridCellCount, _cellSize.y * gridCellCount);
            var atlasPos = new Vector2Int(0, atlasSize.y - _cellSize.y);

            if (atlasSize.x > 4096 || atlasSize.y > 4096)
            {
                Debug.LogErrorFormat("Error attempting to capture an animation with a length and" +
                    "resolution that would produce a texture of size: {0}", atlasSize);
            }

            var diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };
            if (!_pixelated)
            {
                diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
                {
                    filterMode = FilterMode.Bilinear
                };
            }
            ClearAtlas(diffuseMap, Color.clear);


            var normalMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };
            if (!_pixelated)
            {
                normalMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
                {
                    filterMode = FilterMode.Bilinear
                };
            }
            ClearAtlas(normalMap, new Color(0.5f, 0.5f, 1.0f, 0.0f));


            var rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point,
                antiAliasing = 1,
                hideFlags = HideFlags.HideAndDontSave
            };
            if (!_pixelated)
            {
                rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
                {
                    filterMode = FilterMode.Bilinear,
                    antiAliasing = 1,
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            var normalCaptureShader = Shader.Find("Hidden/ViewSpaceNormal");

            _captureCamera.targetTexture = rtFrame;
            var cachedCameraColor = _captureCamera.backgroundColor;

            try
            {
                for (_currentFrame = 0; _currentFrame < numFrames; _currentFrame++)
                {
                    var currentTime = (_currentFrame / (float)numFrames) * animClip.length;
                    SampleAnimation(currentTime, animClip);
                    yield return null;

                    QualitySettings.renderPipeline = defaultQualityPipeline;
                    GraphicsSettings.defaultRenderPipeline = defaultGraphicsPipeline;

                    _captureCamera.backgroundColor = Color.clear;
                    _captureCamera.Render();
                    Graphics.SetRenderTarget(rtFrame);
                    diffuseMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
                    diffuseMap.Apply();

                    QualitySettings.renderPipeline = null;
                    GraphicsSettings.defaultRenderPipeline = null;

                    _captureCamera.backgroundColor = new Color(0.5f, 0.5f, 1.0f, 0.0f);
                    _captureCamera.RenderWithShader(normalCaptureShader, "");
                    Graphics.SetRenderTarget(rtFrame);
                    normalMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
                    normalMap.Apply();

                    atlasPos.x += _cellSize.x;

                    if ((_currentFrame + 1) % gridCellCount == 0)
                    {
                        atlasPos.x = 0;
                        atlasPos.y -= _cellSize.y;
                    }
                }
                onComplete.Invoke(diffuseMap, normalMap, _createNormalMap, _useAnimation, _pixelated, _cellSize, _pivot, animClip);
            }
            finally
            {
                QualitySettings.renderPipeline = defaultQualityPipeline;
                GraphicsSettings.renderPipelineAsset = defaultGraphicsPipeline;
                Graphics.SetRenderTarget(null);
                _captureCamera.targetTexture = null;
                _captureCamera.backgroundColor = cachedCameraColor;
                DestroyImmediate(rtFrame);
            }
        }
    }

    private int SqrtCeil(int input)
    {
        return Mathf.CeilToInt(Mathf.Sqrt(input));
    }

    private void ClearAtlas(Texture2D texture, Color color)
    {
        var pixels = new Color[texture.width * texture.height];
        for (var i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }
}
