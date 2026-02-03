using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card.Client;
using Cynthia.Card;
using Autofac;
using System.Linq;
using System.Threading.Tasks;

public class DeckShufler : MonoBehaviour
{
    private GwentClientService _clientService;
    private EditorInfo _editorInfo;

    // Step 1: declare x and y
    public static string x = null;
    public static string y = null;

    void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _editorInfo = FindObjectOfType<EditorInfo>();
    }

    public async void DoStuff()
    {
        
        
    }
    
    public void GetClick(string id)
    {
        var decks = _clientService.User.Decks.ToList();
        _clientService.User.Decks = decks;
        _editorInfo.SetDeckList(decks);
        
        //Debug.Log("Received ID: " + id);

        if (x == null)
        {
            x = id;
            Debug.Log("Set as first deck "+id);
        }
        else if (y == null)
        {
            if (id != x)
            {
                y = id;
                Debug.Log("Set as second deck "+id);
            }
            else
            {
                Debug.Log("Clicked same deck again");
            }

        }

        if (x != null && y != null)
        {
            //Debug.Log("Swapping");
            int indexX = decks.FindIndex(d => d.Id == x);
            int indexY = decks.FindIndex(d => d.Id == y);

            if (indexX != -1 && indexY != -1)
            {
                // Swap the deck objects
                var temp = decks[indexX];
                decks[indexX] = decks[indexY];
                decks[indexY] = temp;

                // Update the user decks and the editor UI
                _clientService.User.Decks = decks;
                _editorInfo.SetDeckList(decks);

                Debug.Log($"Swapped decks at indexes {indexX} and {indexY}");

                // Optional: save to server immediately
                
                //await SaveDecksToServer(decks);
            }
            else
            {
                Debug.LogWarning($"Could not find decks to swap: {x}, {y}");
            }

                x = null;
                y = null;
            SaveDecksToServer(decks,_clientService);
        }     
    }
    public async Task SaveDecksToServer( List<DeckModel> decks, GwentClientService _clientService)
    {  
        Debug.Log("Saving Decks to Server");

        var deckIds = decks.Select(d => d.Id).ToList();

        foreach (var i in deckIds)
        {
            await _clientService.RemoveDeck(i);
        }
        foreach (var i in decks)
        {
            await _clientService.AddDeck(i);
        }
        Debug.Log(decks.GetType().FullName);
    }

}