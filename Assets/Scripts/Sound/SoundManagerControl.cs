using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class SoundManager
{
    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="target">再生するBGM</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">ループするか</param>
    /// <param name="position">再生する位置</param>
    /// <returns>サウンドID</returns>
    public int PlayMusic(MusicDef target, float volume = 1, bool isLoop = false, Vector3? position = null)
    {
        return PlayAudio(Audio.AudioType.Music, (int) target, volume, isLoop, position);
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="target">再生するSE</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">ループするか</param>
    /// <param name="position">再生する位置</param>
    /// <returns>サウンドID</returns>
    public int PlaySound(SoundDef target, float volume = 1, bool isLoop = false, Vector3? position = null)
    {
        return PlayAudio(Audio.AudioType.Sound, (int) target, volume, isLoop, position);
    }

    /// <summary>
    /// UI用のSEを再生する
    /// </summary>
    /// <param name="target">再生するUI SE</param>
    /// <param name="volume">音量</param>
    /// <returns>サウンドID</returns>
    public int PlayUISound(UISoundDef target, float volume = 1)
    {
        return PlayAudio(Audio.AudioType.UISound, (int) target, volume, false, null);
    }

    /// <summary>
    /// サウンドを再生する
    /// </summary>
    /// <param name="type">再生するサウンドの種類</param>
    /// <param name="target">再生するサウンドのenumインデックス</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">ループするか</param>
    /// <param name="position">再生する位置</param>
    /// <returns>サウンドID</returns>
    public int PlayAudio(Audio.AudioType type, int target, float volume, bool isLoop, Vector3? position)
    {
        int id = -1;

        switch (type)
        {
            case Audio.AudioType.Music:
            {
                id = PrepareMusic(target, volume, isLoop, position);
                GetMusicAudio(id).Play();

                break;
            }

            case Audio.AudioType.Sound:
            {
                id = PrepareSound(target, volume, isLoop, position);
                GetSoundAudio(id).Play();

                break;
            }

            case Audio.AudioType.UISound:
            {
                id = PrepareUISound(target, volume);
                GetUISoundAudio(id).Play();

                break;
            }
        }

        return id;
    }

    /// <summary>
    /// すべてのサウンドを停止する
    /// </summary>
    public void StopAll()
    {
        _musicSource.Stop();
    }

    /// <summary>
    /// BGMをフェードアウトする
    /// </summary>
    /// <param name="fadeTime">フェードアウトするまでの時間 (秒)</param>
    public async UniTask FadeOutMusic(float fadeTime)
    {
        await InternalFadeOutMusic(fadeTime);
    }

    /// <summary>
    /// BGMをフェードアウトする
    /// </summary>
    /// <param name="id">フェードアウトするサウンドのID</param>
    /// <param name="fadeTime">フェードアウトするまでの時間 (秒)</param>
    public async UniTask FadeOutMusic(int id, float fadeTime)
    {
        Audio audio = GetAudio(Audio.AudioType.Music, id);

        if (audio == null) return;

        await audio.FadeOut(fadeTime);
    }

    private async UniTask<Audio> InternalFadeOutMusic(float fadeTime)
    {
        Audio audioToReturn = null;

        foreach ((int _, Audio audio) in _musicsAudio)
        {
            if (!audio?.IsPlaying ?? true)
                continue;

            audioToReturn = audio;

            await audio.FadeOut(fadeTime);

            break;
        }

        return await UniTask.FromResult(audioToReturn);
    }

    /// <summary>
    /// BGMをクロスフェード再生する
    /// </summary>
    /// <param name="nextMusic">次に再生するBGM</param>
    /// <param name="fadeTime">フェードするまでの時間 (秒)</param>
    /// <param name="interval">フェードアウト後にフェードインするまでの待機時間 (秒)</param>
    /// <param name="isLoop">次の再生をループするか</param>
    public async UniTask CrossFadeMusic(MusicDef nextMusic, float fadeTime, float interval = 0f, bool isLoop = true)
    {
        await FadeOutMusic(fadeTime);
        await UniTask.Delay(System.TimeSpan.FromSeconds(interval));

        int   id    = PrepareMusic((int) nextMusic, 0f, isLoop, null);
        Audio audio = GetMusicAudio(id);

        audio.Play();

        await audio.FadeIn(fadeTime);
    }
}
