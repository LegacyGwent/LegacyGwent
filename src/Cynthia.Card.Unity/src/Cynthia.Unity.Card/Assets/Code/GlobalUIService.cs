using System;
using System.Threading.Tasks;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.Audio;
using Alsein.Extensions;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class GlobalUIService
    {
        private MessageBox _messageBox;
        public GlobalUIService()
        {
            _messageBox = GameObject.Find("GlobalUI").transform.Find("MessageBoxBg").gameObject.GetComponent<MessageBox>();
        }
        public Task<bool> YNMessageBox(string title,string message,string yes = "确定",string no = "取消")
        {
            return _messageBox.Show(title,message,yes,no);
        }
    }
}
