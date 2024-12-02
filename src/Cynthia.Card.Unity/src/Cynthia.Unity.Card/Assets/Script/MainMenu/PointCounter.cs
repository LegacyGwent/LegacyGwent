using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using System.Linq;
using Assets.Script.Localization;
using System.Threading.Tasks;



public class PointCounter : MonoBehaviour
{
    
    [SerializeField] counterHUD counterHUD;
    public Text PlayerCounttNum;
    //-----------------------------------
    private LocalizationService _translator;
    private GwentClientService server;
    public float timer=-0;
    public float interval=-5;
    void Update()
    {
        if (timer<interval)
        {
            timer=timer+Time.deltaTime;
        }
        else
        {
            CountPoints();
            timer=0;
        }
    }
    
    private async void Start ()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        server = DependencyResolver.Container.Resolve<GwentClientService>();
        
    }

    private async void CountPoints()
    {
            int HUD =  await server.GetUserCount();
            await Task.Delay(5);
            counterHUD.Points = HUD;
            await Task.CompletedTask;
            return;
    }
}