using System.Collections;
using System.Collections.Generic;
using Assets.Script.SettingMenu;
using Cynthia.Card;
using UnityEngine;
using UnityEngine.Audio;

public class InitSetting : MonoBehaviour
{
    public GameObject TextLanguagePanel;
    public GameObject AudioLanguagePanel;
    public GameObject ResolutionPanel;
    public GameObject FullPanel;
    public GameObject CloseSoundPanel;
    public GameObject MusicVolumPanel;
    public GameObject EffectVolumPanel;
    public GameObject QualityPanel;
    public GameObject GamePlayBGMsPanel;

    // Use this for initialization
    void Start()
    {
        TextLanguagePanel.GetComponent<ChooseValueInit>().LoadValues();
        TextLanguagePanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("TextLanguage", 0);

        AudioLanguagePanel.GetComponent<ChooseValueInit>().LoadValues();
        AudioLanguagePanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("AudioLanguage", 0);

        ResolutionPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("resolutionIndex", 2);
        FullPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isFull", 0);
        QualityPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("quality", 2);
        CloseSoundPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isCloseSound", 1);
        MusicVolumPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("musicVolum", 7);
        EffectVolumPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("effectVolum", 7);
        GamePlayBGMsPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isWitcher", 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
