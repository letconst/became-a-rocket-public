using System.Collections.Generic;
using UniRx;

namespace LetConst.MasterData
{
    public sealed class EditorPageManager
    {
        private readonly IReadOnlyDictionary<RenderPage, EditorPage> _pages;

        private readonly System.IDisposable           _onToggleDisposable;
        private readonly ReactiveProperty<RenderPage> _currentPage = new(RenderPage.Home);

        public IReactiveProperty<RenderPage> CurrentPage => _currentPage;

        public RenderPage PrevPage { get; private set; } = RenderPage.Home;

        public EditorPageManager(IReadOnlyDictionary<RenderPage, EditorPage> pages)
        {
            _pages = pages;

            // ページ切り替え時のイベント登録
            _onToggleDisposable = _currentPage.Skip(1).DistinctUntilChanged().Subscribe(OnPageToggled);
        }

        ~EditorPageManager()
        {
            // デストラクト時にSubscribeも破棄
            _onToggleDisposable.Dispose();
        }

        private void OnPageToggled(RenderPage openedPage)
        {
            // 同じページへの変更なら実行しない
            if (PrevPage == openedPage)
                return;

            // 切り替え前のページの切り替え処理実行
            if (_pages.ContainsKey(PrevPage))
            {
                _pages[PrevPage]?.OnToggled?.Invoke(false);
            }

            // 切り替え先のページの切り替え処理実行
            if (_pages.ContainsKey(openedPage))
            {
                _pages[openedPage]?.OnToggled?.Invoke(true);
                PrevPage = openedPage;
            }
            else
            {
                _currentPage.Value = PrevPage;
            }
        }
    }
}
