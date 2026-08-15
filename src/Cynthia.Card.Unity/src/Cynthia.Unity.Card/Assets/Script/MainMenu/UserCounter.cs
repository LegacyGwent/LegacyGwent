using UnityEngine;
using Cynthia.Card.Client;
using System;
using Autofac;
using System.Threading.Tasks;

public class UserCounter : MonoBehaviour
{
    [SerializeField] counterHUD counterHUD;
    [SerializeField] displayflag displayflag;

    private GwentClientService server;
    private float timer;
    private const float interval = 1;
    private bool refreshInProgress;
    private bool refreshErrorLogged;

    void Update()
    {
        if (server == null || refreshInProgress)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            RefreshCounters();
        }
    }

    private void Start()
    {
        try
        {
            server = DependencyResolver.Container?.Resolve<GwentClientService>();
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"UserCounter disabled because the client service is unavailable: {exception.Message}");
        }

        // Refresh immediately after a clean scene start instead of waiting one second.
        timer = interval;
    }

    private async void RefreshCounters()
    {
        refreshInProgress = true;
        try
        {
            Task<int> usersTask = server.GetUserCount();
            Task<int> usersInMatchTask = server.GetUsersInMatchCount();
            Task<int> usersVsAiTask = server.GetUsersvsAICount();
            Task<int> usersInRankedTask = server.GetUsersInRankedCount();
            Task<int> usersInCasualTask = server.GetUsersInCasualCount();
            Task<int> casualQueueTask = server.GetIsCasualQueue();
            Task<int> rankQueueTask = server.GetIsRankQueue();

            await Task.WhenAll(
                usersTask,
                usersInMatchTask,
                usersVsAiTask,
                usersInRankedTask,
                usersInCasualTask,
                casualQueueTask,
                rankQueueTask);

            if (counterHUD != null)
            {
                counterHUD.Users = usersTask.Result;
                counterHUD.UsersInMatch = usersInMatchTask.Result;
                counterHUD.UsersvsAI = usersVsAiTask.Result;
                counterHUD.UsersInRanked = usersInRankedTask.Result;
                counterHUD.UsersInCasual = usersInCasualTask.Result;
            }

            if (displayflag != null)
            {
                displayflag.IsCasualFlag = casualQueueTask.Result == 1;
                displayflag.IsRankFlag = rankQueueTask.Result == 1;
            }

            refreshErrorLogged = false;
        }
        catch (Exception exception)
        {
            if (!refreshErrorLogged)
            {
                Debug.LogWarning($"Unable to refresh online counters: {exception.Message}");
                refreshErrorLogged = true;
            }
        }
        finally
        {
            refreshInProgress = false;
        }
    }
}
