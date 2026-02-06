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
    
    public void GetClick(string id)
    {
        var decks = _clientService.User.Decks.ToList();
        _clientService.User.Decks = decks;
        _editorInfo.SetDeckList(decks);
        
        //Debug.Log("Received ID: " + id);

        if (x == null)
        {
            x = id;
            //Debug.Log("Set as first deck "+id);
        }
        else if (y == null)
        {
            if (id != x)
            {
                y = id;
                //Debug.Log("Set as second deck "+id);
            }
            else
            {
                //Debug.Log("Clicked same deck again");
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

                //Debug.Log($"Swapped decks at indexes {indexX} and {indexY}");

                // Optional: save to server immediately
                
                //await SaveDecksToServer(decks);
            }

            SwapOnServer(x,y);

            x = null;
            y = null;
        }     
    }
    public async Task<bool> SwapOnServer(string firstDeckId, string secondDeckId)
    {
        return await _clientService.SwapDecks(firstDeckId, secondDeckId);
    }
    


}