using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BGM列挙定数
public enum BGM
{
    Main,               // メインBGM
    BossBattle,         // ボス戦BGM
    

    Max
}

// SE列挙定数
public enum SE
{
    Landing,        // 着地
    Shot,           // 弾発射
    ShotHit,        // 弾命中
    Damage,         // 被ダメージ
    Miss,           // ミス
    EnemyShot,      // 敵弾発射
    BossAiming,     // ボス銃構え
    BossShot1,      // ボス弾発射
    BossCharge,     // ボスチャージ
    BossShot2,      // ボス弾発射
    BossJump,       // ボスジャンプ

    Shutter,        // シャッター開閉
    MagmaDive,      // マグマで死
    Needle,         // 針で死

    Victory,        // クリア
    StageStart,     // ステージスタート

    Max
}

// システムSE列挙定数
public enum SysSE
{
    Touch,              // ボタンにタッチ
    GameStart,          // ゲームスタートボタン押下時
    Pause,              // ポーズ時
    HighScoreUpdate,    // ハイスコア更新

    MAX
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // 音量
    public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    // BGM
    private AudioSource BGMsource;
    // SE
    private AudioSource[] SEsources = new AudioSource[32];
    // 音声
    private AudioSource[] SystemSEsources = new AudioSource[16];

    // === AudioClip ===
    // BGM
    public AudioClip[] BGM;
    // SE
    public AudioClip[] SE;
    // 音声
    public AudioClip[] SystemSE;


    protected override void Awake()
    {
        base.Awake();

        // シーン遷移時にDestroyされないようにする
        DontDestroyOnLoad(gameObject);

        // BGM AudioSource
        BGMsource = gameObject.AddComponent<AudioSource>();
        // BGMはループを有効にする
        BGMsource.loop = true;

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++)
        {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }

        // 音声 AudioSource
        for (int i = 0; i < SystemSEsources.Length; i++)
        {
            SystemSEsources[i] = gameObject.AddComponent<AudioSource>();
        }

        // ボリューム設定
        BGMsource.volume = volume.BGM;
        foreach (AudioSource source in SEsources)
        {
            source.volume = volume.SE;
        }
        foreach (AudioSource source in SystemSEsources)
        {
            source.volume = volume.SystemSE;
        }
    }

    void Update()
    {
        // ミュート設定
        BGMsource.mute = volume.Mute;
        foreach (AudioSource source in SEsources)
        {
            source.mute = volume.Mute;
        }
        foreach (AudioSource source in SystemSEsources)
        {
            source.mute = volume.Mute;
        }

    }

    // ***** BGM再生 *****
    // BGM再生
    public void PlayBGM(BGM index, float startTime = 0)
    {
        if (0 > index || BGM.Length <= (int)index)
        {
            return;
        }
        // 同じBGMの場合は何もしない
        if (BGMsource.clip == BGM[(int)index])
        {
            return;
        }
        BGMsource.Stop();
        BGMsource.clip = BGM[(int)index];
        BGMsource.time = startTime;
        BGMsource.Play();
    }

    // BGMSourceのvolume変更
    public void BGMVolume(float volume)
    {
        BGMsource.volume = volume;
    }

    // BGM一時停止
    public void PauseBGM()
    {
        BGMsource.Pause();
    }

    // BGM再生再開
    public void UnPauseBGM()
    {
        BGMsource.UnPause();
    }

    // BGM停止
    public void StopBGM()
    {
        BGMsource.Stop();
        BGMsource.clip = null;
    }


    // ***** SE再生 *****
    // SE再生
    public void PlaySE(SE index, float SEVolume = 0f)
    {
        if (0 > index || SE.Length <= ((int)index))
        {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources)
        {
            if (false == source.isPlaying)
            {
                source.clip = SE[((int)index)];
                if(SEVolume <= 0f)
                {
                    source.volume = volume.SE;
                }
                else
                {
                    source.volume = SEVolume;
                }
                source.Play();
                return;
            }
        }
    }

    // SE一時停止
    public void PauseSE()
    {
        foreach (AudioSource source in SEsources)
        {
            source.Pause();
        }
    }

    // SE再生再開
    public void UnPauseSE()
    {
        foreach (AudioSource source in SEsources)
        {
            source.UnPause();
        }
    }

    // SE停止
    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }


    // ***** 音声再生 *****
    // 音声再生
    public void PlaySystemSE(SysSE index)
    {
        if (0 > index || SystemSE.Length <= ((int)index))
        {
            return;
        }
        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SystemSEsources)
        {
            if (false == source.isPlaying)
            {
                source.clip = SystemSE[((int)index)];
                source.Play();
                return;
            }
        }
    }

    // 音声停止
    public void StopSystemSE()
    {
        // 全ての音声用のAudioSouceを停止する
        foreach (AudioSource source in SystemSEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    // 再生中のBGMをフェードアウトさせてから指定した新しいBGMを再生開始する
    public void BGMFadeChange(BGM newBGM, float fadeOutTime)
    {
        StartCoroutine(FadeOut(fadeOutTime));
        StartCoroutine(PlayBGMAfterWait(newBGM, fadeOutTime));
    }

    // 指定した時間待機してからBGMを再生する
    private IEnumerator PlayBGMAfterWait(BGM newBGM, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        yield return new WaitForFixedUpdate();
        PlayBGM(newBGM);
    }

    // フェードアウトコルーチン呼び出し
    public void BGMFadeOut(float fadeOutTime)
    {
        StartCoroutine(FadeOut(fadeOutTime));
    }

    // フェードアウト
    private IEnumerator FadeOut(float fadeOutTime)
    {
        float currentTime = 0.0f;
        float firstVol = BGMsource.volume;

        // 現在の音量から0まで、0.0f ~ 1.0fの範囲になるように徐々に下げる
        while(fadeOutTime > currentTime)
        {
            currentTime += Time.fixedDeltaTime;
            BGMsource.volume = Mathf.Clamp01(firstVol * (fadeOutTime - currentTime) / fadeOutTime);
            yield return new WaitForFixedUpdate();
        }

        // 0に下げきる
        BGMsource.volume = 0;
        // BGM停止
        StopBGM();

        // 一瞬待ってから音量を戻す
        yield return new WaitForFixedUpdate();
        BGMsource.volume = firstVol;
    }
}

// 音量クラス
[Serializable]
public class SoundVolume
{
    public float BGM = 1.0f;
    public float SE = 1.0f;
    public float SystemSE = 1.0f;
    public bool Mute = false;

    public void Init()
    {
        BGM = 1.0f;
        SE = 1.0f;
        SystemSE = 1.0f;
        Mute = false;
    }
}
