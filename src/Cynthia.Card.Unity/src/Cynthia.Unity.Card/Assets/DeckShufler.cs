using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card.Client;
using Autofac;
using System.Linq;

public class DeckShufler : MonoBehaviour
{
    private GwentClientService _clientService;
    private EditorInfo _editorInfo;

    // Step 1: declare x and y
    public int x = 0;
    public int y = 2;

    void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _editorInfo = FindObjectOfType<EditorInfo>();
    }

    public async void DoStuff()
    {
        Debug.Log("SHUFLING");

        // Step 2: swap decks at positions x and y
        var decks = _clientService.User.Decks.ToList(); // make a copy
        if (x < decks.Count && y < decks.Count)
        {
            var temp = decks[x];
            decks[x] = decks[y];
            decks[y] = temp;
        }

        _clientService.User.Decks = decks;
        _editorInfo.SetDeckList(decks);

        // Step 3: remove all decks from server
        var deckIds = decks.Select(d => d.Id).ToList();
        foreach (var id in deckIds)
        {
            await _clientService.RemoveDeck(id);
        }

        // Step 4: re-add decks in new order
        foreach (var deck in decks)
        {
            await _clientService.AddDeck(deck);
        }
    }
}