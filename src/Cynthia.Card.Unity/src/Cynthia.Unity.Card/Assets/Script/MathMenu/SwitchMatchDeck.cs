using Autofac;
using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwitchMatchDeck : MonoBehaviour
{
    private int _id;
    private string _deckId;
    private GwentClientService _client;
    private MainCodeService _codeService;

    private void Start()
    {
        _client = DependencyResolver.Container.Resolve<GwentClientService>();
        _codeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    public void SetId(int id)
    {
        _id = id;
        _deckId = null;
    }
    public void SetDeckId(string deckId)
    {
        _deckId = deckId;
    }
    public void OnClick()
    {
        var deck = string.IsNullOrWhiteSpace(_deckId)
            ? _client.User.Decks[_id]
            : _client.User.Decks.FirstOrDefault(x => x.Id == _deckId);
        if (deck == null) return;
        _codeService.SetDeck(deck, deck.Id);
        ClientGlobalInfo.DefaultDeckId = deck.Id;
        _codeService.SwitchDeckClose();
    }
}
