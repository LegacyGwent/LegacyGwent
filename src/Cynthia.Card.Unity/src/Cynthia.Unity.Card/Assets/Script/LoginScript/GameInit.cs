using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;

public class GameInit : MonoBehaviour
{
    public GameObject GlobalUI;
    //public GameObject AudioSound;
    public AudioMixer AudioMixer;

    public Text NowVersionText;
    public Text LatestVersionText;
    public Text NotesText;
    public RectTransform NotesContext;

    private GwentClientService _gwentClientService;

    private void Start()
    {
        _gwentClientService = DependencyResolver.Container.Resolve<GwentClientService>();
        ConfigureGame();
        LoadServerMessage();
    }

    public void ExitClick()
    {
        _gwentClientService.ExitGameClick();
    }

    public async void LoadServerMessage()
    {
        try
        {
            Debug.Log($"试图获取公告");
            NotesText.text = (await _gwentClientService.GetNotes()).Replace("\\n", "\n");
            LayoutRebuilder.ForceRebuildLayoutImmediate(NotesText.GetComponent<RectTransform>());
            NotesContext.sizeDelta = new Vector2(NotesContext.sizeDelta.x, NotesText.GetComponent<RectTransform>().sizeDelta.y);
        }
        catch
        {
            NotesText.text = "暂未获取到公告。可能是由于服务器未打开或网络连接中断。";
        }

        try
        {
            //var version = new Version(await _gwentClientService.GetLatestVersion());
            //LatestVersionText.text = ClientGlobalInfo.Version == version ? "当前已为最新版本" : "最新版本为：" + version.ToString();
            await _gwentClientService.AutoUpdateCardMapVersion(LatestVersionText);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            LatestVersionText.text = $"发生异常错误,可能原因:文件损坏或未连接到服务器  异常信息:{e.Message}";
        }
        NotesContext.sizeDelta = new Vector2(NotesContext.sizeDelta.x, NotesText.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void ConfigureGame()
    {
        if (ClientGlobalInfo.IsLoadGlobal) return;
        ClientGlobalInfo.IsLoadGlobal = true;
        var globalUI = Instantiate(GlobalUI);
        //var musicSource = Instantiate(AudioSound);
        globalUI.name = "GlobalUI";
        //musicSource.name = "MusicSource";
        DontDestroyOnLoad(globalUI);
        //DontDestroyOnLoad(musicSource);

        SetResolution(PlayerPrefs.GetInt("resolutionIndex", 2));
        SetQuality(PlayerPrefs.GetInt("quality", 2));
        SetCloseSound(PlayerPrefs.GetInt("isCloseSound", 1));
        SetMusic(PlayerPrefs.GetInt("musicVolum", 7));
        SetEffect(PlayerPrefs.GetInt("effectVolum", 7));
        NowVersionText.text = "当前版本为：" + ClientGlobalInfo.Version.ToString();
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
#if !UNITY_ANDROID
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
        AudioMixer.SetFloat("musicVolum", (float)((volum * 8) - 80));
    }

    //设置音效大小
    public void SetEffect(int volum)
    {
        PlayerPrefs.SetInt("effectVolum", volum);
        AudioMixer.SetFloat("effectVolum", (float)((volum * 8) - 80));
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
