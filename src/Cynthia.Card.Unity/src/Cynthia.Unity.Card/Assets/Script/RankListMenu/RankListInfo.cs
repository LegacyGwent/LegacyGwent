using System.Collections.Generic;
using UnityEngine;
using Autofac;
using Cynthia.Card.Client;
using System;
using Assets.Script.Localization;
public class RankListInfo : MonoBehaviour
{
    public GameObject RankListUI;
    public GameObject RankListUIBody;
    public GameObject RankListRowPrefab;
    private GwentClientService _clientService;
    private IList<Tuple<string, int>> rankList;
    private int currentIndex = 0;
    private int totalCount = 0;
    private const int PageRowCount = 10;
    private int currentPageRowCount = 10;
    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
    }

    public async void OpenRankList()
    {
        RankListUI.SetActive(true);

        rankList = await _clientService.GetAllMMR(0, 50);
        totalCount = rankList.Count;
        currentIndex = 0;
        GenerateRankListRow();
    }
    public void CleanUpRow()
    {
        foreach (Transform child in RankListUIBody.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void CloseRankList()
    {
        CleanUpRow();
        RankListUI.SetActive(false);
    }
    public void GenerateRankListRow()
    {
        currentPageRowCount = 0;
        for (; currentIndex < totalCount && currentPageRowCount < PageRowCount; currentIndex++)
        {
            var item = rankList[currentIndex];
            var row = Instantiate(RankListRowPrefab, RankListUIBody.transform);
            row.GetComponent<RankListRow>().SetRankListRow(currentIndex + 1, item.Item1, item.Item2);
            currentPageRowCount++;
        }
    }

    public void PreviousPageClick()
    {
        if (currentIndex - (PageRowCount + currentPageRowCount) < 0)
        {
            return;
        }
        CleanUpRow();
        currentIndex -= PageRowCount + currentPageRowCount;
        GenerateRankListRow();

    }
    public void NextPageClick()
    {
        if (currentIndex == totalCount)
        {
            return;
        }
        CleanUpRow();
        GenerateRankListRow();

    }
}
