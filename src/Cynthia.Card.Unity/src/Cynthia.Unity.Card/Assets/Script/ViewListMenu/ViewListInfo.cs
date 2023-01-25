using System.Collections.Generic;
using UnityEngine;
using Autofac;
using Cynthia.Card.Client;
using System;

public class ViewListInfo : MonoBehaviour
{
    public GameObject ViewListUI;
    public GameObject ViewListUIBody;
    public GameObject ViewListRowPrefab;
    private GwentClientService _clientService;
    private IList<Tuple<string, string, string>> ViewList;
    private int currentIndex = 0;
    private int totalCount = 0;
    private const int PageRowCount = 10;
    private int currentPageRowCount = 10;
    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
    }

    public async void OpenViewList()
    {
        ViewListUI.SetActive(true);

        var (_, vsHumanList, vsAiList) = await _clientService.GetUsers();
        ViewList = vsHumanList;
        if (vsAiList != null)
        {
            foreach (var item in vsAiList)
            {
                ViewList.Add(item);
            }
        }
        CleanUpRow();
        totalCount = ViewList.Count;
        currentIndex = 0;
        GenerateViewListRow();
    }
    public void CleanUpRow()
    {
        foreach (Transform child in ViewListUIBody.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void CloseViewList()
    {
        CleanUpRow();
        ViewListUI.SetActive(false);
    }
    public void GenerateViewListRow()
    {
        currentPageRowCount = 0;
        for (; currentIndex < totalCount && currentPageRowCount < PageRowCount; currentIndex++)
        {
            var item = ViewList[currentIndex];
            var row = Instantiate(ViewListRowPrefab, ViewListUIBody.transform);
            row.GetComponent<ViewListRow>().SetViewListRow(currentIndex + 1, item.Item1 + " vs " + item.Item2, item.Item3);
            currentPageRowCount++;
        }
        for (int extraCount = 0; currentPageRowCount + extraCount < PageRowCount; extraCount++)
        {
            var row = Instantiate(ViewListRowPrefab, ViewListUIBody.transform);
            row.GetComponent<ViewListRow>().SetViewListRow(0, "-", "");
            row.GetComponent<ViewListRow>().SetTransparency();
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
        GenerateViewListRow();
    }
    public void NextPageClick()
    {
        if (currentIndex == totalCount)
        {
            return;
        }
        CleanUpRow();
        GenerateViewListRow();
    }
}
