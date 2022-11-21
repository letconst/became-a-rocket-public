using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ResultModel : MonoBehaviour
{
    /// <summary>
    /// リトライボタンクリック時の処理
    /// </summary>
    public void OnRetryClicked()
    {
        var dataPack = new ToMainGameSceneDataPack(GameScene.MainGame);

        SceneManager.Instance.LoadScene(GameScene.MainGame, .5f, .5f, dataPack);
    }

    /// <summary>
    /// タイトルに戻るボタンクリック時の処理
    /// </summary>
    public void OnTitleClicked()
    {
        SceneManager.Instance.LoadScene(GameScene.Title);
    }

    /// <summary>
    /// リザルトUIを表示する
    /// </summary>
    /// <param name="canvasGroup">対象のリザルトUIのCanvasGroup</param>
    /// <param name="fadeTime">UIをフェード表示する時間 (秒)</param>
    public void ShowResult(CanvasGroup canvasGroup, float fadeTime)
    {
        FadeTransition.FadeOut(canvasGroup, fadeTime).Forget();
    }
}
