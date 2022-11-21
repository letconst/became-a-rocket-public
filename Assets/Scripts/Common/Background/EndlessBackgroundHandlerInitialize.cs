using UnityEngine;
using UnityEngine.Assertions;

public partial class EndlessBackgroundHandler
{
    private void Initialize()
    {
        // backgroundSpriteのGameObjectとしての距離的な長さを取得
        _bgSpriteYLength = backgroundSprite.bounds.size.y;

        // 通常背景の上端の位置を取得
        _normalBgTopPos = normalBackground.transform.TransformPoint(normalBackground.sprite.bounds.max).y;

        // 画面中央から画面端までの距離を取得
        _screenYLength = VectorUtility.GetScreenLength(_cam).y;

        // 背景の生成数を算出
        int generateCount = Mathf.CeilToInt(_screenYLength / _bgSpriteYLength);

        // 最低4つは生成するように
        generateCount = Mathf.Clamp(generateCount, 4, generateCount);

        Transform bgParent = new GameObject("EndlessBackgrounds").transform;

        var normalBg = new EndlessBackground(normalBackground);
        _topmostBg = normalBg;
        _lowestBg  = normalBg;

        for (int i = 0; i < generateCount; i++)
        {
            // オブジェクト生成
            (GameObject obj, SpriteRenderer renderer) newBg    = GenerateBackgroundSprite($"Bg_{i}", backgroundSprite);
            Transform                                 newBgTrf = newBg.obj.transform;
            newBgTrf.SetParent(bgParent);

            EndlessBackground prevBg = null;

            if (i > 0)
            {
                prevBg = _backgrounds[i - 1];
            }

            // 識別用インスタンス作成
            var newBgInstance = new EndlessBackground(newBg.renderer, prev: prevBg);
            _backgrounds.Add(newBgInstance);

            if (prevBg != null)
            {
                prevBg.Next = newBgInstance;
            }

            // 座標設定
            SetPositionToTopmost(newBgInstance);

            if (i == 0)
            {
                _lowestBg = newBgInstance;
            }
        }
    }

    private (GameObject obj, SpriteRenderer renderer) GenerateBackgroundSprite(string objectName, Sprite sprite)
    {
        Assert.IsNotNull(sprite, "sprite != null");

        var newBackground = new GameObject(objectName);
        var newRenderer   = newBackground.AddComponent<SpriteRenderer>();
        newRenderer.sprite       = sprite;
        newRenderer.sortingOrder = GameConstants.BackgroundOrderInLayer;

        // TODO: PPUに合わせてスケーリングさせる。通常背景と幅・PPUが合っていれば一応崩れない

        return (newBackground, newRenderer);
    }
}
