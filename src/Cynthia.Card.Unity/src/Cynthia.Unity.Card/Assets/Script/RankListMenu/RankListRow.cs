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
}
