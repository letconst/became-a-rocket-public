using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public sealed partial class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField, Header("UI用SEのチャンネル数（同時に鳴らせる数）")]
    private int UISoundChannel;

    [SerializeField, Header("プーリング用のAudioSource")]
    private AudioSource sourceForPool;

    private AudioSource   _musicSource;
    private AudioSource[] _soundSources;
    private AudioSource[] _UISoundSources;

    private Dictionary<int, Audio> _musicsAudio   = new();
    private Dictionary<int, Audio> _soundsAudio   = new();
    private Dictionary<int, Audio> _UISoundsAudio = new();

    private Dictionary<int, AudioClip> _musicClips   = new();
    private Dictionary<int, AudioClip> _soundClips   = new();
    private Dictionary<int, AudioClip> _UISoundClips = new();

    public static bool IsInitialized { get; private set; }

    public static SoundObjectPool SoundPool { get; private set; }

    protected override async void Awake()
    {
        base.Awake();

        await Init();
    }

    private void Update()
    {
        // 各種登録AudioのUpdate処理を実行
        UpdateAudio(_musicsAudio);
        UpdateAudio(_soundsAudio);
        UpdateAudio(_UISoundsAudio);
    }

    /// <summary>
    /// 初期化処理を行う
    /// </summary>
    private async UniTask Init()
    {
        if (IsInitialized) return;

        // 各種AudioSource生成
        _musicSource    = AddAudioSourceComponent();
        _soundSources   = new AudioSource[UISoundChannel];
        _UISoundSources = new AudioSource[UISoundChannel];

        SoundPool = new SoundObjectPool(sourceForPool);

        // サウンドファイル読み込み
        Array musicDef   = Enum.GetValues(typeof(MusicDef));
        Array soundDef   = Enum.GetValues(typeof(SoundDef));
        Array uiSoundDef = Enum.GetValues(typeof(UISoundDef));

        for (int i = 0; i < musicDef.Length; i++)
        {
            await LoadAudio(musicDef, i, _musicClips);
        }

        for (int i = 0; i < soundDef.Length; i++)
        {
            await LoadAudio(soundDef, i, _soundClips);
        }

        for (int i = 0; i < uiSoundDef.Length; i++)
        {
            await LoadAudio(uiSoundDef, i, _UISoundClips);
        }

        IsInitialized = true;

        // 自身にAudioSourceを新規アタッチする
        AudioSource AddAudioSourceComponent()
        {
            return Instance.gameObject.AddComponent<AudioSource>();
        }

        // 指定のサウンドデータを読み込む
        async UniTask LoadAudio(Array audioDef, int index, IDictionary<int, AudioClip> audioClips)
        {
            object def  = audioDef.GetValue(index);
            var    clip = await Addressables.LoadAssetAsync<AudioClip>(def.ToString());

            audioClips.Add((int) def, clip);
        }
    }

    /// <summary>
    /// SoundManagerの初期化が終了するまで待機する
    /// </summary>
    /// <returns></returns>
    public UniTask WaitForReady()
    {
        return UniTask.WaitUntil(() => IsInitialized);
    }

    /// <summary>
    /// 登録されたAudioのUpdate処理を呼ぶ
    /// </summary>
    /// <param name="audioDict">対象のAudio dict</param>
    private void UpdateAudio(Dictionary<int, Audio> audioDict)
    {
        List<int> keys = new(audioDict.Keys);

        foreach (int id in keys)
        {
            Audio audio = audioDict[id];
            audio.Update();

            // サウンドが未使用状態になったらプールに返す
            if (!audio.IsPlaying && !audio.IsPaused && Application.isFocused)
            {
                // AudioSource情報をリセット
                audio.Reset();

                // 自身にアタッチされている場合は返さない
                if (!IsSelfAudioSource(audio.AudioSource) && audio.Position != null)
                {
                    SoundPool.Return(audio.AudioSource);
                }

                audioDict.Remove(id);
            }
        }
    }

    /// <summary>
    /// BGMを初期化する
    /// </summary>
    /// <param name="target">再生するBGMのenumインデックス</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">ループするか</param>
    /// <param name="position">再生する位置</param>
    /// <returns>サウンドID</returns>
    private int PrepareMusic(int target, float volume, bool isLoop, Vector3? position)
    {
        return PrepareAudio(Audio.AudioType.Music, target, volume, isLoop, position);
    }

    /// <summary>
    /// SEを初期化する
    /// </summary>
    /// <param name="target">再生するSEのenumインデックス</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">ループするか</param>
    /// <param name="position">再生する位置</param>
    /// <returns>サウンドID</returns>
    private int PrepareSound(int target, float volume, bool isLoop, Vector3? position)
    {
        return PrepareAudio(Audio.AudioType.Sound, target, volume, isLoop, position);
    }

    /// <summary>
    /// UI用SEを初期化する
    /// </summary>
    /// <param name="target">再生するUI用SEのenumインデックス</param>
    /// <param name="volume">音量</param>
    /// <returns>サウンドID</returns>
    private int PrepareUISound(int target, float volume)
    {
        return PrepareAudio(Audio.AudioType.UISound, target, volume, false, null);
    }

    /// <summary>
    /// サウンドを初期化する
    /// </summary>
    /// <param name="type">再生するサウンドの種類</param>
    /// <param name="target">再生するサウンドデータのenumインデックス</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">ループするか</param>
    /// <param name="position">再生位置</param>
    /// <returns>サウンドID</returns>
    private int PrepareAudio(Audio.AudioType type, int target, float volume, bool isLoop, Vector3? position)
    {
        AudioClip clip = type switch
        {
            Audio.AudioType.Music   => _musicClips[target],
            Audio.AudioType.Sound   => _soundClips[target],
            Audio.AudioType.UISound => _UISoundClips[target]
        };

        AudioSource targetSource = type switch
        {
            Audio.AudioType.Music   => _musicSource,
            Audio.AudioType.Sound   => SoundPool.Rent(),
            Audio.AudioType.UISound => SoundPool.Rent()
        };

        var audio = new Audio(type, targetSource, clip, volume, isLoop, position);

        Dictionary<int, Audio> targetDict = type switch
        {
            Audio.AudioType.Music   => _musicsAudio,
            Audio.AudioType.Sound   => _soundsAudio,
            Audio.AudioType.UISound => _UISoundsAudio,
        };

        targetDict.Add(audio.ID, audio);

        return audio.ID;
    }
}
