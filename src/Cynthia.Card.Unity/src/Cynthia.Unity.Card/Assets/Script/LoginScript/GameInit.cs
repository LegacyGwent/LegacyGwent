using Autofac;
using Cynthia.Card.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Script.Localization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameInit : MonoBehaviour
{
    public GameObject GlobalUI;
    public AudioMixer AudioMixer;
    public BGMManager bgmManager;

    public List<GameObject> ObjectLocks;
    public UnityEvent OnGameLoad;

    public Text NowVersionText;
    public Text LatestVersionText;
    public Text NotesText;
    public Text VersionText;
    public RectTransform NotesContext;
    private string UpToDateVersion;
    private string CurrentVersion => Application.version;
    public GameObject Download_Button;
    string link;

    private GwentClientService _gwentClientService;
    private LocalizationService _translator;
    private CancellationTokenSource _loadServerMessageCancellation;

    private void Start()
    {
        _gwentClientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        ConfigureGame();
        _loadServerMessageCancellation = new CancellationTokenSource();
        LoadServerMessage(_loadServerMessageCancellation.Token);
    }

    private void OnDestroy()
    {
        _loadServerMessageCancellation?.Cancel();
    }

    public void OpenDownloadLink()
    {
        if (!string.IsNullOrWhiteSpace(link))
        {
            Application.OpenURL(link);
        }
    }

    public void ExitClick()
    {
        _gwentClientService.ExitGameClick();
    }

    private async void LoadServerMessage(CancellationToken cancellationToken)
    {
        try
        {
            await LoadServerMessageAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _loadServerMessageCancellation?.Dispose();
            _loadServerMessageCancellation = null;
        }
    }

    private async Task LoadServerMessageAsync(CancellationToken cancellationToken)
    {
        var i = 0;
        LatestVersionText.text = _translator.GetText("LoginMenu_LoadingLatestVersion");
        NotesText.text = _translator.GetText("LoginMenu_LoadingNews");
        while (true)
        {
            try
            {
                await _gwentClientService.EnsureConnectedAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                break;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                if (NotesText != null)
                {
                    i++;
                    if (i == 1)
                    {
                        NotesText.text += $"\n{_translator.GetText("LoginMenu_DisconnectionInfo")}";
                    }
                    NotesText.text += "\n" + string.Format(_translator.GetText("LoginMenu_DisconnectionRetry"), i, i + 1);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(NotesText.GetComponent<RectTransform>());
                    NotesContext.sizeDelta = new Vector2(NotesContext.sizeDelta.x, NotesText.GetComponent<RectTransform>().sizeDelta.y);
                }
                await Task.Delay(TimeSpan.FromSeconds(Math.Min(Math.Max(i, 1), 5)), cancellationToken);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var downloadLink = await _gwentClientService.GetDownloadLink(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            link = downloadLink;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            link = string.Empty;
            Debug.Log($"Unable to load the client download link: {e.Message}");
        }

        try
        {
            var latestClientVersion = await _gwentClientService.GetLatestClientVersion(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            UpToDateVersion = latestClientVersion;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            UpToDateVersion = "Unknown";
        }
        VersionText.text = $"{_translator.GetText("Local_version")}: {CurrentVersion}\n{_translator.GetText("Latest_version")}: {UpToDateVersion}";
        if (CurrentVersion != UpToDateVersion)
        {
            Download_Button.SetActive(true);
        }
        else 
        {
            Download_Button.SetActive(false);
        }
        try
        {
            var textLanguageManager = DependencyResolver.Container.Resolve<LocalizationService>().TextLocalization;
            var language = textLanguageManager.ChosenLanguage.Filename;
            if (language=="cn")
            {
                var notes = await _gwentClientService.GetNotes(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                NotesText.text = notes.Replace("\\n", "\n");
                LayoutRebuilder.ForceRebuildLayoutImmediate(NotesText.GetComponent<RectTransform>());
                NotesContext.sizeDelta = new Vector2(NotesContext.sizeDelta.x, NotesText.GetComponent<RectTransform>().sizeDelta.y);
            }
            else if (Array.Exists(new[] { "en", "ru", "pl" }, element => element == language))
            {
                var notes = await _gwentClientService.GetNotesEN(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                NotesText.text = notes.Replace("\\n", "\n");
                LayoutRebuilder.ForceRebuildLayoutImmediate(NotesText.GetComponent<RectTransform>());
                NotesContext.sizeDelta = new Vector2(NotesContext.sizeDelta.x, NotesText.GetComponent<RectTransform>().sizeDelta.y);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            if (NotesText != null)
            {
                NotesText.text = _translator.GetText("LoginMenu_NewsError");
            }
        }

        try
        {
            await _gwentClientService.AutoUpdateGame(LatestVersionText, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            if (LatestVersionText != null)
            {
                LatestVersionText.text = string.Format(_translator.GetText("LoginMenu_LatestVersionError"), e.Message);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (NotesContext != null)
        {
            NotesContext.sizeDelta =
                new Vector2(NotesContext.sizeDelta.x, NotesText.GetComponent<RectTransform>().sizeDelta.y);
        }

        foreach (var objLock in ObjectLocks)
        {
            objLock.SetActive(false);
        }
        OnGameLoad?.Invoke();
    }

    public void ConfigureGame()
    {
        if (!ClientGlobalInfo.IsLoadGlobal)
        {
            ClientGlobalInfo.IsLoadGlobal = true;
            var globalUI = Instantiate(GlobalUI);
            // var musicSource = Instantiate(AudioSound);
            var bgmManagerInstance = Instantiate(bgmManager);
            DontDestroyOnLoad(globalUI);
            // DontDestroyOnLoad(musicSource);
            DontDestroyOnLoad(bgmManagerInstance);

            globalUI.name = "GlobalUI";
            bgmManagerInstance.name = "BGMManager";
            // musicSource.name = "MusicSource";

            SetResolution(PlayerPrefs.GetInt("resolutionIndex", 2));
            SetQuality(PlayerPrefs.GetInt("quality", 2));
            SetCloseSound(PlayerPrefs.GetInt("isCloseSound", 1));
            SetMusic(PlayerPrefs.GetInt("musicVolum", 7));
            SetEffect(PlayerPrefs.GetInt("effectVolum", 7));
            NowVersionText.text = string.Format(_translator.GetText("LoginMenu_CurrentVersionInfo"), Application.version);

            // AudioManager.Instance.SetVolume(PlayerPrefs.GetInt("musicVolum", 5));
            // AudioManager.Instance.SetLanguageType((LanguageType)PlayerPrefs.GetInt("Language", 0));
        }
    }

    public Resolution IndexToResolution(int index)
    {
        Resolution resolution = new Resolution();
        switch (index)
        {
            case 0:
                resolution.width = 1024;
                resolution.height = 576;
                break;
            case 1:
                resolution.width = 1600;
                resolution.height = 900;
                break;
            case 2:
                resolution.width = 1920;
                resolution.height = 1080;
                break;
            default:
                resolution.width = 1920;
                resolution.height = 1080;
                break;
        }
        return resolution;
    }

    //屏幕分辨率
    public void SetResolution(int index)
    {
#if !(UNITY_ANDROID || UNITY_IOS)
        PlayerPrefs.SetInt("resolutionIndex", index);
        var screenResolution = IndexToResolution(index);
        var isFullScreen = ((PlayerPrefs.GetInt("isFull", 0) == 0) ? true : false);
        Screen.SetResolution(screenResolution.width, screenResolution.height, isFullScreen);
#endif
    }

    //设置背景音乐大小
    public void SetMusic(int volum)
    {
        PlayerPrefs.SetInt("musicVolum", volum);
        AudioMixer.SetFloat("musicVolum", SettingPanel.LinearToDecibel(volum / 10f));
    }

    //设置音效大小
    public void SetEffect(int volum)
    {
        PlayerPrefs.SetInt("effectVolum", volum);
        AudioMixer.SetFloat("effectVolum", SettingPanel.LinearToDecibel(volum / 10f));
        AudioManager.Instance.SetVolume(volum);
    }

    //设置画质
    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt("quality", qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    //设置静音
    public void SetCloseSound(int isClose)
    {
        PlayerPrefs.SetInt("isCloseSound", isClose);
        var isCloseSound = ((isClose == 0) ? true : false);
        if (isCloseSound)
        {
            //AudioSource.GetComponent<AudioSource>().Pause();
            AudioMixer.SetFloat("volum", -80);
            return;
        }
        //AudioSource.GetComponent<AudioSource>().Play();
        AudioMixer.SetFloat("volum", 0);
    }
}
