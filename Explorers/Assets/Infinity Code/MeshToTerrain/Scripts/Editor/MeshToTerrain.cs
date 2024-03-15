/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.MeshToTerrain
{
    public partial class MeshToTerrain : EditorWindow
    {
        public const string version = "2.4.0.1";

        public static MeshToTerrainPhase phase;

        private static int activeIndex;
        private static float progress;
        private int lastX;
        private Vector3 maxBounds = Vector3.zero;
        private Vector3 minBounds = Vector3.zero;
        private MeshToTerrainPrefs prefs = new MeshToTerrainPrefs();
        private string resultFolder;
        private static Transform lastTransform;
        private static Mesh lastMesh;
        private static MeshToTerrain wnd;
        private GameObject meshContainer;
        private Vector3 boundsRange;
        private Vector3 meshScale;
        private Vector3 originalBoundsRange;

        private void CancelConvert()
        {
            phase = MeshToTerrainPhase.idle;
            if (container != null) DestroyImmediate(container);
            Dispose();
        }

        private void ConvertUI()
        {
            string phaseTitle = "";
            if (phase == MeshToTerrainPhase.prepare) phaseTitle = "Preparing.";
            else if (phase == MeshToTerrainPhase.createTerrains) phaseTitle = "Create terrains.";
            else if (phase == MeshToTerrainPhase.generateHeightmaps) phaseTitle = "Generate heightmaps.";
            else if (phase == MeshToTerrainPhase.generateTextures) phaseTitle = "Generate textures.";
            else if (phase == MeshToTerrainPhase.finish) phaseTitle = "Finishing.";

            GUILayout.Label(phaseTitle);

            Rect r = EditorGUILayout.BeginVertical();
            if (phase == MeshToTerrainPhase.generateHeightmaps || phase == MeshToTerrainPhase.generateTextures || phase == MeshToTerrainPhase.createTerrains)
            {
                r.height = 16;
                int iProgress = Mathf.FloorToInt(progress * 100);
                EditorGUI.ProgressBar(r, progress, iProgress + "%");
                GUILayout.Space(18);
            }
            else
            {
                GUILayout.Space(38);
            }

            if (GUILayout.Button("Cancel")) CancelConvert();

            EditorGUILayout.EndVertical();
        }

        private void DisplayDialog(string msg)
        {
            EditorUtility.DisplayDialog("Error", msg, "OK");
        }

        public static Object FindAndLoad(string filename, Type type)
        {
            string[] files = Directory.GetFiles("Assets", filename, SearchOption.AllDirectories);
            if (files.Length > 0) return AssetDatabase.LoadAssetAtPath(files[0], type);
            return null;
        }

        private static double GetHitPoint(RaycastHit hit, Vector3 curPoint)
        {
            if (lastTransform != hit.transform)
            {
                lastTransform = hit.transform;
                lastMesh = hit.collider.GetComponent<MeshFilter>().sharedMesh;
                lastTriangles = lastMesh.triangles;
                lastVerticles = lastMesh.vertices;
            }

            int ti = hit.triangleIndex * 3;

            int pi1 = lastTriangles[ti];
            int pi2 = lastTriangles[ti + 1];
            int pi3 = lastTriangles[ti + 2];

            Vector3 p1 = lastTransform.TransformPoint(lastVerticles[pi1]);
            Vector3 p2 = lastTransform.TransformPoint(lastVerticles[pi2]);
            Vector3 p3 = lastTransform.TransformPoint(lastVerticles[pi3]);

            Vector3 a = new Vector3(p1.x, 0, p1.z);
            Vector3 b = new Vector3(p2.x, 0, p2.z);
            Vector3 c = new Vector3(p3.x, 0, p3.z);
            Vector3 d = new Vector3(curPoint.x, 0, curPoint.z);

            float ad = (a - d).magnitude;
            float bd = (b - d).magnitude;
            float cd = (c - d).magnitude;
            float ab = (a - b).magnitude;
            float ac = (a - c).magnitude;
            float bc = (b - c).magnitude;

            float wa = bd * cd / (ab * ac);
            float wb = ad * cd / (ab * bc);
            float wc = ad * bd / (ac * bc);

            return (p1 * wa + p2 * wb + p3 * wc).y;
        }

        private void OnDestroy()
        {
            if (prefs != null) prefs.Save();
        }

        private void OnDisable()
        {
            if (prefs != null) prefs.Save();
        }

        private void OnEnable()
        {
            wnd = this;
            prefs = new MeshToTerrainPrefs();
            MeshToTerrainPrefs.meshesChecked = false;
            prefs.Init();
            Repaint();
        }

        private void OnGUI()
        {
            if (phase == MeshToTerrainPhase.idle) prefs.OnGUI();
            else ConvertUI();
        }

        [MenuItem(MeshToTerrainPrefs.MenuPath + "Mesh to Terrain", false, 0)]
        private static void OpenWindow()
        {
            wnd = GetWindow<MeshToTerrain>(false, "Mesh to Terrain");
            Rect rect = wnd.position;

            if (rect.width < 500) rect.width = 500;
            if (rect.height < 400) rect.height = 400;
            rect.x = rect.y = 100;
            wnd.position = rect;
        }

        [MenuItem("GameObject/Convert Mesh to Terrain", false, 40)]
        private static void OpenWindowWithGO()
        {
            List<GameObject> gameObjects = Selection.gameObjects.Where(g => g.GetComponentsInChildren<MeshFilter>().Length > 0).ToList();
            OpenWindow();

            wnd.prefs.meshes = gameObjects;
        }

        [MenuItem("GameObject/Convert Mesh to Terrain", true, 40)]
        private static bool OpenWindowWithGOValidation()
        {
            MeshFilter[] filters = Selection.gameObjects.SelectMany(g => g.GetComponentsInChildren<MeshFilter>()).ToArray();
            return filters.Length > 0;
        }

        public static void RepaintWindow()
        {
            wnd.Repaint();
        }

        private void Update()
        {
            if (phase == MeshToTerrainPhase.idle)
            {
                if (EditorApplication.isCompiling && prefs.showBoundSelector)
                {
                    prefs.showBoundSelector = false;

                    if (prefs.boundsHelper != null)
                    {
                        prefs.boundsHelper.OnBoundChanged = null;
                        prefs.boundsHelper.OnDestroyed = null;
                        DestroyImmediate(prefs.boundsHelper.gameObject);
                        prefs.boundsHelper = null;
                    }
                }

                return;
            }

            if (phase == MeshToTerrainPhase.prepare) Prepare();
            else if (phase == MeshToTerrainPhase.createTerrains) CreateTerrain(activeIndex);
            else if (phase == MeshToTerrainPhase.generateHeightmaps) UpdateTerrain(prefs.terrains[activeIndex]);
            else if (phase == MeshToTerrainPhase.prepareTextures) PrepareTexture();
            else if (phase == MeshToTerrainPhase.generateTextures) UpdateTexture(prefs.terrains[activeIndex]);
            else if (phase == MeshToTerrainPhase.finish) Finish();

            Repaint();
        }
    }
}