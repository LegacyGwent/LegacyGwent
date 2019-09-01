using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Alsein.Extensions.IO;

public class MessageBox : MonoBehaviour
{
    public Text TitleText;
    public Text MessageText;
    public Text YesText;
    public Text NoText;
    public GameObject YesButton;
    public GameObject NoButton;
    private ITubeInlet sender;
    private ITubeOutlet receiver;
    //private IAsyncDataSender sender;
    //private IAsyncDataReceiver receiver;
    private void Awake()
    {
        (sender, receiver) = Tube.CreateSimplex();
    }
    public Task<bool> Show(string title, string message, string yes = "确定", string no = "取消", bool isOnlyYes = false)
    {
        if (isOnlyYes)
        {
            YesButton.SetActive(true);
            NoButton.SetActive(false);
        }
        else
        {
            YesButton.SetActive(true);
            NoButton.SetActive(true);
        }
        gameObject.SetActive(true);
        TitleText.text = title;
        MessageText.text = message;
        YesText.text = yes;
        NoText.text = no;
        return receiver.ReceiveAsync<bool>();
    }
    public void YesClick()
    {
        sender.SendAsync<bool>(true);
        gameObject.SetActive(false);
    }
    public void NoClick()
    {
        sender.SendAsync<bool>(false);
        gameObject.SetActive(false);
    }
}
