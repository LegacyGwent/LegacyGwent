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

        private bool TryGetMainCode(out MainCode mainCode)
        {
            // Pointer-exit callbacks may arrive while Game.unity is unloading.
            // Unity's destroyed-object null semantics let us safely reacquire Code
            // after a scene change and quietly ignore the final stale callback.
            if (_code == null) _code = GameObject.Find("Code");
            mainCode = _code == null ? null : _code.GetComponent<MainCode>();
            return mainCode != null;
        }

        public T GetCode<T>()
        {
            if (_code == null) _code = GameObject.Find("Code");
            return _code == null ? default(T) : _code.GetComponent<T>();
        }
        //
        public void ClickEditorListLeader(string id)
        {//点击了编辑列表领袖
            if (TryGetMainCode(out var code)) code.EditorMenu.ClickEditorListLeader(id);
        }
        public void ClickEditorListCard(string id)
        {//点击了编辑列表卡牌
            if (TryGetMainCode(out var code)) code.EditorMenu.ClickEditorListCard(id);
        }
        public void ClickEditorUICoreCard(CardStatus card)
        {//点击了编辑菜单卡牌
            if (TryGetMainCode(out var code)) code.EditorMenu.ClickEditorUICoreCard(card);
        }
        //
        public void AddDeckClick()
        {
            if (TryGetMainCode(out var code)) code.EditorMenu.AddDeckClick();
        }

        public void ClickSwitchUICard(CardStatus card)
        {
            if (TryGetMainCode(out var code)) code.EditorMenu.ClickSwitchUICard(card);
        }

        public void SelectSwitchUICard(CardStatus card, bool isOver = true)
        {
            if (TryGetMainCode(out var code)) code.EditorMenu.SelectSwitchUICard(card, isOver);
        }
        public void SetMatchArtCard(CardStatus card, bool isOver = true)
        {
            if (TryGetMainCode(out var code)) code.MatchUI.GetComponent<MatchInfo>().SetMatchArtCard(card, isOver);
        }
        public void SetMatchDeckList(IList<DeckModel> decks)
        {
            if (TryGetMainCode(out var code)) code.MatchUI.GetComponent<MatchInfo>().SetDeckList(decks);
        }

        public void SetDeck(DeckModel deck, string id)
        {
            if (TryGetMainCode(out var code)) code.MatchUI.GetComponent<MatchInfo>().SetDeck(deck, id);
        }

        public void SwitchDeckOpen()
        {
            if (TryGetMainCode(out var code)) code.MatchUI.GetComponent<MatchInfo>().SwitchDeckOpen();
        }

        public void MatchReset()
        {
            if (TryGetMainCode(out var code)) code.MatchUI.GetComponent<MatchInfo>().MatchReset();
        }

        public void SwitchDeckClose()
        {
            if (TryGetMainCode(out var code)) code.MatchUI.GetComponent<MatchInfo>().SwitchDeckClose();
        }
    }
}
