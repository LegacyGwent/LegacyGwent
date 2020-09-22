using Assets.Script.Localization;
using Autofac;
using System.Linq;
using UnityEngine;

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

    void Start()
    {
        Debug.Log("OBJ" + gameObject.name);
        Debug.Log("PARENT" + gameObject.transform.parent.name);

        InitLanguageSettings();

        ResolutionPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("resolutionIndex", 2);
        FullPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isFull", 0);
        QualityPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("quality", 2);
        CloseSoundPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isCloseSound", 1);
        MusicVolumPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("musicVolum", 7);
        EffectVolumPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("effectVolum", 7);
        GamePlayBGMsPanel.GetComponent<ChoseValue>().Index = PlayerPrefs.GetInt("isWitcher", 0);
    }

    void InitLanguageSettings()
    {
        var localizationService = DependencyResolver.Container.Resolve<LocalizationService>();

        var textLocalization = localizationService.TextLocalization;
        var textLangValues = textLocalization.ResourceHandler.LoadConfiguration().Select(c => c.Name).ToList();
        TextLanguagePanel.GetComponent<ChoseValue>().ChoseList = textLangValues;
        TextLanguagePanel.GetComponent<ChoseValue>().Index = textLocalization.ChosenLanguageIndex;

        var audioLocalization = localizationService.AudioLocalization;
        var audioLangValues = localizationService.AudioLocalization.ResourceHandler.LoadConfiguration().Select(c => c.Name).ToList();
        AudioLanguagePanel.GetComponent<ChoseValue>().ChoseList = audioLangValues;
        AudioLanguagePanel.GetComponent<ChoseValue>().Index = audioLocalization.ChosenLanguageIndex;
    }
}
