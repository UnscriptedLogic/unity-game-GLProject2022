using Core.Pooling;
using UnityEngine;
using UnscriptedEngine;

public class AudioManager : ULevelObject
{
    public enum AudioType
    {
        BGM,
        TOWERS,
        UNITS,
        UI,
        OTHER
    }
    
    public static AudioManager Instance { get; private set; }

    [SerializeField] private GameObject audioSourcePrefab;

    private GI_CustomGameInstance gameInstance;
    private PoolManager poolManager;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        base.Awake();
    }

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        poolManager = PoolManager.instance;
        gameInstance = GameMode.GetGameInstance<GI_CustomGameInstance>();
    }

    public static void PlayAudio(AudioType audioType, AudioClip clip, float volume, Vector3 position, bool variations = false)
    {
        GameObject audioSourceGO = Instance.poolManager.PullFromPool(Instance.audioSourcePrefab);
        audioSourceGO.transform.position = position;
        AudioSource audioSource = audioSourceGO.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        
        if (variations)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        }
        
        audioSource.volume = Instance.GetVolumeFromSave(audioType) * volume * Instance.gameInstance.SaveData.volumeData.master;
        audioSource.Play();
        
        Instance.poolManager.PushToPool(audioSourceGO, clip.length);
    }

    public float GetVolumeFromSave(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.BGM:
                return gameInstance.SaveData.volumeData.bgm;
            case AudioType.TOWERS:
                return gameInstance.SaveData.volumeData.towers;
            case AudioType.UNITS:
                return gameInstance.SaveData.volumeData.units;
            case AudioType.UI:
                return gameInstance.SaveData.volumeData.ui;
            case AudioType.OTHER:
                return gameInstance.SaveData.volumeData.other;
            default:
                return 1f;
        }
    }
}
