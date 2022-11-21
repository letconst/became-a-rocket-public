using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public sealed class BackgroundImageHandler : MonoBehaviour
{
    [SerializeField, Header("生成する背景画像の設定")]
    private BackgroundImage[] backgroundImages;

    [SerializeField, Header("初期生成される高さのオフセット")]
    private float offsetY;

    [SerializeField, Header("背景の切り替わり時のBGMクロスフェードが行われるまでのプレイヤー座標のオフセット")]
    private int startCrossFadeOffset;

    private Transform _parent;

    private float _backgroundLength;
    private float _halfBackgroundLength;

    private BackgroundSoundHandler _soundHandler;

    private async UniTaskVoid Start()
    {
        _parent = new GameObject("Backgrounds").transform;

        // 背景画像の高さを取得
        if (backgroundImages.Length != 0 && backgroundImages[0].FadeImageForNext != null)
        {
            _backgroundLength     = backgroundImages[0].FadeImageForNext.bounds.size.y;
            _halfBackgroundLength = _backgroundLength / 2f;
        }

        await UniTask.DelayFrame(3);
        await SoundManager.Instance.WaitForReady();

        _soundHandler = new BackgroundSoundHandler(SoundManager.Instance, ScoreManager.Instance, startCrossFadeOffset);
        _soundHandler.AddTo(this);

        Generate();
    }

    private void Generate()
    {
        float lastGenerateY = offsetY;

        // 背景画像を生成
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            BackgroundImage image       = backgroundImages[i];
            float           totalLength = image.GenerateMaxRange - image.GenerateMinRange - _halfBackgroundLength;

            // 初回生成のみオフセット適用
            if (i == 0)
            {
                totalLength -= offsetY;
            }

            // メイン背景の生成
            SpriteRenderer mainRenderer = GenerateSpriteRenderer(image.MainImage, $"Background{i}");
            mainRenderer.drawMode           = SpriteDrawMode.Tiled;
            mainRenderer.size               = new Vector2(_backgroundLength, totalLength);
            mainRenderer.transform.position = new Vector3(0f, lastGenerateY);

            lastGenerateY += totalLength;

            if (!image.FadeImageForNext)
                return;

            // 次の背景へのフェード部分生成
            SpriteRenderer fadeRenderer = GenerateSpriteRenderer(image.FadeImageForNext, $"Background{i}.5");
            fadeRenderer.transform.position = new Vector3(0f, lastGenerateY);

            lastGenerateY += _backgroundLength;

            // 背景移り変わり時の次のBGMを登録
            MusicDef nextMusic = i switch
            {
                0 => MusicDef.Stage2,
                1 => MusicDef.Stage3,
                2 => MusicDef.Stage3
            };

            _soundHandler.RegisterMusicSwitching(image.GenerateMaxRange, nextMusic);
        }
    }

    private SpriteRenderer GenerateSpriteRenderer(Sprite image, string objectName)
    {
        var newBg            = new GameObject(objectName) { transform = { parent = _parent } };
        var rendererToReturn = newBg.AddComponent<SpriteRenderer>();
        rendererToReturn.sprite       = image;
        rendererToReturn.sortingOrder = GameConstants.BackgroundOrderInLayer;

        return rendererToReturn;
    }
}
