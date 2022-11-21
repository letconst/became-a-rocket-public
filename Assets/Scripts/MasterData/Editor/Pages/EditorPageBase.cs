using UnityEditor;

namespace LetConst.MasterData
{
    public abstract class EditorPageBase<T> : SingletonBase<T> where T : class
    {
        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="window"><see cref="EditorWindow"/>インスタンス</param>
        public abstract void OnGUI(EditorWindow window);
    }
}
