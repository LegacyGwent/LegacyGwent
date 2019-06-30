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

    // Use this for initialization
    void Start()
    {
        ResolutionPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("resolutionIndex", 2);
        FullPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isFull", 0);
        QualityPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("quality", 2);
        CloseSoundPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isCloseSound", 1);
        MusicVolumPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("musicVolum", 5);
        EffectVolumPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("effectVolum", 5);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
