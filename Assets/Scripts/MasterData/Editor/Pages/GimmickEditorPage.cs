using UnityEditor;
using UnityEngine;

namespace LetConst.MasterData
{
    public sealed class GimmickEditorPage : EditorPageBase<GimmickEditorPage>, IPageToggledEvent
    {
        private Editor          _editor;
        private MasterGimmick[] _data;
        private MasterGimmick   _targetData;

        public override void OnGUI(EditorWindow window)
        {
            EditorUtility.SideMenuGUI(window, MasterDataEditor.CurrentPage, MasterDataEditor.Broker);
            EditorUtility.PanelGUI(window, LeftContent, RightContent);

            void LeftContent()
            {
                // Debug.Log(_data);

                if (_data == null) return;

                foreach (MasterGimmick data in _data)
                {
                    if (!data) continue;

                    if (GUILayout.Button($"{data.Type.ToString()} ({data.Id})", GUIStyles.EnemyListButtonStyle))
                    {
                        _targetData = data;
                    }
                }
            }

            void RightContent()
            {
                if (!_targetData) return;

                _editor = Editor.CreateEditor(_targetData);

                _editor.OnInspectorGUI();
            }
        }

        public void OnToggled(bool isOpened)
        {
            if (isOpened)
            {
                _data = EditorUtility.GetMasterDataFiles<MasterGimmick>(EditorConstants.Path.GimmickDataDir);
            }
            else
            {
                System.Array.Clear(_data, 0, _data.Length);
            }
        }
    }
}
