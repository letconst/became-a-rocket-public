using UnityEditor;

namespace LetConst.MasterData
{
    public sealed class GimmickGenerationEditorPage : EditorPageBase<GimmickGenerationEditorPage>, IPageToggledEvent
    {
        private Editor                  _editor;
        private MasterGimmickGeneration _data;

        public override void OnGUI(EditorWindow window)
        {
            EditorUtility.SideMenuGUI(window, MasterDataEditor.CurrentPage, MasterDataEditor.Broker);
            EditorUtility.PanelGUI(window, LeftContent);

            void LeftContent()
            {
                if (!_data) return;

                _editor ??= Editor.CreateEditor(_data);

                _editor.OnInspectorGUI();
            }
        }

        public void OnToggled(bool isOpened)
        {
            if (isOpened)
            {
                _data = EditorUtility.GetMasterDataFile<MasterGimmickGeneration>(EditorConstants.Path.GimmickGenerationDataPath);
            }
            else
            {
                _data = null;
            }
        }
    }
}
