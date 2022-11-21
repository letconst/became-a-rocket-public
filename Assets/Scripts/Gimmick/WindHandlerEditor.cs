#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public sealed partial class WindHandler
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WindHandler))]
    private class Editor : UnityEditor.Editor
    {
        private WindHandler _instance;

        private void OnEnable()
        {
            _instance = (WindHandler) target;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(_instance, "");
            EditorGUI.BeginChangeCheck();

            // Sprite
            _instance.windSprite =
                EditorGUILayout.ObjectField("自身のSprite", _instance.windSprite, typeof(SpriteRenderer), true) as SpriteRenderer;

            // 風向き指定
            _instance.windDirection = (WindDirection) EditorGUILayout.EnumPopup("風向き", _instance.windDirection);

            EditorGUILayout.Space();

            // 回転速度の上書き指定
            _instance.isOverrideRotateSpeed =
                EditorGUILayout.BeginToggleGroup("プレイヤーに与える回転速度を上書き", _instance.isOverrideRotateSpeed);

            _instance.overrideRotateSpeed = EditorGUILayout.FloatField("回転速度", _instance.overrideRotateSpeed);

            if (GUILayout.Button("リセット"))
            {
                _instance.overrideRotateSpeed = GameConstants.RotateSpeedByWind;
            }

            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space();

            // 移動速度の上書き指定
            _instance.isOverrideMoveSpeed = EditorGUILayout.BeginToggleGroup("プレイヤーに与える移動速度を上書き", _instance.isOverrideMoveSpeed);
            _instance.overrideMoveSpeed   = EditorGUILayout.FloatField("移動速度", _instance.overrideMoveSpeed);

            if (GUILayout.Button("リセット"))
            {
                _instance.overrideMoveSpeed = GameConstants.MoveSpeedByWind;
            }

            EditorGUILayout.EndToggleGroup();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_instance);
            }
        }
    }
#endif
}
