namespace LetConst.MasterData
{
    public class EditorPage
    {
        protected System.Action<UnityEditor.EditorWindow> onGUI;
        protected System.Action<bool>                     onToggled;

        /// <summary>ページの画面描画処理</summary>
        public System.Action<UnityEditor.EditorWindow> OnGUI => onGUI;

        /// <summary>ページを切り替えた際の処理</summary>
        public System.Action<bool> OnToggled => onToggled;
    }

    public sealed class EditorPage<T> : EditorPage where T : EditorPageBase<T>
    {
        public EditorPage(T page)
        {
            onGUI = page.OnGUI;

            if (page is IPageToggledEvent toggleable)
            {
                onToggled = toggleable.OnToggled;
            }
        }
    }
}
