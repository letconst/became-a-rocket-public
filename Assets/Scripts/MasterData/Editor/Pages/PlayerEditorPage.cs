using UnityEditor;

namespace LetConst.MasterData
{
    public sealed class PlayerEditorPage : EditorPageBase<PlayerEditorPage>, IPageToggledEvent
    {
        private static Editor       _playerEditor;
        private static MasterPlayer _playerData;

        public override void OnGUI(EditorWindow window)
        {
            EditorUtility.SideMenuGUI(window, MasterDataEditor.CurrentPage, MasterDataEditor.Broker);
            EditorUtility.PanelGUI(window, LeftContent);

            void LeftContent()
            {
                if (!_playerData) return;

                _playerEditor = Editor.CreateEditor(_playerData);
                _playerEditor.OnInspectorGUI();

            }
        }

        public void OnToggled(bool isOpened)
        {
            if (isOpened)
            {
                _playerData = EditorUtility.GetMasterDataFile<MasterPlayer>(EditorConstants.Path.PlayerDataPath);
            }
            else
            {
                _playerData = null;
            }
        }
    }
}
