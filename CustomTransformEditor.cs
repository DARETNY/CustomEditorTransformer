using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    [RootEditor]
    public class CustomTransformEditor : UnityEditor.Editor
    {
        private Vector3[] _positions;
        private Quaternion[] _rotations;
        private Vector3[] _scales;

        private const string PositionPrefsKey = "CustomTransformEditor_Position";
        private const string RotationPrefsKey = "CustomTransformEditor_Rotation";
        private const string ScalePrefsKey = "CustomTransformEditor_Scale";

        private void OnEnable()
        {
            // Load the positions and rotations from editor preferences
            _positions = new Vector3[targets.Length];
            _rotations = new Quaternion[targets.Length];
            _scales = new Vector3[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                var savedPosition = EditorPrefs.GetString(PositionPrefsKey + i);
                if (!string.IsNullOrEmpty(savedPosition))
                {
                    _positions[i] = JsonUtility.FromJson<Vector3>(savedPosition);
                }

                var savedRotation = EditorPrefs.GetString(RotationPrefsKey + i);
                if (!string.IsNullOrEmpty(savedRotation))
                {
                    _rotations[i] = JsonUtility.FromJson<Quaternion>(savedRotation);
                }

                var savedScale = EditorPrefs.GetString(ScalePrefsKey + i);
                if (!string.IsNullOrEmpty(savedScale))
                {
                    _scales[i] = JsonUtility.FromJson<Vector3>(savedScale);
                }
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                var savedPosition = JsonUtility.ToJson(_positions[i]);
                EditorPrefs.SetString(PositionPrefsKey + i, savedPosition);

                var savedRotation = JsonUtility.ToJson(_rotations[i]);
                EditorPrefs.SetString(RotationPrefsKey + i, savedRotation);

                var savedScale = JsonUtility.ToJson(_scales[i]);
                EditorPrefs.SetString(ScalePrefsKey + i, savedScale);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            var transformTargets = new Transform[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                transformTargets[i] = (Transform)targets[i];
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Current Position"))
            {
                for (int i = 0; i < transformTargets.Length; i++)
                {
                    transformTargets[i].GetPositionAndRotation(out _positions[i], out _rotations[i]);
                    _scales[i] = transformTargets[i].localScale;
                    Debug.Log("<color=green>Data saved</color>", transformTargets[i].gameObject);
                }
            }

            if (GUILayout.Button("Load Saved Position"))
            {
                for (int i = 0; i < transformTargets.Length; i++)
                {
                    transformTargets[i].localScale = _scales[i];
                    transformTargets[i].SetPositionAndRotation(_positions[i], _rotations[i]);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (GUILayout.Button("Reset All"))
            {
                for (int i = 0; i < transformTargets.Length; i++)
                {
                    transformTargets[i].localPosition = Vector3.zero;
                    transformTargets[i].localRotation = Quaternion.identity;
                    transformTargets[i].localScale = Vector3.one;
                }
            }
        }
    }
}