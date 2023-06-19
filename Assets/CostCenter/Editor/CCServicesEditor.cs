using UnityEngine;
using UnityEditor;

namespace CostCenter.GUI {
    [CustomEditor(typeof(CCServices))]
    public class CCServicesEditor : Editor
    {
        bool IsFoldout = false;

        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector();
            CCServices manager = (CCServices)target;

            IsFoldout = EditorGUILayout.Foldout(IsFoldout, "Add Services");

            if (IsFoldout) {
                if (GUILayout.Button("Add Attribution")) {
                    manager.AddAttribution();
                    EditorUtility.SetDirty(manager);
                }
            }
        }
    }
}
