using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

namespace FMLHT {

[CustomEditor(typeof(MeshTween))]
public class MeshTweenEditor : Editor
{
    MeshTween meshTween;
    int statesCount = 0;
    Mesh newMesh;

    public override void OnInspectorGUI() {
        meshTween = (MeshTween)target;
        GUIStyle styleTitle = new GUIStyle("label") {
            fontStyle = FontStyle.Bold,
            fontSize = 12
        };

        GUILayout.Space(20f);
        GUILayout.Label("States", styleTitle);
        EditorGUI.BeginChangeCheck();
        //DrawDefaultInspector();
        
        if (meshTween.states == null)
            meshTween.states = new List<MeshTween.MeshTweenState>();
        if (meshTween.states.Count >= 0) {
            int lineLimit = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / 90f);
            int limitCount = 0;
            for (int i = 0; i <= meshTween.states.Count; i++) {
                if (limitCount == 0)
                    GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.MaxWidth(80f));
                if (i < meshTween.states.Count) {
                    GUILayout.Label("State " + (i + 1));
                    meshTween.states[i].mesh = (Mesh)EditorGUILayout.ObjectField(meshTween.states[i].mesh, typeof(Mesh), true, GUILayout.MaxWidth(80f));
                    GUILayout.Label(AssetPreview.GetAssetPreview(meshTween.states[i].mesh), GUILayout.MaxWidth(80f), GUILayout.MaxHeight(80f));
                    Rect r = GUILayoutUtility.GetLastRect();
                    r.y += 80;
                    r.height = 30f;
                    meshTween.states[i].Weight = GUI.HorizontalSlider(r, meshTween.states[i].Weight, -1f, 1f);
                    GUILayout.Space(20f);
                } else {
                    GUILayout.Label("New");
                    newMesh = (Mesh)EditorGUILayout.ObjectField(newMesh, typeof(Mesh), true, GUILayout.MaxWidth(80f));
                    GUILayout.Label(newMesh != null ? AssetPreview.GetAssetPreview(newMesh) : new Texture2D(80, 80), GUILayout.MaxWidth(80f), GUILayout.MaxHeight(80f));
                    GUILayout.Space(20f);
                    if (newMesh != null) {
                        meshTween.states.Add(new MeshTween.MeshTweenState(newMesh));
                        newMesh = null;
                    }
                }
                GUILayout.EndVertical();
                limitCount++;
                if (i == meshTween.states.Count || limitCount == lineLimit) {
                    GUILayout.EndHorizontal();
                    limitCount = 0;
                }
            }
        }
        GUILayout.Space(20f);
        if (EditorGUI.EndChangeCheck()) {
            if (statesCount != meshTween.states.Count) {
                meshTween.Init();
                statesCount = meshTween.states.Count;
            }
            meshTween.UpdateStates();
            EditorUtility.SetDirty(meshTween);
            SavePrefab();
        }
        if (meshTween.states.Count >= 2) {
            GUILayout.Label("Tween", styleTitle);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            meshTween.useBetween = EditorGUILayout.Toggle(meshTween.useBetween, GUILayout.MaxWidth(30f));
            if (meshTween.useBetween) {
                var states_ = new string[meshTween.states.Count];
                for (int i = 0; i < meshTween.states.Count; i++)
                    states_[i] = (i + 1).ToString();
                meshTween.between.from = Mathf.Clamp(meshTween.between.from, 0, meshTween.states.Count - 2);
                meshTween.between.from = EditorGUILayout.Popup(meshTween.between.from, states_, GUILayout.MaxWidth(30f));
                meshTween.between.state = EditorGUILayout.Slider(meshTween.between.state, 0f, 1f);
                meshTween.between.to = Mathf.Clamp(meshTween.between.to, meshTween.between.from + 1, meshTween.states.Count - 1);
                meshTween.between.to = EditorGUILayout.Popup(meshTween.between.to, states_, GUILayout.MaxWidth(30f));
                if (EditorGUI.EndChangeCheck()) {
                    meshTween.Between(meshTween.between);
                    EditorUtility.SetDirty(meshTween);
                    SavePrefab();
                }
            }
            GUILayout.EndHorizontal();
        }
        if (meshTween.states.Count >= 1) {
            GUILayout.Space(20f);
            if (GUILayout.Button("Reinitialize Meshes")) {
                meshTween.Init();
            }
        }
    }

    void SavePrefab() {
        var prefabStage = PrefabStageUtility.GetPrefabStage(meshTween.gameObject);
        if (prefabStage != null) EditorSceneManager.MarkSceneDirty(prefabStage.scene);
    }
}
}