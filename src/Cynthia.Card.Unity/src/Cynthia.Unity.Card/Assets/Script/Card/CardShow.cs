using Cynthia.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CardShow : MonoBehaviour {
    public Group Group = Group.Copper;
    public Faction Faction = Faction.Neutral;
    public int Strong = 0;

    private RectTransform icon;
    private RectTransform strong;
    private RectTransform frame;
    //private RectTransform body;
    public RectTransform back;
    // Use this for initialization
    void Start()
    {
        foreach (RectTransform item in gameObject.GetComponentsInChildren<RectTransform>())
        {
            Debug.Log(item.name);
            if (item.name == "CardIcon")
            {
                icon = item;
            }
            if (item.name == "CardStrong")
            {
                strong = item;
            }
            if (item.name == "CardFrame")
            {
                frame = item;
            }
            if (item.name == "CardBody")
            {
                //body = item;
            }
        }
        strong.gameObject.GetComponent<Text>().text = Strong.ToString();
        frame.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/Frames/border" + (Group == Group.Copper ? 1 : (Group == Group.Silver ? 2 : 3)));
        if (Faction == Faction.Neutral)
        {
            icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FactionIcon/factionIcon1" + (Group == Group.Gold ? "l" : ""));
            back.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FlippedFaction/flippedFaction1");
        }
        if (Faction == Cynthia.Card.Faction.Monsters)
        {
            icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FactionIcon/factionIcon2" + (Group == Group.Gold ? "l" : ""));
            back.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FlippedFaction/flippedFaction2");
        }
        if (Faction == Faction.Neutral)
        {
            icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FactionIcon/factionIcon3" + (Group == Group.Gold ? "l" : ""));
            back.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FlippedFaction/flippedFaction3");
        }
        if (Faction == Cynthia.Card.Faction.ScoiaTael)
        {
            icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FactionIcon/factionIcon4" + (Group == Group.Gold ? "l" : ""));
            back.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FlippedFaction/flippedFaction4");
        }
        if (Faction == Faction.Skellige)
        {
            icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FactionIcon/factionIcon5" + (Group == Group.Gold ? "l" : ""));
            back.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FlippedFaction/flippedFaction5");
        }
        if (Faction == Faction.Nilfgaard)
        {
            icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FactionIcon/factionIcon6" + (Group == Group.Gold ? "l" : ""));
            back.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CardSprotes/FlippedFaction/flippedFaction6");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}