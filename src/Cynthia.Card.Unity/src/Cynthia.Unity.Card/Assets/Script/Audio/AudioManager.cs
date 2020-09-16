using Autofac;
using System.Collections.Generic;
using System.IO;
using Assets.Script.Localization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("AudioManager");
                obj.AddComponent<AudioManager>();
            }
            return instance;
        }
    }

    private static string cardAudioDirectory = "Voicelines/";

    private static string effectDirectory = "Music/Effect/";

    private float volume;

    private AudioSource _queueAudioSource;

    private List<AudioSource> onshotAudioSource = new List<AudioSource>();

    private List<AudioClip> audioClipbuffer = new List<AudioClip>();

    private void Awake()
    {
        instance = this;
        _queueAudioSource = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!_queueAudioSource.isPlaying && audioClipbuffer.Count > 0)
        {
            _queueAudioSource.clip = audioClipbuffer[0];
            audioClipbuffer.RemoveAt(0);
            _queueAudioSource.Play();
        }


        OnShootAudioVolumeSetting();
    }

    public void SetVolume(float volume)
    {
        this.volume = volume / 10;

        _queueAudioSource.volume = volume;

        foreach (var audioSource in onshotAudioSource)
        {
            audioSource.volume = volume;
        }
    }

    public void PlayAudio(string id, AudioType type, AudioPlayMode mode = AudioPlayMode.Append)
    {
        Object[] allclips = Resources.LoadAll(GetDirectory(type) + id);
        if (allclips.Length == 0)
            return;
        AudioClip clip = (AudioClip)allclips[Random.Range(0, allclips.Length)];
        if (mode == AudioPlayMode.Append)     //追加模式  等之前同类型音频播放完
        {
            if (audioClipbuffer.Count > 0 && audioClipbuffer[audioClipbuffer.Count - 1].name.Equals(id))
            {
                return;
            }
            else if (_queueAudioSource.isPlaying && _queueAudioSource.clip.name.Equals(id) && _queueAudioSource.clip.length - _queueAudioSource.time > 1)
            {
                return;
            }
            else
                audioClipbuffer.Add(clip);
        }
        else if (mode == AudioPlayMode.PlayOneShoot)          //播放一次模式  不管之前的音频，总播放  75%音量如果同时播放
        {
            AudioSource source = GetOneShootAudioSource();
            source.volume = volume;
            source.clip = clip;
            source.Play();
        }
    }

    private string GetDirectory(AudioType type)
    {
        string path = "";

        switch (type)
        {
            case AudioType.Card:
                path = GetCardDirectory();
                break;
            case AudioType.Effect:
                path = effectDirectory;
                break;
        }
        return path;
    }

    private void OnShootAudioVolumeSetting()
    {
        AudioSource[] audiosources = transform.GetComponents<AudioSource>();
        int playnum = 0;
        for (int i = 0; i < audiosources.Length; i++)
        {
            if (audiosources[i].isPlaying)
                playnum++;
            if (playnum > 1)
                break;
        }
        if (playnum > 1)
        {
            for (int i = 0; i < onshotAudioSource.Count; i++)
            {
                if (onshotAudioSource[i].isPlaying)
                {
                    onshotAudioSource[i].volume = 0.75f * volume;
                }
            }
        }
        else
        {
            for (int i = 0; i < onshotAudioSource.Count; i++)
            {
                if (onshotAudioSource[i].isPlaying)
                {
                    onshotAudioSource[i].volume = volume;
                }
            }
        }

    }

    private AudioSource GetOneShootAudioSource()
    {
        for (int i = 0; i < onshotAudioSource.Count; i++)
        {
            if (!onshotAudioSource[i].isPlaying)
            {
                return onshotAudioSource[i];
            }
        }
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = volume;
        onshotAudioSource.Add(source);
        return source;
    }

    private string GetCardDirectory()
    {
        var audioLanguageManager = DependencyResolver.Container.Resolve<LocalizationService>().AudioLocalization;
        string languageFilename = audioLanguageManager.ChosenLanguage.Filename;
        string path = $"{cardAudioDirectory}{languageFilename}/"; //CN, JP, EN
        if (Directory.Exists(path))
        {
            return path;
        }
        return $"{cardAudioDirectory}EN/";
    }
}

public enum AudioPlayMode
{
    Append = 0,
    PlayOneShoot
}

public enum AudioType
{
    Card = 0,
    Effect
}
