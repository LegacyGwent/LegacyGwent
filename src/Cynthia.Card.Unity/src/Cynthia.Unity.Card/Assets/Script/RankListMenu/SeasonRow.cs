using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Autofac;
using Assets.Script.Localization;
using System;
using DG.Tweening;

public class SeasonRow : MonoBehaviour
{
    public Text SeasonName;
    public Text TextBeforeTime;
    public Text Time;
    public GameObject SeasonInformations;
    private DateTime endTime = DateTime.MinValue;
    private bool timerInvoked = false;
    private bool isTabOpened = true;
    private bool isMoving = false;
    private LocalizationService _translator;

    private void Start()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    public void TabClick()
    {
        if (isMoving)
            return;

        if (!isTabOpened)
            OpenTab();
        else
            CloseTab();
        isTabOpened = !isTabOpened;
    }

    private void OpenTab()
    {
        isMoving = true;
        SeasonInformations.SetActive(true);
        SeasonInformations.transform.DOLocalMoveY(0.0f, 2.0f, false).OnComplete(() => {
            isMoving = false;
        });
    }

    private void CloseTab()
    {
        isMoving = true;
        SeasonInformations.transform.DOLocalMoveY(970.0f, 2.0f, false).OnComplete(() =>
        {
            SeasonInformations.SetActive(false);
            isMoving = false;
        });
    }


    public void SetSeasonRow(string seasonName, string seasonEndTimerText, DateTime seasonEndTime, Color color, bool active)
    {
        if (seasonName == "")
            SeasonName.text = "NO_SEASON_DATA";
        else
            SeasonName.text = seasonName;

        Time.color = Color.white;
        SeasonName.color = color;
        TextBeforeTime.text = seasonEndTimerText;
        endTime = seasonEndTime;
        

        if (!timerInvoked && endTime > DateTime.UtcNow && active)
        {
            InvokeRepeating("UpdateTimer", 0f, 60f);
            timerInvoked = true;
        }
        else
        {
            if (endTime < DateTime.UtcNow || endTime > DateTime.UtcNow && !active)
            {
                if (timerInvoked)
                {
                    CancelInvoke("UpdateTimer");
                    timerInvoked = false;
                }
                Time.text = endTime.ToString("d");
            }
        }

    }

    public void SetTransparency()
    {
        SeasonName.color = Color.clear;
        TextBeforeTime.color = Color.clear;
        Time.color = Color.clear;
    }

    private void UpdateTimer()
    {
        TimeSpan timeToEnd = endTime - DateTime.UtcNow;
        List<string> parts = new List<string>();

        if (timeToEnd.Days > 0)
            parts.Add($"{timeToEnd.Days}d");
        if (timeToEnd.Hours > 0)
            parts.Add($"{timeToEnd.Hours}h");
        if (timeToEnd.Minutes > 0)
            parts.Add($"{timeToEnd.Minutes}m");
        //if (timeToEnd.Seconds > 0)
        //    parts.Add($"{timeToEnd.Seconds}s");

        Time.text = parts.Count > 0 ? string.Join(" : ", parts) : "0s";
        if (timeToEnd < TimeSpan.FromDays(7))
            Time.color = Color.red;
        else if (timeToEnd < TimeSpan.FromDays(14))
            Time.color = Color.yellow;
    }
    private void OnDisable()
    {
        CancelInvoke("UpdateTimer");
        timerInvoked = false;
    }
}
