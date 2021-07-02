using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCardID
{
    private static List<string> cardid = new List<string>()
    {
        "20149000",
        "20005300",
        "11320700",
        "11320100",
        "11320200",
        "11320300",
        "11330700",
        "11330100",
        "11331900",
        "20174900",
        "15330100",
        "20150200",
        "11331000",
        "11331100",
        "20143800",
        "20153400",
        "20022500",
        "20007800",
        "20015400"
    };


    public static bool isSpecialCard(string id)
    {
        for (int i = 0; i < cardid.Count; i++)
        {
            if (cardid[i].Equals(id))
                return true;
        }
        return false;
    }

}
