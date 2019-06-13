using System;
using System.Threading.Tasks;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.Audio;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card.Client
{
    [Transient]
    public class MainCodeService
    {
        private GameObject _code;

        public MainCodeService()
        {
            _code = GameObject.Find("Code");
        }
        public T GetCode<T>()
        {
            return _code.GetComponent<T>();
        }
        //
        public void ClickEditorListLeader(string id)
        {//点击了编辑列表领袖
            _code.GetComponent<MainCode>().EditorMenu.ClickEditorListLeader(id);
        }
        public void ClickEditorListCard(string id)
        {//点击了编辑列表卡牌
            _code.GetComponent<MainCode>().EditorMenu.ClickEditorListCard(id);
        }
        public void ClickEditorUICoreCard(CardStatus card)
        {//点击了编辑菜单卡牌
            _code.GetComponent<MainCode>().EditorMenu.ClickEditorUICoreCard(card);
        }
        //
        public void AddDeckClick()
        {
            _code.GetComponent<MainCode>().EditorMenu.AddDeckClick();
        }

        public void ClickSwitchUICard(CardStatus card)
        {
            _code.GetComponent<MainCode>().EditorMenu.ClickSwitchUICard(card);
        }

        public void SelectSwitchUICard(CardStatus card, bool isOver = true)
        {
            _code.GetComponent<MainCode>().EditorMenu.SelectSwitchUICard(card, isOver);
        }

        public void SetMatchDeckList(IList<DeckModel> decks)
        {
            _code.GetComponent<MainCode>().MatchUI.GetComponent<MatchInfo>().SetDeckList(decks);
        }

        public void SetDeck(DeckModel deck,string id)
        {
            _code.GetComponent<MainCode>().MatchUI.GetComponent<MatchInfo>().SetDeck(deck,id);
        }

        public void SwitchDeckOpen()
        {
            _code.GetComponent<MainCode>().MatchUI.GetComponent<MatchInfo>().SwitchDeckOpen();
        }

        public void MatchReset()
        {
            _code.GetComponent<MainCode>().MatchUI.GetComponent<MatchInfo>().MatchReset();
        }

        public void SwitchDeckClose()
        {
            _code.GetComponent<MainCode>().MatchUI.GetComponent<MatchInfo>().SwitchDeckClose();
        }
    }
}
