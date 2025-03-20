using System.Collections;
using System.Collections.Generic;
using Cynthia.Card;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using Autofac;
using Cynthia.Card.Client;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using Assets.Script.Localization;
using UnityEngine.Events;
using static UnityEngine.UI.Scrollbar;
using Cynthia.Card.Common.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
public class TrinketsInfo : MonoBehaviour // this script controls the behaviour of the trinket screen
{   // rect transforms
    public RectTransform AvatarsContext;
    public RectTransform BordersContext;
    public RectTransform TitlesContext;
    // scrollbats
    public Scrollbar AvatarsScroll;
    public Scrollbar BordersScroll;
    public Scrollbar TitlesScroll;
    // objects
    public GameObject MainUI;
    public GameObject TrinketsUI;
    public GameObject GameProfile;
    public GameObject AvatarPrefab;
    public GameObject BorderPrefab;
    public GameObject TitlePrefab;
    // services
    private GwentClientService _clientService;
    // data
    private IList<TrinketAvatar> _avatars_released { get => TrinketMap.GetAvatars().Where(x => x.IsReleased).ToList(); }
    private IList<Border> _borders_released { get => TrinketMap.GetBorders().Where(x => x.IsReleased).ToList(); }
    private IList<Title> _titles_released { get => TrinketMap.GetTitles().Where(x => x.IsReleased).ToList(); }
    

    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
    }

    void Start()
    {
        AutoSetAvatars();
        AutoSetBorders();
        AutoSetTitles();
    }

    public void SetAvatarsInfo(IList<TrinketAvatar> Avatars)
    {   // Controls the layout of the avatars in the avatar menu
        var pagenum = 30;
        AvatarsScroll.value = 1;
        RemoveAllChild(AvatarsContext);
        var sc = 0;
        AddAvatars(sc, pagenum, Avatars);
        void AddAvatars(int skipCount, int pageCount, IList<TrinketAvatar> avatars)
        {
            if (avatars.Count <= skipCount * pageCount)
            {
                return;
            }
            
            var NewAvatars = avatars.Skip(skipCount * pageCount).Take(pageCount).ToList();
            NewAvatars.ForAll(x =>
            {
                var avatar = Instantiate(AvatarPrefab);                
                avatar.transform.SetParent(AvatarsContext, false);
                avatar.GetComponent<TrinketsShow>().SetAvatarArt(x.ID);
                
            });
        }
        if (_editorCardScrollEvent != null)
        {
            AvatarsScroll.onValueChanged.RemoveListener(_editorCardScrollEvent);
        }
        _editorCardScrollEvent = x =>
        {
            if (x >= 0.3)
            {
                return;
            }

            sc++;
            AddAvatars(sc, pagenum, Avatars);
        };
        AvatarsScroll.onValueChanged.AddListener(_editorCardScrollEvent);
    }
    public void SetBordersInfo(IList<Border> Borders)
    {   // Controls the layout of the avatars in the border menu
        var pagenum = 30;
        BordersScroll.value = 1;
        RemoveAllChild(BordersContext);
        var sc = 0;
        AddBorders(sc, pagenum, Borders);
        void AddBorders(int skipCount, int pageCount, IList<Border> borders)
        {
            if (borders.Count <= skipCount * pageCount)
            {
                return;
            }
            
            var NewBorders = borders.Skip(skipCount * pageCount).Take(pageCount).ToList();
            NewBorders.ForAll(x =>
            {
                var border = Instantiate(BorderPrefab);                
                border.transform.SetParent(BordersContext, false);
                border.GetComponent<TrinketsShow>().SetBorderArt(x.ID);
                
            });
        }
        if (_editorCardScrollEvent != null)
        {
            BordersScroll.onValueChanged.RemoveListener(_editorCardScrollEvent);
        }
        _editorCardScrollEvent = x =>
        {
            if (x >= 0.3)
            {
                return;
            }

            sc++;
            AddBorders(sc, pagenum, Borders);
        };
        BordersScroll.onValueChanged.AddListener(_editorCardScrollEvent);
    }

    public void SetTitlesInfo(IList<Title> Titles)
    {   // Controls the layout of the titles in the titles menu
        var pagenum = 30;
        TitlesScroll.value = 1;
        RemoveAllChild(TitlesContext);
        var sc = 0;
        AddTitles(sc, pagenum, Titles);
        void AddTitles(int skipCount, int pageCount, IList<Title> titles)
        {
            if (titles.Count <= skipCount * pageCount)
            {
                return;
            }
            
            var NewTitles = titles.Skip(skipCount * pageCount).Take(pageCount).ToList();
            NewTitles.ForAll(x =>
            {
                var title = Instantiate(TitlePrefab);                
                title.transform.SetParent(TitlesContext, false);
                title.GetComponent<TrinketsShow>().SetTitleList(x.ID, x.TitleColor);
                
            });
        }
        if (_editorCardScrollEvent != null)
        {
            TitlesScroll.onValueChanged.RemoveListener(_editorCardScrollEvent);
        }
        _editorCardScrollEvent = x =>
        {
            if (x >= 0.3)
            {
                return;
            }

            sc++;
            AddTitles(sc, pagenum, Titles);
        };
        BordersScroll.onValueChanged.AddListener(_editorCardScrollEvent);
    }
    private UnityAction<float> _showCardScrollEvent = null;
    private UnityAction<float> _editorCardScrollEvent = null;
    
    public void AutoSetAvatars()
    {   //autobuild avatar menu with released avatars
            SetAvatarsInfo
        (
            _avatars_released
            .ToList()
        );
    }
    public void AutoSetBorders()
    {   //autobuild border menu with released borders
            SetBordersInfo
        (
            _borders_released
            .ToList()
        );
    }
    public void AutoSetTitles()
    {   //autobuild titles menu with released titles
            SetTitlesInfo
        (
            _titles_released
            .ToList()
        );
    }
    public void RemoveAllChild(Transform father)
    {   //clean the menu of the relevant trinket
        for (var i = father.childCount - 1; i >= 0; i--)
        {
            Destroy(father.GetChild(i).gameObject);
        }
        father.DetachChildren();
    }
    public void RemovePreview()
    {   // remove the trinket preview before loading another one
        foreach (Transform child in transform.root)
        {
            if (child.name.Contains("Prefab"))
            {
                Destroy(child.gameObject);
            }
        }
    }
    public void TrinketsButtonClicked()
    {
        MainUI.SetActive(false);
        TrinketsUI.SetActive(true);
        GameProfile.SetActive(false);
    }
    public void BackButtonClicked()
    {
        RemovePreview();
        MainUI.SetActive(true);
        TrinketsUI.SetActive(false);
        GameProfile.SetActive(true);
    }

}
