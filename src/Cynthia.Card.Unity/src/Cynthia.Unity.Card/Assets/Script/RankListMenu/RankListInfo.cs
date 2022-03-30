using System.Collections;
using System.Collections.Generic;
using Cynthia.Card;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using Autofac;
using Cynthia.Card.Client;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using Assets.Script.Localization;
using UnityEngine.Events;
using static UnityEngine.UI.Scrollbar;
using Cynthia.Card.Common.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
public class RankListInfo : MonoBehaviour
{
    private GwentClientService _clientService;
    private GlobalUIService _globalUIService;
    private LocalizationService _translator;

    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _globalUIService = DependencyResolver.Container.Resolve<GlobalUIService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }

    void Start()
    {
    }

    public void OpenRankList(bool IsMoveLeftRight = true)
    {
        ResetEditor();
    }

    public void ResetEditor()
    {

    }
}
