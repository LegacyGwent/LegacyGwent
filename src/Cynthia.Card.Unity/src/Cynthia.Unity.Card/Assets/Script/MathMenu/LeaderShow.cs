using Autofac;
using Cynthia.Card;
using System.Collections.Generic;
using Assets.Script.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class LeaderShow : MonoBehaviour
{
    public Text Streng;
    public Text Name;
    public Image CardShow;
    public Image Title;
    public Image Miniature;
    public Sprite NorthernreaIcon;
    public Sprite ScoiataelIcon;
    public Sprite MonsterIcon;
    public Sprite SkelligeIcon;
    public Sprite NilfgaardIcon;
    private IDictionary<Faction, Sprite> _groupIconMap;
    public string CurrentId { get; private set; } = null;
    public void Start()
    {
        _groupIconMap = new Dictionary<Faction, Sprite>
         {
             {Faction.NorthernRealms,NorthernreaIcon},
             {Faction.ScoiaTael,ScoiataelIcon},
             {Faction.Monsters,MonsterIcon},
             {Faction.Skellige,SkelligeIcon},
             {Faction.Nilfgaard,NilfgaardIcon},
         };
    }
    public void SetLeader(string id)
    {
        CurrentId = id;
        if (_groupIconMap == null) Start();

        var card = GwentMap.CardMap[id];
        var translator = DependencyResolver.Container.Resolve<LocalizationService>();
        card.Name = translator.GetCardName(id);
        card.Info = translator.GetCardInfo(id);

        Name.text = card.Name;
        Streng.text = card.Strength.ToString();
        Title.sprite = _groupIconMap[card.Faction];
        string miniatureid = card.CardArtsId + "_slot";
        var op = Addressables.LoadAssetAsync<Sprite>(miniatureid);
        Sprite go = op.WaitForCompletion();
        Miniature.sprite = go;
        // Addressables.LoadAssetAsync<Sprite>(miniatureid).Completed += (obj) =>
        // {
        //     Miniature.sprite = obj.Result;
        // };
    }
}
