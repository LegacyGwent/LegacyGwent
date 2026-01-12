using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using Autofac;
using Cynthia.Card.Client;
using Cynthia.Card;
using Assets.Script.Localization;

public class RankPlayerScreenScript : MonoBehaviour
{
    public Image TotalBar_BarDraws;
    public Image TotalBar_BarWins;
    public Image Monsters_BarDraws;
    public Image Monsters_BarWins;
    public Image Nilfgaard_BarDraws;
    public Image Nilfgaard_BarWins;
    public Image NorthernRealms_BarDraws;
    public Image NorthernRealms_BarWins;
    public Image Scoiatael_BarDraws;
    public Image Scoiatael_BarWins;
    public Image Skellige_BarDraws;
    public Image Skellige_BarWins;
    public Image PlayerAvatar;
    public Image PlayerBorder;
    public Text PlayerNickname;
    public Image RankImage;
    public Text MMR;
    public Text Games;
    public Text Winrate;
    public Text RankNumber;
    public Text AllFactionsLabel;
    public Text AllFactionsStreak;
    public Text MonstersLabel;
    public Text MonstersStreak;
    public Text NilfgaardLabel;
    public Text NilfgaardStreak;
    public Text NorthernRealmsLabel;
    public Text NorthernRealmsStreak;
    public Text ScoiataelLabel;
    public Text ScoiataelStreak;
    public Text SkelligeLabel;
    public Text SkelligeStreak;
    public GameObject[] FactionRankIcons;
    public GameObject FactionRankList;
    public Sprite[] factionHeaderSprites;
    public Image FactionRankListHeader;
    public Sprite[] factionIcons;
    public Image FactionBackgroundLogo;
    public Text peakMMR;
    public Text PlayerTitle;
    public GameObject FactionBars;

    private Dictionary<int, List<Tuple<string, int>>> factionsRanks;

    private Dictionary<string, Tuple<Image, Image>> bars;
    private string playerNickname;
    private GwentClientService _clientService;
    private IList<Title> _titles { get => TrinketMap.GetTitles().ToList(); }
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; } 
    private  LocalizationService _translator;

    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
         _translator = DependencyResolver.Container.Resolve<LocalizationService>();

        bars = new Dictionary<string, Tuple<Image, Image>>(){
            {"total", new Tuple<Image, Image>(TotalBar_BarDraws, TotalBar_BarWins)},
            {"monsters", new Tuple<Image, Image>(Monsters_BarDraws, Monsters_BarWins)},
            {"nilfgaard", new Tuple<Image, Image>(Nilfgaard_BarDraws, Nilfgaard_BarWins)},
            {"northernrealms", new Tuple<Image, Image>(NorthernRealms_BarDraws, NorthernRealms_BarWins)},
            {"scoiatael", new Tuple<Image, Image>(Scoiatael_BarDraws, Scoiatael_BarWins)},
            {"skellige", new Tuple<Image, Image>(Skellige_BarDraws, Skellige_BarWins)}
        };
    }

    private void Start()
    {
        var _streakTexts = new List<Text> { AllFactionsStreak, MonstersStreak, NilfgaardStreak, NorthernRealmsStreak, ScoiataelStreak, SkelligeStreak };
        float _outline = 1.6f;
        foreach (var _textObject in _streakTexts)
        {
            _textObject.color = new Color(1.0f, 0.854f, 0.325f, 1.0f);
            _textObject.GetComponent<Outline>().effectDistance = new Vector2(_outline, -_outline);
        }

    }
    public void SetPlayerInfo(string avatar, string border, string nickname, string title, int mmr, int bestMMR, IList<int[]> statsFactions, int rankNumber, Dictionary<int, List<Tuple<string, int>>> factionsRankings)
    {
        factionsRanks = factionsRankings;
        playerNickname = nickname;

        if (avatar == null || avatar == "")
            avatar = "NoAvatar";

        if (border == null || border == "")
            border = "NoBorder";

        if (title == null || title == "")
            title = "CARDSMITH";

        try
        {
            var _avatar = Addressables.LoadAssetAsync<Sprite>(avatar);
            Sprite avatar_img = _avatar.WaitForCompletion();
            PlayerAvatar.sprite = avatar_img;
        }
        catch (Exception e) { }

        try
        {
            var _border = Addressables.LoadAssetAsync<Sprite>(border);
            Sprite border_img = _border.WaitForCompletion();
            PlayerBorder.sprite = border_img;
        }
        catch (Exception e) { }
        var titleColor = Color.white;
        try
        {
            titleColor = mycolormap[_titles.Where(x => x.ID == title).Single().TitleColor];
            PlayerTitle.text = _translator.GetText(title + "Name");
            PlayerTitle.color = titleColor;
        }
        catch (Exception e) { }
        try
        {
            var _rank = Addressables.LoadAssetAsync<Sprite>(CurrentTrinkets.GetRankIcon(mmr));
            Sprite rank_img = _rank.WaitForCompletion();
            RankImage.sprite = rank_img;
            RankImage.SetNativeSize();
        }
        catch (Exception e) { }

        PlayerNickname.text = nickname;

        MMR.text = mmr.ToString();
        peakMMR.text = $"({bestMMR.ToString()})";

        RankNumber.text = "#" + rankNumber.ToString();

        int games = 0;
        var totalgames = new int[3];

        foreach (var item in statsFactions)
        {
            for (int i = 0; i < 3; i++)
            {
                totalgames[i] += item[i];
                games += item[i];
            }
        }

        Games.text = $"{_translator.GetText("Leaderboard_Games")}: {games.ToString()}";

        Winrate.text = games > 0
            ? $"{_translator.GetText("Leaderboard_Winrate")}: " + ((float)totalgames[0] / games * 100f).ToString("F2") + "%"
            : $"{_translator.GetText("Leaderboard_Winrate")}: 0%";

        if (games > 0)
        {
            float win_ratio = (float)totalgames[0] / (float)games;
            SetBar("total", "wins", win_ratio);
            SetBar("total", "draws", win_ratio + (float)totalgames[2] / (float)games);
        }
        else
        {
            SetBar("total", "wins", 0.0f);
            SetBar("total", "draws", 0.0f);
        }


        var factionnames = new List<string>() { "monsters", "nilfgaard", "northernrealms", "scoiatael", "skellige" };

        for (int i = 0; i < statsFactions.Count; i++)
        {
            float faction_games = (float)statsFactions[i].Sum();
            if (faction_games > 0)
            {
                float faction_win_ratio = (float)statsFactions[i][0] / faction_games;
                SetBar(factionnames[i], "wins", faction_win_ratio);
                SetBar(factionnames[i], "draws", faction_win_ratio + (float)statsFactions[i][2] / faction_games);

                var wins = (float)statsFactions[i][0];
                var draws = (float)statsFactions[i][2];
                var loses = (float)statsFactions[i][1];
                var a = 2000.0f;
                var b = 2000.0f;
                var ewr = (wins + 0.5 * draws + a) / (wins + loses + draws + a + b) * 100;
                var ewrWeighted = ewr + Math.Log10(faction_games) * 0.05f;
            }
            else
            {
                SetBar(factionnames[i], "wins", 0.0f);
                SetBar(factionnames[i], "draws", 0.0f);
            }

            int _factionRankPosition = factionsRankings[i].FindIndex(t => t.Item1 == nickname) + 1;
            FactionRankIcons[i].transform.GetChild(0).GetComponent<Text>().text = $"#{_factionRankPosition}";

            if (_factionRankPosition <= 3)
                FactionRankIcons[i].GetComponent<Image>().color = new Color(0.924f, 0.766f, 0f, 1f);
            else if (_factionRankPosition > 10)
                FactionRankIcons[i].GetComponent<Image>().color = new Color(0.849f, 0.641f, 0.452f, 1f);
            else
                FactionRankIcons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }


        AllFactionsStreak.text = $"{totalgames[0]}  -  {totalgames[2]}  -  {totalgames[1]}";
        AllFactionsLabel.text = games > 0 ? $"{_translator.GetText("Faction_ALL")} {_translator.GetText("Leaderboard_Games")}: {games} ({((float)totalgames[0] / (float)games) * 100:F2}%)" : $"{_translator.GetText("Faction_ALL")} {_translator.GetText("Leaderboard_Games")}: {games}";

        int _factionGames = statsFactions[0].Sum();
        int _factionWins = statsFactions[0][0];

        MonstersStreak.text = $"{statsFactions[0][0]}  -  {statsFactions[0][2]}  -  {statsFactions[0][1]}";
        MonstersLabel.text = _factionGames > 0 ? $"{_translator.GetText("Faction_MO")} {_translator.GetText("Leaderboard_Games")}: {_factionGames} ({((float)_factionWins / (float)_factionGames) * 100:F2}%)" : $"{_translator.GetText("Faction_MO")} {_translator.GetText("Leaderboard_Games")}: {_factionGames}";

        _factionGames = statsFactions[1].Sum();
        _factionWins = statsFactions[1][0];
        NilfgaardStreak.text = $"{statsFactions[1][0]}  -  {statsFactions[1][2]}  -  {statsFactions[1][1]}";
        NilfgaardStreak.color = new Color(1.0f, 0.854f, 0.325f, 1.0f);
        NilfgaardLabel.text = _factionGames > 0 ? $"{_translator.GetText("Faction_NG")} {_translator.GetText("Leaderboard_Games")}: {_factionGames} ({((float)_factionWins / (float)_factionGames) * 100:F2}%)" : $"{_translator.GetText("Faction_NG")} {_translator.GetText("Leaderboard_Games")}: {_factionGames}";

        _factionGames = statsFactions[2].Sum();
        _factionWins = statsFactions[2][0];
        NorthernRealmsStreak.text = $"{statsFactions[2][0]}  -  {statsFactions[2][2]}  -  {statsFactions[2][1]}";
        NorthernRealmsStreak.color = new Color(1.0f, 0.854f, 0.325f, 1.0f);
        NorthernRealmsLabel.text = _factionGames > 0 ? $"{_translator.GetText("Faction_NR")} {_translator.GetText("Leaderboard_Games")}: {_factionGames} ({((float)_factionWins / (float)_factionGames) * 100:F2}%)" : $"{_translator.GetText("Faction_NR")} {_translator.GetText("Leaderboard_Games")}: {_factionGames}";

        _factionGames = statsFactions[3].Sum();
        _factionWins = statsFactions[3][0];
        ScoiataelStreak.text = $"{statsFactions[3][0]}  -  {statsFactions[3][2]}  -  {statsFactions[3][1]}";
        ScoiataelStreak.color = new Color(1.0f, 0.854f, 0.325f, 1.0f);
        ScoiataelLabel.text = _factionGames > 0 ? $"{_translator.GetText("Faction_ST")} {_translator.GetText("Leaderboard_Games")}: {_factionGames} ({((float)_factionWins / (float)_factionGames) * 100:F2}%)" : $"{_translator.GetText("Faction_ST")} {_translator.GetText("Leaderboard_Games")}: {_factionGames}";

        _factionGames = statsFactions[4].Sum();
        _factionWins = statsFactions[4][0];
        SkelligeStreak.text = $"{statsFactions[4][0]}  -  {statsFactions[4][2]}  -  {statsFactions[4][1]}";
        SkelligeLabel.text = _factionGames > 0 ? $"{_translator.GetText("Faction_SK")} {_translator.GetText("Leaderboard_Games")}: {_factionGames} ({((float)_factionWins / (float)_factionGames) * 100:F2}%)" : $"{_translator.GetText("Faction_SK")} {_translator.GetText("Leaderboard_Games")}: {_factionGames}";
        SkelligeStreak.color = new Color(1.0f, 0.854f, 0.325f, 1.0f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(FactionBars.GetComponent<RectTransform>());
    }

    private void SetBar(string faction, string type, float value)
    {
        if (type == "wins")
            bars[faction].Item2.fillAmount = value;
        else if (type == "draws")
            bars[faction].Item1.fillAmount = value;
    }

    public void ButtonPointerEnter(int id)
    {
        OpenFactionRankList(id);
    }

    public void ButtonPointerExit(int id)
    {
        CloseFactionRankList();
    }


    private void OpenFactionRankList(int buttonid)
    {
        FactionRankListHeader.sprite = factionHeaderSprites[buttonid];
        FactionBackgroundLogo.sprite = factionIcons[buttonid];


        string myNickname = playerNickname;

        FactionRankList.SetActive(true);

        var players = FactionRankList.transform.GetChild(2);

        int myNickIndex = -1;
        int my_i = -1;
        int loggedNickIndex = -1;
        int logged_i = -1;

        for (int i = 1; i < players.transform.childCount - 1; i++)
        {
            if (factionsRanks[buttonid].Count < i)
                break;
            int ranksIndex = i - 1;
            var playerData = factionsRanks[buttonid][ranksIndex];
            players.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = $"#{i}";
            players.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = $"{playerData.Item1}";
            players.transform.GetChild(i).GetChild(3).GetComponent<Text>().text = $"{playerData.Item2}";
            if (myNickname == playerData.Item1)
            {
                myNickIndex = ranksIndex;
                my_i = i;
            }
            if (_clientService.User.PlayerName == playerData.Item1){
                loggedNickIndex = ranksIndex;
                logged_i = i; 
            }
                players.transform.GetChild(i).GetChild(1).GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                players.transform.GetChild(i).GetChild(2).GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                players.transform.GetChild(i).GetChild(3).GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        if (loggedNickIndex != -1)
        {
            Color tintColor = new Color(0.993f, 0.785f, 0.135f, 1.0f);
            players.transform.GetChild(logged_i).GetChild(1).GetComponent<Text>().color = tintColor;
            players.transform.GetChild(logged_i).GetChild(2).GetComponent<Text>().color = tintColor;
            players.transform.GetChild(logged_i).GetChild(3).GetComponent<Text>().color = tintColor;
        }
        if(myNickIndex != -1 && myNickIndex != loggedNickIndex)
        {
            Color tintColor = new Color(1f, 0.455f, 0.027f, 1.0f);
            players.transform.GetChild(my_i).GetChild(1).GetComponent<Text>().color = tintColor;
            players.transform.GetChild(my_i).GetChild(2).GetComponent<Text>().color = tintColor;
            players.transform.GetChild(my_i).GetChild(3).GetComponent<Text>().color = tintColor;
        }
        if (myNickIndex == -1)
        {
            players.GetChild(11).gameObject.SetActive(true);
            
            int _factionRankPosition = factionsRanks[buttonid].FindIndex(t => t.Item1 == myNickname) + 1;
            players.transform.GetChild(11).GetChild(1).GetComponent<Text>().text = $"#{_factionRankPosition}";
            players.transform.GetChild(11).GetChild(2).GetComponent<Text>().text = $"{factionsRanks[buttonid][_factionRankPosition - 1].Item1}";
            players.transform.GetChild(11).GetChild(3).GetComponent<Text>().text = $"{factionsRanks[buttonid][_factionRankPosition - 1].Item2}";

            Color tintColor = new Color(1f, 0.455f, 0.027f, 1.0f);
            if (factionsRanks[buttonid][_factionRankPosition - 1].Item1 == _clientService.User.PlayerName)
            {
                tintColor = new Color(0.976f, 1.0f, 0.027f, 1.0f);
            }
            players.transform.GetChild(11).GetChild(1).GetComponent<Text>().color = tintColor;
            players.transform.GetChild(11).GetChild(2).GetComponent<Text>().color = tintColor;
            players.transform.GetChild(11).GetChild(3).GetComponent<Text>().color = tintColor;
        }
        else
            players.GetChild(11).gameObject.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(players.gameObject.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(FactionRankList.GetComponent<RectTransform>());

        players.gameObject.GetComponent<Canvas>().sortingOrder = 2;


    }

    private void CloseFactionRankList()
    {
        FactionRankList.SetActive(false);
    }

    
}
