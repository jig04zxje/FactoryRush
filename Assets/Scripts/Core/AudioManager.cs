using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Clips")]
    public AudioClip sellDingClip;
    public AudioClip timerTickClip;
    public AudioClip recordBrokenClip;

    [Header("Music Clips")]
    public AudioClip bgmGameplay;
    public AudioClip bgmMenu;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
    }

    // Gọi theo tên key string — đúng với mô tả requirement
    public void PlaySFX(string key)
    {
        AudioClip clip = key switch
        {
            "sell_ding" => sellDingClip,
            "timer_tick" => timerTickClip,
            "record_broken" => recordBrokenClip,
            _ => null
        };

        if (clip != null)
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"AudioManager: key '{key}' not found!");
    }

    public void PlayMusic(string key)
    {
        AudioClip clip = key switch
        {
            "bgm_gameplay" => bgmGameplay,
            "bgm_menu" => bgmMenu,
            _ => null
        };

        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();
}
