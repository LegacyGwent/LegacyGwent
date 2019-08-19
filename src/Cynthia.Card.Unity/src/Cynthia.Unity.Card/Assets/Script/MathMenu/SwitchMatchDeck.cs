using Autofac;
using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMatchDeck : MonoBehaviour
{
    private int _id;
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
    }
    public void OnClick()
    {
        _codeService.SetDeck(_client.User.Decks[_id], _client.User.Decks[_id].Id);
        GlobalState.DefaultDeckId = _client.User.Decks[_id].Id;
        _codeService.SwitchDeckClose();
    }
}
