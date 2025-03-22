using System.Collections;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using Assets.Script.Localization;
using UnityEngine;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class Taunts : MonoBehaviour // This script controls the behaviour of the TauntUI in the Game scene
{
    private GwentClientService _clientService;
    private LocalizationService _translator;
    //-------------------------------------------------------------------------------------------------------------------
    //[SerializeField] GameObject GameUI; // import game UI object to access GameUIControl script
    public GameObject GameUI;
    GameUIControl gameUIControl; // access GameUIControl script to retreive GameInformation
    public GameObject TauntUI; // the taunt menu
    public GameObject MyTaunt; // my taunt panel where the taunt text is displayed
    public GameObject EnemyTaunt; // the enemy taunt panel where the taunt text is displayed
    public Image RoundAvatar; // the round avatar in the center of the taunt menu
    public Image AvatarArt; // my avatar art, to set to grey if taunts are on cooldown
    public Material LightGray;
    public Text MyTauntText;
    public Text EnemyTauntText;
    //-------------------------------------------------------------------------------------------------------------------
    public string myavatar; // revert public after test
    public string enemyname;
    public bool IsAwaiting;
    private bool IsEnemyNotMute; // if the ennemy is not mute, play the taunts he sends
    private bool IsTauntNotOnCoolDown = true; // check if you have to wait before sending another taunt
    private IList<TrinketAvatar> _avatars { get => TrinketMap.GetAvatars().ToList(); } // lists all avatar cosmetics
    [SerializeField] private Sprite mute;
    [SerializeField] private Sprite unmute;
    [SerializeField] private Image targetButton;


    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        gameUIControl = GameUI.GetComponent<GameUIControl>();
        
        
    }
    private void Start() 
    {
        IsTauntNotOnCoolDown = true;
        IsEnemyNotMute = true;
        IsAwaiting = false;
    }
    void Update()
    {
        if (myavatar.Length <1) // set the round avatar in the avatar UI
        {
            gameUIControl = GameUI.GetComponent<GameUIControl>();
            myavatar = gameUIControl.Myavatar;
            enemyname = gameUIControl.Enemyname;
            var op =Addressables.LoadAssetAsync<Sprite>(myavatar+"Round");
            Sprite go = op.WaitForCompletion();
            RoundAvatar.sprite = go;
        }
        ReceiveTaunt();
    }
    private void DisableSendTaunt() // Reenable sending taunts
    {
        IsTauntNotOnCoolDown = true;
        AvatarArt.material = null;
    }
    private void CloseMyTaunt() // Set Taunt Inactive After Animation and my avatar to grey
    {
        MyTaunt.SetActive(false);
        AvatarArt.material = LightGray;
        EnemyTaunt.SetActive(false);
    }
    private void CloseEnemyTaunt() // Set Taunt Inactive After Animation
    {
        MyTaunt.SetActive(false);
        EnemyTaunt.SetActive(false);
    }
    public async void ReceiveTaunt() // when you receive an enemytaunt from server, play its audio and write its text
    {
        IsAwaiting = true;
        if (IsEnemyNotMute)
        {
            
            string tauntID = await _clientService.PlayTaunt();
            if (tauntID.Length > 1)
            {
                EnemyTaunt.SetActive(true);
                PlayTaunt(tauntID);
                WriteEnemyTaunt(tauntID);
                InvokeRepeating("CloseEnemyTaunt", 3, 0);
            }
            
        }
        IsAwaiting = false;
    }
    public void PlayMyTaunt(string mytaunt) // play my taunt, write its text and send it to the server
    {
        PlayTaunt(mytaunt);
        WriteMyTaunt(mytaunt);
        SendTaunt(mytaunt);
        MyTaunt.SetActive(true);
        TauntUI.SetActive(false);
        InvokeRepeating("CloseMyTaunt", 3, 0);
    }
    public void WatchThis() // taunt 1
    {
        
        string mytaunt = _avatars.Where(x => x.ID == myavatar).Single().Taunt1.ToString();
        InvokeRepeating("DisableSendTaunt", 5, 0);
        PlayMyTaunt(mytaunt);
        IsTauntNotOnCoolDown = false;
    }
    public void Dammit() // taunt 2
    {
        string mytaunt = _avatars.Where(x => x.ID == myavatar).Single().Taunt2.ToString();
        InvokeRepeating("DisableSendTaunt", 5, 0);
        PlayMyTaunt(mytaunt);
        IsTauntNotOnCoolDown = false;
    }
    public void YouReGoingDown() // taunt 3
    {
        string mytaunt = _avatars.Where(x => x.ID == myavatar).Single().Taunt3.ToString();
        InvokeRepeating("DisableSendTaunt", 5, 0);
        PlayMyTaunt(mytaunt);
        IsTauntNotOnCoolDown = false;
    }
    public void BadMove() // taunt 4
    {
        string mytaunt = _avatars.Where(x => x.ID == myavatar).Single().Taunt4.ToString();
        InvokeRepeating("DisableSendTaunt", 5, 0);
        PlayMyTaunt(mytaunt);
        IsTauntNotOnCoolDown = false;
    }
    public void WellPlayed() // taunt 5
    {
        string mytaunt = _avatars.Where(x => x.ID == myavatar).Single().Taunt5.ToString();
        InvokeRepeating("DisableSendTaunt", 5, 0);
        PlayMyTaunt(mytaunt);
        IsTauntNotOnCoolDown = false;
    }
    public void Thanks() // taunt 6
    {
        string mytaunt = _avatars.Where(x => x.ID == myavatar).Single().Taunt6.ToString();
        InvokeRepeating("DisableSendTaunt", 5, 0);
        PlayMyTaunt(mytaunt);
        IsTauntNotOnCoolDown = false;
    }
    public async void SendTaunt(string tauntID) // send taunt to server
    {
        await _clientService.SendTaunt(enemyname, tauntID);
    }
    public void WriteMyTaunt(string tauntID) // write my taunt text
    {
        MyTauntText.text = _translator.GetText(tauntID);
    }
    public void WriteEnemyTaunt(string tauntID) // write the enemy taunt text
    {
        EnemyTauntText.text = _translator.GetText(tauntID);
    }
    public void PlayTaunt(string tauntID) // play the audio of a taunt
    {
        
        var audioLanguageManager = DependencyResolver.Container.Resolve<LocalizationService>().AudioLocalization;
        AudioManager.Instance.PlayAudio(tauntID + audioLanguageManager.ChosenLanguage.Filename, AudioType.Effect, AudioPlayMode.PlayOneShoot);
    }
    public void TauntButtonClicked()
    {   
        if(IsTauntNotOnCoolDown && myavatar != "NoAvatar")
        {
            TauntUI.SetActive(true);
        }
    }
    public void MuteButtonClicked()
    {
        targetButton.sprite = mute;
        if (IsEnemyNotMute)
        {
            IsEnemyNotMute = false;
            targetButton.sprite = mute;
        }
        else
        {
            IsEnemyNotMute = true;
            targetButton.sprite = unmute;
        }
    }
}
