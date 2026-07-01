using System.Collections.Generic;
using UnityEngine;
using Autofac;
using Cynthia.Card.Client;
using Cynthia.Card.Common.Models;
using System;
using System.Threading.Tasks;
using Assets.Script.Localization;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using System.Security.Cryptography.X509Certificates;

public class RankListInfo : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject RankListUI;
    public GameObject RankListUIBody;
    public GameObject SeasonRowUI;
    public GameObject RankListRowPrefab;
    public GameObject PlayersPositionsTable;
    public GameObject PositionRewardsTable;
    public GameObject RewardTableElementPrefab;
    public GameObject PlayerTableElementPrefab;
    public GameObject LoggedPlayerTableElement;
    public GameObject HeaderPositionTable;
    public GameObject HeaderRewardsTable;
    public GameObject ScrollContent;
    public GameObject[] sideButtons;
    public ScrollRect scrollRect;
    public GameObject RankPlayerScreen;
    public Button SeasonBackButton;
    public Button SeasonNextButton;
    public Image Background2;
    private IList<GameObject> LeaderBoardTabs;
    private IList<GameObject> RewardTableTabs;
    private SeasonRow seasonRowScript;
    private GwentClientService _clientService;
    private IList<Tuple<string, string, string, string, int, int, IList<int[]>>> rankList;
    private IList<SeasonReward> seasonRewards;
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; }
    private int myMMR = 0;
    private int myPeakMMR = 0;
    private int[] myStreak = new int[3];
    private string myAvatar = "NoAvatar";
    private string myBorder = "NoBorder";
    private int currentIndex = 0;
    private Func<MessageBox> _messageBox;
    private int PageRowCount = 10;
    //private int currentPageRowCount = 0;
    private SeasonInfo seasonData;
    private SeasonInfo previousSeasonData;
    private SeasonInfo nextSeasonData;
    private LocalizationService _translator { get => DependencyResolver.Container.Resolve<LocalizationService>(); }
    private Dictionary<int, List<Tuple<string, int>>> factionsRanking;
    private int activeSeasonID;
    private int openedSeasonID;
    private bool _displayLoggedPlayerElement = true;
    private int totalCount = 0;
    private const int MAX_ROWS = 300;
    private IList<SeasonInfo> allSeasons;
    private void Awake()
    {
        seasonRowScript = SeasonRowUI.GetComponent<SeasonRow>();

        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
    }

    private void Start()
    {
        LeaderBoardTabs = new List<GameObject>() { PlayersPositionsTable, HeaderPositionTable };
        RewardTableTabs = new List<GameObject>() { PositionRewardsTable, HeaderRewardsTable };
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private bool clicked = false;
    public async void RankListMenuButton(){
        if (!clicked)
        {
            clicked = true;
            var clientService = DependencyResolver.Container.Resolve<GwentClientService>();
            CleanUpRow();
            Background2.enabled = true;

    
            DOTween.Kill("BackgroundFade");

            Color c = Background2.color;
            c.a = 1f;
            Background2.color = c;

            await OpenRankList(true);
            clicked = false;        
        }

    }

    public async Task OpenRankList(bool openActiveSeason = true, IList<Tuple<string, string, string, string, int, int, IList<int[]>>> rankingList = null)
    {
        MainUI.SetActive(false);
        RankListUI.SetActive(true);

        ResetScrollPosition();

        if (openActiveSeason)
        {
            var _activeRankList = await _clientService.GetAllMMR(0, 300);
            await AssignSeasonData(_activeRankList);
            await LoadSeasonInfoAsync();
        }
        else if (rankingList != null)
            await AssignSeasonData(rankingList);
        else{
            return;
        }


        CleanUpRow();
        await GenerateRankListRows();
        await GenerateRewardsTable();

        if (rankList != null && rankList.Count > 0)
        {
            sideButtons[0].SetActive(true);
            LeaderboardButtonClicked();
        }
        else
        {
            sideButtons[0].SetActive(false);
            RewardsButtonClicked();
        }

        Background2.DOFade(0f, 0.7f).SetId("BackgroundFade").OnComplete(() => Background2.enabled = false);


    }

    private async Task<bool> AssignSeasonData(IList<Tuple<string, string, string, string, int, int, IList<int[]>>> rankingList)
    {
        rankList = rankingList;

        var myMMRandPeak = await _clientService.GetPalyernameMMRandPeak(_clientService.User.PlayerName);
        myMMR = myMMRandPeak.Item1;
        myPeakMMR = myMMRandPeak.Item2;
        myStreak = await _clientService.GetPlayernameStreak(_clientService.User.PlayerName);
        myAvatar = _clientService.User.CurrentAvatar;
        myBorder = _clientService.User.CurrentBorder;
        currentIndex = 0;
        factionsRanking = null;

        return true;
    }

    public void CleanUpRow()
    {
        foreach (Transform child in RankListUIBody.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in PlayersPositionsTable.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in PositionRewardsTable.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //currentPageRowCount = 0;
        PageRowCount = 10;
    }
    public void AddSelfRow()
    {
        LoggedPlayerTableElement.SetActive(true);
        _displayLoggedPlayerElement = true;
        if (rankList == null || rankList.Count == 0)
        {
            LoggedPlayerTableElement.SetActive(false);
            return;
        }
        if(rankList.Count < 7)
            PlayersPositionsTable.GetComponent<VerticalLayoutGroup>().padding.bottom = 0;
        else
            PlayersPositionsTable.GetComponent<VerticalLayoutGroup>().padding.bottom = 100;
        int index = 0;
        for (; index < rankList.Count; index++)
        {
            if (rankList[index].Item1 == _clientService.User.PlayerName)
                break;
        }
        if (index == rankList.Count)
        {
            if (openedSeasonID != activeSeasonID)
            {
                LoggedPlayerTableElement.SetActive(false);
                _displayLoggedPlayerElement = false;
                PlayersPositionsTable.GetComponent<VerticalLayoutGroup>().padding.bottom = 0;
                return;
            }
            LoggedPlayerTableElement.GetComponent<PlayerTableRowScript>().SetRankListRowString("300+", _clientService.User.PlayerName, myAvatar, myBorder, myMMR, myPeakMMR, myStreak, true);
        }
        else
        {
            var streakSum = rankList[index].Item7.Aggregate((a, b) => a.Zip(b, (x, y) => x + y).ToArray());
            LoggedPlayerTableElement.GetComponent<PlayerTableRowScript>().SetRankListRow(index + 1, _clientService.User.PlayerName, rankList[index].Item2, rankList[index].Item3, rankList[index].Item5, rankList[index].Item6, streakSum, true);
        }
    }
    public void CloseRankList()
    {
        CleanUpRow();
        RankListUI.SetActive(false);
    }


    public async Task GenerateRankListRows()
    {
        if(rankList == null)
            return;
        int endIndex = Mathf.Min(currentIndex + PageRowCount, rankList.Count);

        foreach (var tab in LeaderBoardTabs)
        {
            tab.SetActive(true);
        }

        for (; currentIndex < endIndex; currentIndex++)
        {
            GenerateRankListRow(rankList[currentIndex], currentIndex, (currentIndex % 2 != 0));
        }
        

        AddSelfRow();

        LayoutRebuilder.ForceRebuildLayoutImmediate(PositionRewardsTable.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(PlayersPositionsTable.GetComponent<RectTransform>());
    }

    private void OpenPlayerRankScreen(Tuple<string, string, string, string, int, int, IList<int[]>> playerData, int rankNumber)
    {

        RankPlayerScreen.SetActive(true);
        RankPlayerScreen.transform.localScale = Vector3.one * 0.01f;
        RankPlayerScreen.transform.DOScale(Vector3.one * 1.0f, 0.5f);
        var rankScreenScript = RankPlayerScreen.GetComponent<RankPlayerScreenScript>();
        rankScreenScript.SetPlayerInfo(playerData.Item2, playerData.Item3, playerData.Item1, playerData.Item4, playerData.Item5, playerData.Item6, playerData.Item7, rankNumber, Season.CalculateFactions(rankList));
    }

    private void GenerateRankListRow(Tuple<string, string, string, string, int, int, IList<int[]>> playerData, int rankNumber, bool darker)
    {
        var item = playerData;
        var row = Instantiate(PlayerTableElementPrefab, PlayersPositionsTable.transform);
        var rowScript = row.GetComponent<PlayerTableRowScript>();
        rowScript.SetRankListRow(rankNumber + 1, item.Item1, item.Item2, item.Item3, item.Item5, item.Item6, SumStreaks(item.Item7));
        rowScript.Background.color = new Color(rowScript.Background.color.r, rowScript.Background.color.g, rowScript.Background.color.b, darker ? 0.2f : 0f);
        rowScript.playerButton.onClick.AddListener(() =>
        {
            EventSystem.current.SetSelectedGameObject(null);
            OpenPlayerRankScreen(playerData, rankNumber + 1);
        });
    }

    public async Task GenerateRewardsTable()
    {
        if (seasonRewards == null)
            return;
        foreach (var tab in RewardTableTabs)
        {
            tab.SetActive(true);
        }

        // Only display season-end (position-based) rewards here.
        foreach (var _reward in seasonRewards.Where(r => !r.isInSeasonReward))
        {
            var rewardTable = Instantiate(RewardTableElementPrefab, PositionRewardsTable.transform);
            var rewardElementScript = rewardTable.GetComponent<RewardTableElement>();
            rewardElementScript.GenerateReward(_reward);
        }

    }

    public async Task LoadSeasonInfoAsync(bool active = true, int seasonId = -1, int buttondirection = 0)
    {
        try
        {
            if (allSeasons == null)
            {
                allSeasons = (await DependencyResolver.Container.Resolve<GwentClientService>().GetSeasons())
                    .OrderBy(x => x.SeasonId)
                    .ToList();

                foreach (var item in allSeasons)
                {
                    UnityEngine.Debug.Log($"{item.SeasonId} / {item.SeasonName} active={item.isActive}");
                }
                var firstActiveSeason = allSeasons.FirstOrDefault(x => x.isActive);
                if (firstActiveSeason != null)
                    activeSeasonID = firstActiveSeason.SeasonId;
            }



            var lastLoadedSeason = seasonData;

            bool openedActiveSeason = active;

            if (buttondirection != 0)
            {
                if (buttondirection == 1 && nextSeasonData != null)
                {
                    seasonData = nextSeasonData;
                }
                if (buttondirection == -1 && previousSeasonData != null)
                {
                    seasonData = previousSeasonData;
                }
                openedActiveSeason = seasonData.SeasonId == activeSeasonID;
            }

            if (openedActiveSeason)
            {
                if (buttondirection == 0)
                    seasonData = allSeasons.FirstOrDefault(x => x.isActive);
                activeSeasonID = seasonData.SeasonId;
            }
            else
            {
                if (buttondirection == 0)
                {
                    var _newSeasonData = allSeasons.FirstOrDefault(x => x.isActive == active && x.SeasonId == seasonId);
                    if (_newSeasonData != null)
                    {
                        seasonData = _newSeasonData;
                    }
                }

            }
            openedSeasonID = seasonData.SeasonId;
            if (openedSeasonID > 0)
                seasonRewards = await _clientService.GetSeasonRewards(openedSeasonID, "all");
            //back button
            if (buttondirection == -1)
                nextSeasonData = lastLoadedSeason;
            else
                nextSeasonData = allSeasons.FirstOrDefault(x => x.SeasonId == openedSeasonID + 1);
            //next button
            if (buttondirection == 1)
                previousSeasonData = lastLoadedSeason;
            else
                previousSeasonData = allSeasons.FirstOrDefault(x => x.SeasonId == openedSeasonID - 1);


            int previousSeasonDataId = previousSeasonData != null ? previousSeasonData.SeasonId : -1;
            int nextSeasonDataId = nextSeasonData != null ? nextSeasonData.SeasonId : -1;

            if (nextSeasonData != null)
                UnityEngine.Debug.Log($"next season id: {nextSeasonData.SeasonId}");
            if (previousSeasonData != null)
                UnityEngine.Debug.Log($"previous season id: {previousSeasonData.SeasonId}");

            SeasonBackButton.gameObject.SetActive(previousSeasonDataId != -1);
            SeasonNextButton.gameObject.SetActive(nextSeasonDataId != -1);

            string _seasondEndTimerText =
                activeSeasonID > seasonData.SeasonId ? _translator.GetText("Season_EndTimer_Finished")
                : activeSeasonID == seasonData.SeasonId ? _translator.GetText("Season_EndTimer_Current")
                : _translator.GetText("Season_EndTimer_Upcoming");

            string _seasonName = seasonData.SeasonName;
            DateTime _seasonEndTime = seasonData.SeasonEndTime;
            string _seasonColor = seasonData.SeasonColor;
            int _seasonId = seasonData.SeasonId;

            if (_seasonId != -1)
                seasonRowScript.SetSeasonRow(_translator.GetText(_seasonName), _seasondEndTimerText, _seasonEndTime, mycolormap[_seasonColor], activeSeasonID == _seasonId);
        }
        catch
        {
            Debug.Log("Couldn't load season info from server.");
            string _seasonName = "Season_SeasonOfMahakam";
            DateTime _seasonEndTime = new DateTime(2025, 02, 27, 0, 0, 0, DateTimeKind.Utc);
            string _seasonColor = "lightblue";
            string _seasondEndTimerText = _translator.GetText("Season_EndTimer_Current");
            int _seasonId = 0;

            if (_seasonId != -1)
                seasonRowScript.SetSeasonRow(_translator.GetText(_seasonName), _seasondEndTimerText, _seasonEndTime, mycolormap[_seasonColor], activeSeasonID == _seasonId);

        }


    }

    public void LeaderboardButtonClicked()
    {
        foreach (var tab in LeaderBoardTabs)
        {
            tab.SetActive(true);
        }
        LoggedPlayerTableElement.SetActive(_displayLoggedPlayerElement);
        foreach (var tab in RewardTableTabs)
        {
            tab.SetActive(false);
        }

        for (int i = 0; i < sideButtons.Length; i++)
        {
            if (i == 0)
                sideButtons[i].transform.GetChild(1).GetComponent<Text>().color = new Color(222f / 255f, 193f / 255f, 146f / 255f);
            else
                sideButtons[i].transform.GetChild(1).GetComponent<Text>().color = new Color(166f / 255f, 129f / 255f, 68f / 255f);
        }

        ResetScrollPosition();

    }


    public async void RewardsButtonClicked()
    {
        foreach (var tab in RewardTableTabs)
        {
            tab.SetActive(true);
        }
        foreach (var tab in LeaderBoardTabs)
        {
            tab.SetActive(false);
        }
        LoggedPlayerTableElement.SetActive(false);

        for (int i = 0; i < sideButtons.Length; i++)
        {
            if (i == 1)
                sideButtons[i].transform.GetChild(1).GetComponent<Text>().color = new Color(222f / 255f, 193f / 255f, 146f / 255f);
            else
                sideButtons[i].transform.GetChild(1).GetComponent<Text>().color = new Color(166f / 255f, 129f / 255f, 68f / 255f);
        }

        ResetScrollPosition();
    }

    public async void SeasonBackButtonClick()
    {
        if (seasonData != null)
        {
            var _newSeasonId = seasonData.SeasonId - 1;
            bool isActive = activeSeasonID == _newSeasonId;
            await LoadSeasonInfoAsync(isActive, _newSeasonId);
            await OpenRankList(isActive, Season.DecodeRanklist(seasonData.rankingHistory));
        }
    }
    public async void SeasonNextButtonClick()
    {
        if (seasonData != null)
        {
            var _newSeasonId = seasonData.SeasonId + 1;
            await LoadSeasonInfoAsync(false, _newSeasonId, 1);
            bool isActive = activeSeasonID == seasonData.SeasonId;
            await OpenRankList(isActive, Season.DecodeRanklist(seasonData.rankingHistory));
        }
    }

    private void ResetScrollPosition()
    {
        ScrollContent.transform.position = new Vector3(ScrollContent.transform.position.x, 0.0f, ScrollContent.transform.position.z);
    }

    private bool isLoadingMore = false;

    async void OnScrollChanged(Vector2 scrollPosition)
    {
        if (!isLoadingMore && PlayersPositionsTable.activeSelf && scrollPosition.y <= 0.15f)
        {
            isLoadingMore = true;
            await LoadMoreRowsAsync();
            
            //Background2.DOFade(0f, 0.7f).SetId("BackgroundFade").OnComplete(() => Background2.enabled = false);
            

        }
    }

    async Task LoadMoreRowsAsync()
    {
        PageRowCount += 10;
        await GenerateRankListRows();
        await Task.Delay(400);
        isLoadingMore = false;
        OnScrollChanged(new Vector2(0, scrollRect.verticalNormalizedPosition));
    }

    int[] SumStreaks(IList<int[]> streaksList)
    {
        
        int[] result = new int[3];

        if (streaksList == null)
            return result;

            foreach (var array in streaksList)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    result[i] += array[i];
                }
            }
        return result;
    }

}
