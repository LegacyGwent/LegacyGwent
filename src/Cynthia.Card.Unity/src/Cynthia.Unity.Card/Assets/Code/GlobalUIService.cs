using System;
using System.Collections.Generic;
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
        private Func<MessageBox> _messageBox;
        private Func<EnhancedMessageBox> _messageBoxEnhanced;

        public GlobalUIService()
        {
            _messageBox = () => GameObject.Find("GlobalUI").transform.Find("MessageBoxBg").gameObject.GetComponent<MessageBox>();
            //_messageBoxEnhanced = () => GameObject.Find("GlobalUI").transform.Find("EnhancedMessageBoxBg").gameObject.GetComponent<EnhancedMessageBox>();
            _messageBoxEnhanced = () => GameObject.Find("GlobalUI").GetComponent<GlobalUI>().GetEnhancedMessageBox();
        }
        public Task<bool> YNMessageBox(string title, string message, string yes = "PopupWindow_YesButton", string no = "PopupWindow_NoButton", bool isOnlyYes = false)
        {
            return _messageBox().Show(title.Replace("\\n", "\n"), message.Replace("\\n", "\n"), yes.Replace("\\n", "\n"), no.Replace("\\n", "\n"), isOnlyYes);
        }

        public Task<bool> YNMessageBoxEnhanced(string title, string message, string yes = "PopupWindow_YesButton", string no = "PopupWindow_NoButton", bool isOnlyYes = true, string message2 = "", string message3 = "", IList<string> avatars = null, IList<string> borders = null, IList<string> titles = null)
        {
            return _messageBoxEnhanced().Show(title.Replace("\\n", "\n"), message.Replace("\\n", "\n"), yes.Replace("\\n", "\n"), no.Replace("\\n", "\n"), isOnlyYes, message2.Replace("\\n", "\n"), message3.Replace("\\n", "\n"), avatars, borders, titles);
        }

        public void Wait(string title, string message)
        {
            _messageBox().Wait(title.Replace("\\n", "\n"), message.Replace("\\n", "\n"));
        }

        public void Close()
        {
            _messageBox().Close();
        }
    }
}
