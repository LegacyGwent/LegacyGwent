using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class InitSetting : MonoBehaviour
{

    // public GameObject AudioSource;
    public AudioMixer AudioMixer;
    public GameObject ResolutionPanel;
    public GameObject FullPanel;
    public GameObject CloseSoundPanel;
    public GameObject MusicVolumPanel;
    public GameObject EffectVolumPanel;
    public GameObject QualityPanel;
    public GameObject LanguagePanel;
    public GameObject GamePlayBGMsPanel;

    // Use this for initialization
    void Start()
    {
        ResolutionPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("resolutionIndex", 2);
        FullPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isFull", 0);
        QualityPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("quality", 2);
        LanguagePanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("Language",0);
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
