using Alsein.Extensions.IO;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class TimeLimitHandler
{
    public async static Task<T> ReceiveWithTimeLimitAsync<T>(ITubeOutlet receiver, Func<Task> callback, float timeLimit = 5)
    {
        // NOTE: can not reach unity in callback since it is not in main thread
        bool isReceived = false;
        if (timeLimit <= 0)
        {
            await callback();
        }
        else
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(timeLimit));
                if (!isReceived)
                {
                    try
                    {
                        await callback();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.ToString());
                    }
                }
            });
        }
        var result = await receiver.ReceiveAsync<T>();
        isReceived = true;
        return result;
    }
}