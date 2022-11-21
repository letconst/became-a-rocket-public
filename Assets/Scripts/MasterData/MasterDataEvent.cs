#if UNITY_EDITOR
namespace LetConst.MasterData
{
    public static class MasterDataEvent
    {
        /// <summary>表示ページを設定するイベント</summary>
        public sealed class SetPage : EventMessage<SetPage, RenderPage>
        {
            /// <summary>設定するページ</summary>
            public RenderPage NewPage => param1;
        }
    }
}
#endif
