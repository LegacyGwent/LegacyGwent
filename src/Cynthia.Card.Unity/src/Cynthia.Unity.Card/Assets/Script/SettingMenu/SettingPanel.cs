using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingPanel : MonoBehaviour
{

    private Resolution screenResolution; //分辨率
    private bool isFullScreen; //是否全屏
    private bool isCloseSound; //是否关闭所有声音
    private int musicVolum; //音乐大小
    private int effectVolum; //音效大小
    private int quality; //画质

    // public GameObject AudioSource;
    public AudioMixer AudioMixer;

    private void Start()
    {
        //初始化值,(计划之后变成读取文件的方式初始化值
        Initialization();
    }

    public void Initialization()
    {
        screenResolution = IndexToResolution(PlayerPrefs.GetInt("resolutionIndex", 2));
        isFullScreen = PlayerPrefs.GetInt("isFull", 0) == 0 ? true : false;
        isCloseSound = PlayerPrefs.GetInt("isCloseSound", 1) == 0 ? true : false;
        musicVolum = PlayerPrefs.GetInt("musicVolum", 5);
        effectVolum = PlayerPrefs.GetInt("effectVolum", 5);
        quality = PlayerPrefs.GetInt("quality", 2);
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
        screenResolution = IndexToResolution(index);
        Screen.SetResolution(screenResolution.width, screenResolution.height, isFullScreen);
#endif
    }

    //设置背景音乐大小
    public void SetMusic(int volum)
    {
        PlayerPrefs.SetInt("musicVolum", volum);
        musicVolum = volum;
        AudioMixer.SetFloat("musicVolum", (float)((musicVolum * 8) - 80));
    }

    //设置音效大小
    public void SetEffect(int volum)
    {
        PlayerPrefs.SetInt("effectVolum", volum);
        effectVolum = volum;
        AudioMixer.SetFloat("effectVolum", (float)((effectVolum * 8) - 80));
    }

    //设置画质
    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt("quality", qualityIndex);
        quality = qualityIndex;
        QualitySettings.SetQualityLevel(quality);
    }

    //设置静音
    public void SetCloseSound(int isClose)
    {
        PlayerPrefs.SetInt("isCloseSound", isClose);
        isCloseSound = ((isClose == 0) ? true : false);
        if (isCloseSound)
        {
            //AudioSource.GetComponent<AudioSource>().Pause();
            AudioMixer.SetFloat("volum", -80);
            return;
        }
        //AudioSource.GetComponent<AudioSource>().Play();
        AudioMixer.SetFloat("volum", 0);
    }

    //设置全屏
    public void SetFullScreen(int isFull)
    {
        PlayerPrefs.SetInt("isFull", isFull);
        isFullScreen = ((isFull == 0) ? true : false);
        Screen.SetResolution(screenResolution.width, screenResolution.height, isFullScreen);

    }
}
