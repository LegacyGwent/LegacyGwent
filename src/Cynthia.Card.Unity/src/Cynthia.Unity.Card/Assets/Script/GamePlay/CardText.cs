using Cynthia.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardText : MonoBehaviour
{
    public Text ShowMessage;
    private void Start()
    {

    }

    public void SetNumber(int num, NumberType type = NumberType.Normal)
    {
        if (num == 0 && type != NumberType.Countdown)
        {
            ShowMessage.text = "";
            return;
        }
        var color = default(Color);
        if (type == NumberType.White)
            color = ClientGlobalInfo.NormalColor;
        else if (type == NumberType.Countdown)
        {
            color = ClientGlobalInfo.BlueColor;
            if (num >= 0)
                transform.localPosition += new Vector3(0.15f, 0, 0);
        }
        else
            color = num > 0 ? ClientGlobalInfo.GreenColor : ClientGlobalInfo.RedColor;
        ShowMessage.color = color;
        ShowMessage.text = (type != NumberType.Countdown && num >= 0) ? "+" : "";
        ShowMessage.text += num.ToString();
    }
    public void Show()
    {
        gameObject.GetComponent<Animator>().Play("ShowNumber");
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
