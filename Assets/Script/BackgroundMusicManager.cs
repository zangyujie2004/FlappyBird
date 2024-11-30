using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance;  // 静态实例确保全局访问
    public AudioSource backgroundMusic;             // 背景音乐 AudioSource

    void Awake()
    {
        // 确保这个对象在场景切换时不会销毁
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 不销毁背景音乐对象
        }
        else
        {
            Destroy(gameObject);  // 如果已经有实例，则销毁新的
        }
    }

    // 控制音乐播放和音量
    public void PlayMusic()
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void StopMusic()
    {
        backgroundMusic.Stop();
    }

    public void SetVolume(float volume)
    {
        backgroundMusic.volume = volume;
    }

    // 停止背景音乐
    public static void StopBackgroundMusic()
    {
        if (instance != null)
        {
            instance.StopMusic();
            Destroy(instance.gameObject);  // 销毁背景音乐对象
        }
    }
}
