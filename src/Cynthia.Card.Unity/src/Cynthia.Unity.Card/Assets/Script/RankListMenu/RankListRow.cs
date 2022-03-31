using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankListRow : MonoBehaviour
{
    public Text RankNumber;
    public Text Playername;
    public Text MMR;

    public void SetRankListRow(int rankNumber, string playername, int mmr)
    {
        RankNumber.text = "Rank " + rankNumber;
        Playername.text = playername;
        MMR.text = mmr.ToString();
    }
    public void SetRankListRowString(string rankNumber, string playername, int mmr)
    {
        RankNumber.text = "Rank " + rankNumber;
        Playername.text = playername;
        MMR.text = mmr.ToString();
    }
    public void SetHighlight()
    {
        Playername.color = RankNumber.color;
    }
    public void SetTransparency()
    {
        RankNumber.color = Color.clear;
        Playername.color = Color.clear;
        MMR.color = Color.clear;
    }
}
