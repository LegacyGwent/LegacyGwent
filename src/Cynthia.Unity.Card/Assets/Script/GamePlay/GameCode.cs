using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Autofac;
using Cynthia.Card.Client;
using Cynthia.Card;
using System.Threading.Tasks;

public class GameCode : MonoBehaviour
{
    public GameUIControl GameUIControl;
    public GameResultControl GameResultControl;
    public GameCardsControl GameCardsControl;
    public GameCardShowControl GameCardShowControl;
    public BigRoundControl BigRoundControl;
    public GameEvent GameEvent;
    public Animator MyRoundShow;
    public Transform GameScale;

    private void Start()
    {
        GameStart();
    }
    private async void GameStart()
    {
        await DependencyResolver.Container.Resolve<GwentClientGameService>().Play(DependencyResolver.Container.Resolve<GwentClientService>().Player);
    }
    public void LeaveGame()
    {
        SceneManager.LoadScene("Game");
    }
}
