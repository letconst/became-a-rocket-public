using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace LetConst.MasterData
{
    public sealed class MasterDataEditor : EditorWindow
    {
        private static EditorWindow _window;

        private static readonly Dictionary<RenderPage, EditorPage> _renderPageDict = new()
        {
            { RenderPage.Home, new EditorPage<HomeEditorPage>(HomeEditorPage.Instance) },
            { RenderPage.Player, new EditorPage<PlayerEditorPage>(PlayerEditorPage.Instance) },
            { RenderPage.Gimmick, new EditorPage<GimmickEditorPage>(GimmickEditorPage.Instance) },
            { RenderPage.GimmickGeneration, new EditorPage<GimmickGenerationEditorPage>(GimmickGenerationEditorPage.Instance) },
        };

        private static readonly EditorPageManager _pageManager = new(_renderPageDict);

        private static readonly CompositeDisposable _compositeDisposable = new();
        private static          MessageBroker       _broker              = new();

        /// <summary>MasterDataEditor用のMessageBroker</summary>
        public static IMessagePublisher Broker => _broker;

        public static RenderPage CurrentPage => _pageManager.CurrentPage.Value;

        /// <summary>
        /// <see cref="MasterDataEditor"/> のEditorWindow
        /// </summary>
        private static EditorWindow Window => _window ? _window : _window = GetWindow<MasterDataEditor>();

        [MenuItem("Tools/マスターデータエディター")]
        private static void ShowWindow()
        {
            _window              = GetWindow<MasterDataEditor>();
            _window.titleContent = new GUIContent(EditorConstants.ToolName);
            _window.minSize = new Vector2(16 * EditorConstants.WindowSizeRatio,
                                          9  * EditorConstants.WindowSizeRatio);

            _window.Show();
        }

        private void OnEnable()
        {
            _broker ??= new MessageBroker();
            _broker.AddTo(_compositeDisposable);

            EventReceiver();
        }

        private void OnDisable()
        {
            // 関連するDisposableをDispose (.Dispose()だと再利用不可)
            _compositeDisposable.Clear();
            _broker = null;
        }

        private void OnGUI()
        {
            if (!_renderPageDict.ContainsKey(_pageManager.CurrentPage.Value))
            {
                // 現在のページ用のGUIがなければ直前ページに戻す
                _pageManager.CurrentPage.Value = _pageManager.PrevPage;

                return;
            }

            // 現在のページに応じた内容を表示
            _renderPageDict[_pageManager.CurrentPage.Value]?.OnGUI?.Invoke(Window);
        }

        private void EventReceiver()
        {
            // ページ設定イベント受付
            _broker.Receive<MasterDataEvent.SetPage>()
                   .Subscribe(data =>
                   {
                       // 描画処理対象のページにのみ遷移する
                       if (!_renderPageDict.ContainsKey(data.NewPage))
                           return;

                       // GUI描画処理のあるページにのみ遷移する
                       if (_renderPageDict[data.NewPage].OnGUI == null)
                           return;

                       _pageManager.CurrentPage.Value = data.NewPage;
                   })
                   .AddTo(_compositeDisposable);
        }
    }
}
