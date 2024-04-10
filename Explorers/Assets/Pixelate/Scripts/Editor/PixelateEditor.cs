using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PixelateCaptureManager))]
public class PixelateEditor : Editor
{
    string targetName;

    Texture2D headerTexture;
    Texture2D pixelateLogo;
    Texture2D previewImage;

    Rect headerSection;

    private PixelateCaptureManager helper;

    private GUIContent discordButtonTexture;

    private const string ASSIGN_REFS_INFO = "Assign the Target and SourceClip to start previewing!";

    private const string LEGACY_ANIM_WARN = "The SourceClip must be marked as Legacy!";

    private const string ASSIGN_CAMERA_INFO = "Assign a camera to start capturing!";

    private IEnumerator _currentCaptureRoutine;

    void OnEnable()
    {
        InitTextures();
    }

    void InitTextures()
    {
        if (EditorGUIUtility.isProSkin == true)
            pixelateLogo = Resources.Load<Texture2D>("Editor\\Pixelate Logo Light");
        else
            pixelateLogo = Resources.Load<Texture2D>("Editor\\Pixelate Logo Dark");

        previewImage = Resources.Load<Texture2D>("Editor\\preview");

        headerTexture = new Texture2D(1, 1);

        if (EditorGUIUtility.isProSkin == true)
            headerTexture.SetPixel(0, 0, new Color32(28, 28, 28, 255));
        else
            headerTexture.SetPixel(0, 0, new Color32(187, 187, 187, 255));

        headerTexture.Apply();
    }

    public override void OnInspectorGUI()
    {
        using (new EditorGUI.DisabledScope(_currentCaptureRoutine != null))
        {
            InitiateBanner();

            helper = (PixelateCaptureManager)target;
            var targetProp = serializedObject.FindProperty("_target");
            var useAnimation = serializedObject.FindProperty("_useAnimation");
            var sourceClipsProp = serializedObject.FindProperty("_sourceClips");

            if (targetProp.objectReferenceValue != null)
            {
                targetName = targetProp.objectReferenceValue.name;
                targetName = targetName.Replace(" ", "");
            }

            EditorGUILayout.PropertyField(targetProp);
            EditorGUILayout.PropertyField(useAnimation);

            if (useAnimation.boolValue == true)
            {
                EditorGUILayout.PropertyField(sourceClipsProp);
                GUILayout.Space(5);
            }

            GUILayout.Space(10);

            if (targetProp.objectReferenceValue == null || sourceClipsProp.arraySize == 0 && useAnimation.boolValue == true)
            {
                if (sourceClipsProp.arraySize != 0)
                {
                    if (sourceClipsProp.GetArrayElementAtIndex(0).objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox(ASSIGN_REFS_INFO, MessageType.Info);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
                EditorGUILayout.HelpBox(ASSIGN_REFS_INFO, MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            bool emptyClips = false;

            if (sourceClipsProp.arraySize != 0)
            {
                if (sourceClipsProp.GetArrayElementAtIndex(0).objectReferenceValue == null)
                {
                    emptyClips = true;
                }
            }

            if (targetProp.objectReferenceValue != null && !emptyClips)
            {
                var sourceClip = (AnimationClip)sourceClipsProp.GetArrayElementAtIndex(0).objectReferenceValue;
                
                if (useAnimation.boolValue)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Animation Options", EditorStyles.boldLabel);

                        var fpsProp = serializedObject.FindProperty("_framesPerSecond");
                        EditorGUILayout.PropertyField(fpsProp);

                        var previewFrameProp = serializedObject.FindProperty("_currentFrame");
                        var numFrames = (int)(sourceClip.length * fpsProp.intValue) + 1;

                        using (var changeScope = new EditorGUI.ChangeCheckScope())
                        {
                            var frame = previewFrameProp.intValue;
                            frame = EditorGUILayout.IntSlider("Current Frame", frame, 0, numFrames - 1);

                            if (changeScope.changed)
                            {
                                previewFrameProp.intValue = frame;
                                helper.SampleAnimation(frame / (float)numFrames * sourceClip.length, sourceClip);
                            }
                        }
                    }
                }

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Capture Options", EditorStyles.boldLabel);

                    var captureCameraProp = serializedObject.FindProperty("_captureCamera");
                    EditorGUILayout.ObjectField(captureCameraProp, typeof(Camera));

                    if (captureCameraProp.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox(ASSIGN_CAMERA_INFO, MessageType.Info);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    var resolutionProp = serializedObject.FindProperty("_cellSize");
                    EditorGUILayout.PropertyField(resolutionProp);

                    var pivot = serializedObject.FindProperty("_pivot");
                    EditorGUILayout.PropertyField(pivot);

                    var createNormalMapProp = serializedObject.FindProperty("_createNormalMap");
                    EditorGUILayout.PropertyField(createNormalMapProp);

                    var isPixelateProp = serializedObject.FindProperty("_pixelated");
                    EditorGUILayout.PropertyField(isPixelateProp);

                    //var createAutoMaterial = serializedObject.FindProperty("_createAutoMaterial");
                    //EditorGUILayout.PropertyField(createAutoMaterial);

                    GUILayout.Space(5);

                    var spriteSavePath = serializedObject.FindProperty("_spriteSavePath");
                    /*var materialSavePath = serializedObject.FindProperty("_materialSavePath");

                    if (createAutoMaterial.boolValue == true)
                    {
                        if (GUILayout.Button("Change Material Export Location", GUILayout.Height(25)))
                        {
                            materialSavePath.stringValue = EditorUtility.OpenFolderPanel("Material Export Location", "", "");
                        }

                        if (materialSavePath.stringValue != "")
                            EditorGUILayout.LabelField(materialSavePath.stringValue, EditorStyles.miniLabel);
                    }*/

                    if (GUILayout.Button("Change Sprite Export Location", GUILayout.Height(25)))
                    {
                        spriteSavePath.stringValue = EditorUtility.OpenFolderPanel("Sprite Export Location", "", "");
                    }

                    if (spriteSavePath.stringValue != "")
                        EditorGUILayout.LabelField(spriteSavePath.stringValue, EditorStyles.miniLabel);


                    if (GUILayout.Button("Capture", GUILayout.Height(25)))
                    {
                        if (useAnimation.boolValue)
                        {
                            for (int i = 0; i < sourceClipsProp.arraySize; i++)
                            {
                                RunRoutine(helper.CaptureAnimation(SaveCapture));
                            }
                        }
                        else
                        {
                            RunRoutine(helper.CaptureFrame(SaveCapture));
                        }
                    }

                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width * 0.78f));
                    GUI.DrawTexture(GUILayoutUtility.GetLastRect(), previewImage);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(0);
                }

                if (GUILayout.Button("Join The Community Discord Server", GUILayout.Height(40)))
                {
                    Application.OpenURL("https://discord.gg/ASkVNuet8K");
                }
                GUILayout.Space(20);

                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (GUILayout.Button("Join The Community Discord Server", GUILayout.Height(40)))
            {
                Application.OpenURL("https://discord.gg/ASkVNuet8K");
            }
            GUILayout.Space(20);

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void InitiateBanner()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 76;

        GUI.DrawTexture(headerSection, headerTexture);

        GUILayout.Space(8);
        GUILayout.FlexibleSpace();
        GUILayout.Label(pixelateLogo, GUILayout.Width(175), GUILayout.Height(45));
        GUILayout.FlexibleSpace();
        GUILayout.Space(35);
    }

    private void RunRoutine(IEnumerator routine)
    {
        _currentCaptureRoutine = routine;
        EditorApplication.update += UpdateRoutine;
    }

    private void UpdateRoutine()
    {
        if (_currentCaptureRoutine != null)
        {
            if (!_currentCaptureRoutine.MoveNext())
            {
                EditorApplication.update -= UpdateRoutine;
                _currentCaptureRoutine = null;
            }
        }
    }

    private void SaveCapture(Texture2D diffuseMap, Texture2D normalMap, bool createNormalMap, bool useAnimation, bool pixelated, Vector2 cellSize, Vector2 pivot, AnimationClip animClip)
    {
        string setSpritePath = serializedObject.FindProperty("_spriteSavePath").stringValue;

        if (setSpritePath == "") {
            setSpritePath = Application.dataPath;
        }

        var diffusePath = setSpritePath;

        //if (setSpritePath == "")
        //    diffusePath = EditorUtility.SaveFilePanel("Save Capture", "", targetName, "png");
        //else
        //{
        //    diffusePath = EditorUtility.SaveFilePanel("Save Capture", setSpritePath, targetName, "png");
        //}

        if (string.IsNullOrEmpty(diffusePath))
        {
            return;
        }

        var clipName = animClip.name;
        clipName = clipName.Replace(" ", "");
        var fileName = targetName;
        var directory = diffusePath;
        var normalPath = "";

        if (useAnimation)
        {
            normalPath = string.Format("{0}/{1}_{2}_{3}.{4}", directory, fileName, clipName, "NormalMap", "png");
            diffusePath = string.Format("{0}/{1}_{2}.{3}", directory, fileName, clipName, "png");
        }
        else
        {
            normalPath = string.Format("{0}/{1}_{2}.{3}", directory, fileName, "NormalMap", "png");
            diffusePath = string.Format("{0}/{1}.{2}", directory, fileName, "png");
        }

        File.WriteAllBytes(diffusePath, diffuseMap.EncodeToPNG());
        
        if (createNormalMap)
            File.WriteAllBytes(normalPath, normalMap.EncodeToPNG());

        AssetDatabase.Refresh();

        var diffuseAssetDirectory = diffusePath.Replace(Application.dataPath, "Assets");
        Texture2D diffuseAsset = (Texture2D)AssetDatabase.LoadAssetAtPath(diffuseAssetDirectory, typeof(Texture2D));

        AutoSpriteSlicer.Slice(diffuseAsset, cellSize, pivot, useAnimation, pixelated);

        if (createNormalMap)
        {
            normalPath = diffusePath.Remove(diffusePath.Length - 4) + "_NormalMap.png";

            var normalAssetDirectory = normalPath.Replace(Application.dataPath, "Assets");
            Texture2D normalAsset = (Texture2D)AssetDatabase.LoadAssetAtPath(normalAssetDirectory, typeof(Texture2D));

            AutoSpriteSlicer.Slice(normalAsset, cellSize, pivot, useAnimation, pixelated);
        }
    }
}
