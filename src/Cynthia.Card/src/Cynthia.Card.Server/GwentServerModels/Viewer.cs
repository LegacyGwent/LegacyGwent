using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Microsoft.AspNetCore.SignalR;


namespace Cynthia.Card.Server
{
    public class Viewer
    {
        public User CurrentUser { get; set; }
        public IList<object> OperationList { get; set; }
        public Func<IHubContext<GwentHub>> Hub;
        public Viewer(User user, Func<IHubContext<GwentHub>> hub)
        {
            CurrentUser = user;
            OperationList = new List<object>();
            Hub = hub;
        }

        public async Task AddOperation(TubeReceiveEventArgs op)
        {
            lock (OperationList)
            {
                OperationList.Add(op.Result);
            }
            if (OperationList.Count > 50)
            {
                await SendOperationList();
            }
            await Task.CompletedTask;
        }

        public async Task SendOperationList()
        {
            if (OperationList.Count == 0) return;
            var tempList = default(IList<object>);
            lock (OperationList)
            {
                tempList = OperationList.ToList();
                OperationList.Clear();
            }
            await Hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("ViewerGameOperation", tempList);
        }
    }
}