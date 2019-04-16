using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FMLHT {

public class TweenEditor : Editor
{
    [MenuItem("FMLHT/Utilities/Tween/Add to scene")]
    public static void AddPrefab() {
        if (Editor.FindObjectOfType<Tween>() == null) {
            var core = GameObject.Find("Core");
            if (core == null) {
                core = new GameObject();
                core.name = "Core";
            }
            var utilities = core.transform.Find("Utilities");
            if (utilities == null) {
                utilities = new GameObject().transform;
                utilities.name = "Utilities";
            }
            utilities.SetParent(core.transform);
            utilities.gameObject.AddComponent<Tween>();
        } else {
            Debug.Log("There is already one Tween in this scene!");
        }
    }
}

}