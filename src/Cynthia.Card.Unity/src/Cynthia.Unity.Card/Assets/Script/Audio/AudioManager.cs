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

     private static string cardAudioCommonDirectory = "Voicelines/Common/";

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

    public void SetVolume(float newVolume)
    {
        volume = newVolume / 10;
        _queueAudioSource.volume = volume;
        foreach (var audioSource in onshotAudioSource)
        {
            audioSource.volume = volume;
        }
    }

    public int GetVoiceLineCount(string id)
    {
        var allclips = Resources.LoadAll<AudioClip>(cardAudioCommonDirectory + id);
        if (allclips.Length == 0)//if no common look in language specyfic
        {
            allclips = Resources.LoadAll<AudioClip>(GetDirectory(AudioType.Card) + id);
        }
        return allclips.Length;
    }

    public bool PlayAudio(string id, AudioType type, AudioPlayMode mode = AudioPlayMode.Append, int clipIndex = -1)
    {
        var allclips = Resources.LoadAll<AudioClip>(cardAudioCommonDirectory + id);//take voicelines from common
        if (allclips.Length == 0)//if no common look in language specyfic
        {
            allclips = Resources.LoadAll<AudioClip>(GetDirectory(type) + id);
            if (allclips.Length == 0)
            {
                return false;
            }
        }

        

        

        AudioClip clip;
        if (clipIndex < 0)
        {
            // default: pick random
            clip = allclips[Random.Range(0, allclips.Length)];
        }
        else
        {
            // pick specific index, safe modulo wrap
            int safeIndex = clipIndex % allclips.Length;
            clip = allclips[safeIndex];
        }

        clip.name = id;

        if (mode == AudioPlayMode.Append) // queue mode
        {
            if (audioClipbuffer.Count > 0 && audioClipbuffer[audioClipbuffer.Count - 1].name.Equals(id))
            {
                return false;
            }
            else if (_queueAudioSource.isPlaying && _queueAudioSource.clip.name.Equals(id) && _queueAudioSource.clip.length - _queueAudioSource.time > 1)
            {
                return false;
            }
            else
            {
                audioClipbuffer.Add(clip);
                return true;
            }
        }
        else if (mode == AudioPlayMode.PlayOneShoot) // fire instantly
        {
            AudioSource source = GetOneShootAudioSource();
            source.volume = volume;
            source.clip = clip;
            source.Play();
            return true;
        }
        return false;
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
        return path;
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
