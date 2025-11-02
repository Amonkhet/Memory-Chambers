using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBGMManager : MonoBehaviour
{
    public static SceneBGMManager Instance;

    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public AudioClip bgmClip;
        public float volume = 0.7f;
    }

    public AudioSource bgmSource;
    public List<SceneBGM> sceneBGMs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // ← 如果你不想跨场景保留，就删掉这一行
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        foreach (var item in sceneBGMs)
        {
            if (item.sceneName == sceneName)
            {
                if (bgmSource.clip == item.bgmClip && bgmSource.isPlaying)
                    return;

                bgmSource.clip = item.bgmClip;
                bgmSource.volume = item.volume;
                bgmSource.loop = true;
                bgmSource.Play();
                return;
            }
        }

        // 场景没配置 → 停
        bgmSource.Stop();
        bgmSource.clip = null;
        Debug.Log($"[SceneBGMManager] No BGM set for scene: {sceneName}, stop playing.");
    }
}
