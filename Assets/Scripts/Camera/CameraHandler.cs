using Cinemachine;
using UnityEngine;

/// <summary>
/// ギャー君を追従するカメラを制御するクラス
/// </summary>
public sealed class CameraHandler : CinemachineExtension
{
    [SerializeField, Header("ゲーム開始時のカメラ座標")]
    private Vector3 initialPosition;

    private float _originXPos;
    private float _prevYPos;
    private bool  _isPlayAreaReached;

    private void Start()
    {
        transform.position = initialPosition;
        _originXPos        = transform.position.x;
        _prevYPos          = initialPosition.y;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
                                                      ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 pos = state.RawPosition;

            // プレイヤーのy座標がカメラの初期座標以上なら追従
            if (vcam.Follow.position.y >= initialPosition.y || _isPlayAreaReached)
            {
                pos.x              = _originXPos;
                _isPlayAreaReached = true;

                // 1フレーム前より低いなら、カメラは動かさない
                if (vcam.Follow.position.y < _prevYPos)
                {
                    pos.y = _prevYPos;
                }
            }
            // 初期位置にいるときのみ、カメラを動かさない
            else if (!_isPlayAreaReached)
            {
                pos = initialPosition;
            }

            state.RawPosition = pos;
            _prevYPos         = pos.y;
        }
    }
}
