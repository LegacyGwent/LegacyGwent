using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;

public class PlayerTableRowScript : MonoBehaviour
{
    public Text RankNumber;
    public Text Playername;
    public Image Avatar;
    public Image Border;
    public Text MMR;
    public Text PeakMMR;
    public Text Wins;
    public Text Defeats;
    public Text Draws;
    public Image Background;
    public Button playerButton;
    [SerializeField]
    private Image LoggedPlayerLine;
    [SerializeField]
    private Image LoggedPlayerLineBackground;
    private static readonly Color standardLineColor = new Color(0.620f, 0f, 0.952f, 1f);
    private static readonly Color topLineColor = new Color(1f, 0.397f, 0f, 1f);
    private static readonly Color standardBackgroundColor = new Color(0.650f, 0f, 1f, 0.901f);
    private static readonly Color topBackgroundColor = new Color(1f, 0.574f, 0f, 0.901f);

    public void SetRankListRow(int rankNumber, string playername, string avatar, string border, int mmr, int peakMMR, int[] stats, bool logged = false)
    {
        RankNumber.text = rankNumber.ToString();
        Playername.text = playername;
        MMR.text = mmr.ToString();
        PeakMMR.text = peakMMR.ToString();
        if (stats != null && stats.Length == 3)
        {
            Wins.text = stats[0].ToString();
            Defeats.text = stats[1].ToString();
            Draws.text = stats[2].ToString();
        }

        if (string.IsNullOrEmpty(avatar))
            avatar = "NoAvatar";
        SetAvatar(avatar);

        if (string.IsNullOrEmpty(border))
            border = "NoBorder";
        SetBorder(border);

        if(logged)
            SetLoggedPlayerLine(rankNumber);
    }

    //(x.PlayerName, x.CurrentAvatar, x.CurrentBorder, x.MMR, x.Streak);
    public void SetRankListRowString(string rankNumber, string playername, string avatar, string border, int mmr, int peakMMR, int[] stats, bool logged = false)
    {
        RankNumber.text = rankNumber;
        Playername.text = playername;
        MMR.text = mmr.ToString();
        PeakMMR.text = peakMMR.ToString();

        if (stats != null && stats.Length == 3)
        {
            Wins.text = stats[0].ToString();
            Defeats.text = stats[1].ToString();
            Draws.text = stats[2].ToString();
        }
        if (string.IsNullOrEmpty(avatar))
            avatar = "NoAvatar";
        SetAvatar(avatar);

        if (string.IsNullOrEmpty(border))
            border = "NoBorder";
        SetBorder(border);
        
        int rank;
        if (int.TryParse(rankNumber, out rank))
        {
            if (logged)
                SetLoggedPlayerLine(rank);
        }

    }

    public void SetAvatar(string avatar)
    {
        try
        {
            var op = Addressables.LoadAssetAsync<Sprite>(avatar);
            Sprite avatar_img = op.WaitForCompletion();
            Avatar.sprite = avatar_img;
        }
        catch (Exception e) { }
    }

    public void SetBorder(string border)
    {
        try
        {
            var op = Addressables.LoadAssetAsync<Sprite>(border);
            Sprite border_img = op.WaitForCompletion();
            Border.sprite = border_img;
        }
        catch (Exception e) { }
    }

    private void SetLoggedPlayerLine(int rank)
    {

        if (LoggedPlayerLineBackground != null && LoggedPlayerLine != null)
        {
            if (rank <= 3)
            {
                LoggedPlayerLine.color = PlayerTableRowScript.topLineColor;
                LoggedPlayerLineBackground.color = PlayerTableRowScript.topBackgroundColor;
            }
            else
            {
                LoggedPlayerLine.color = PlayerTableRowScript.standardLineColor;
                LoggedPlayerLineBackground.color = PlayerTableRowScript.standardBackgroundColor;
            }
        }
    }
}
