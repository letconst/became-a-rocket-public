using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

public static class InputActionsExtension
{
    /// <summary>
    /// <see cref="InputActions"/> の有効化等をGameObjectのイベントに紐づかせる
    /// </summary>
    /// <param name="source"></param>
    /// <param name="gameObjectComponent"></param>
    public static void BindEvent(this InputActions source, Component gameObjectComponent)
    {
        Assert.IsNotNull(source, "source != null");
        Assert.IsNotNull(gameObjectComponent, "gameObjectComponent != null");

        gameObjectComponent.OnEnableAsObservable()
                           .Subscribe(_ => source.Enable())
                           .AddTo(gameObjectComponent);

        gameObjectComponent.OnDisableAsObservable()
                           .Subscribe(_ => source.Disable())
                           .AddTo(gameObjectComponent);

        gameObjectComponent.OnDestroyAsObservable()
                           .Subscribe(_ => source.Dispose())
                           .AddTo(gameObjectComponent);
    }
}
