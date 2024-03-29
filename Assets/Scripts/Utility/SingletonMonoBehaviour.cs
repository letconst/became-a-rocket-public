﻿using UnityEngine;

/// <summary>
/// Singleton
/// </summary>
/// <typeparam name="T">各クラス</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            System.Type t = typeof(T);

            _instance = (T) FindObjectOfType(t);

            if (_instance == null)
            {
                UnityEngine.Debug.LogError($"{t} をアタッチしているGameObjectはありません");
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        CheckInstance();
    }

    /// <summary>
    /// 他のゲームオブジェクトにアタッチされているか調べる。アタッチされている場合は破棄する。
    /// </summary>
    protected bool CheckInstance()
    {
        if (_instance == null)
        {
            _instance = this as T;

            return true;
        }

        if (Instance == this) return true;

        Destroy(this);

        return false;
    }
}
