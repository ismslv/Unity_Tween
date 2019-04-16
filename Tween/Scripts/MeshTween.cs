/* MESH TWEEN SYSTEM
 * STATES, AUTOMATIC TWEENS OF TWO STATES
 * V0.1
 * FMLHT, 04.2019
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMLHT {

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshTween : MonoBehaviour {

    public List<MeshTweenState> states;
    public bool useBetween = true;
    public bool recalculateNormals = false;
    public bool recalculateTangents = false;
    public bool recalculateBounds = true;
    //Vector3[] m_VertsBase;
    Vector3[] m_VertsCurrent;
    Mesh m_MorphedMesh;
    MeshFilter meshFilter;
    public BeTween between = new BeTween();

    [System.Serializable]
	public class MeshTweenState
	{
		public Mesh mesh;
        [HideInInspector]
        public Vector3[] Verts;
		[Range(-1, 1f)]
		public float Weight;

        public MeshTweenState(Mesh newmesh) {
            Weight = 0f;
            mesh = newmesh;
            this.Init();
        }

        public void Init() {
            if (mesh != null)
                Verts = mesh.vertices;
        }
	}

    [System.Serializable]
    public class BeTween {
        public int from;
        public int to;
        public float state;
    }
	
    void Awake() {
        Init();
    }

    void OnEnable() {
        Init();
    }

    public void Init() {
        InitObjects();
        InitStates();
        if (states.Count > 0) {
            if (useBetween)
                Between(between, false);
            UpdateStates();
        }
    }

    public void InitObjects() {
        if (states == null) {
            states = new List<MeshTweenState>();
        } else if (states.Count > 0) {
            for (int i = states.Count - 1; i >= 0; i--) {
                if (states[i].mesh == null)
                    states.RemoveAt(i);
            }
        }
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        m_MorphedMesh = new Mesh();
        m_MorphedMesh.MarkDynamic();
        meshFilter.mesh = m_MorphedMesh;
    }

    public void InitStates() {
        if (states == null || states.Count == 0) return;
        m_MorphedMesh.vertices = states[0].mesh.vertices;
        m_MorphedMesh.triangles = states[0].mesh.triangles;
        m_MorphedMesh.uv = states[0].mesh.uv;
        m_MorphedMesh.uv2 = states[0].mesh.uv2;
        m_MorphedMesh.normals = states[0].mesh.normals;
        m_MorphedMesh.tangents = states[0].mesh.tangents;
        m_MorphedMesh.bounds = states[0].mesh.bounds;

        for (int s = 0; s < states.Count; s++) {
            states[s].Verts = states[s].mesh.vertices;
        }

        m_VertsCurrent = new Vector3[states[0].mesh.vertices.Length];
    }

    public void UpdateStates()
    {
        if (states == null || states.Count == 0) return;
        m_VertsCurrent = states[0].mesh.vertices;
        for (int s = 0; s < states.Count; s++) {
            for (int v = 0; v < m_VertsCurrent.Length; v++) {
                if (s == 0)
                    m_VertsCurrent[v] = Vector3.zero;
                m_VertsCurrent[v] += states[s].Verts[v] * states[s].Weight;
            }
        }

		m_MorphedMesh.vertices = m_VertsCurrent;

        if (recalculateBounds)
		    m_MorphedMesh.RecalculateBounds();

		if (recalculateNormals)
			m_MorphedMesh.RecalculateNormals();
        
        if (recalculateTangents)
            m_MorphedMesh.RecalculateTangents();
    }

    public void SetState(int state, float val, bool setAll = false, bool update = true) {
        if (states == null) return;
        if (state > states.Count - 1) return;
        if (!setAll) {
            states[state].Weight = val;
        } else {
            for (int i = 0; i < states.Count; i++) {
                states[state].Weight = i == state ? val : 0f;
            }
        }
        if (update)
            UpdateStates();
    }

    public void Between(BeTween between_, bool update = true) {
        Between(between_.from, between_.to, between_.state, update);
    }

    public void Between(int A, int B, float T, bool update = true) {
        for (int i = 0; i < states.Count; i++) {
            if (i == A) {
                states[i].Weight = 1f - T;
            } else if (i == B) {
                states[i].Weight = T;
            } else {
                states[i].Weight = 0f;
            }
        }
        if (update)
            UpdateStates();
    }
}
}