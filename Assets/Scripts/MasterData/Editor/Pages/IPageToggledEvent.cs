namespace LetConst.MasterData
{
    public interface IPageToggledEvent
    {
        /// <summary>
        /// ページを切り替えた際の処理
        /// <param name="isOpened">ページを開いたときか</param>
        /// </summary>
        void OnToggled(bool isOpened);
    }
}
