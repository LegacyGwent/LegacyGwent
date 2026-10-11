using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Localization;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Arena mode: draft 26 cards + a leader, then play until 9 wins or 3 losses.
//The tile is a clone of the ranked tile; the panel is rebuilt from the 0.9.24 beta arena layouts (see ArenaLayout),
//so the main menu scene itself is untouched.
public class ArenaMenu : MonoBehaviour
{
    private const float StageWidth = 1600;
    private const float StageHeight = 900;
    private static readonly Color ButtonTextColor = new Color(0.96f, 0.91f, 0.80f);
    private static readonly Color ButtonLabelColor = new Color(0.90f, 0.90f, 0.88f);
    private static readonly Color CountColor = new Color(0.93f, 0.93f, 0.93f);
    private static readonly Color WinsColor = new Color(0.96f, 0.84f, 0.30f);
    private static readonly Color PaperTextColor = new Color(0.33f, 0.21f, 0.12f);

    private GwentClientService _client { get => DependencyResolver.Container.Resolve<GwentClientService>(); }
    private GlobalUIService _UIService { get => DependencyResolver.Container.Resolve<GlobalUIService>(); }
    private LocalizationService _translator { get => DependencyResolver.Container.Resolve<LocalizationService>(); }

    private MatchInfo _matchInfo;
    private GameObject _mainUI;

    private RectTransform _panel;
    private RectTransform _stage;
    private ArenaLayout _frame;
    private Text _title;
    private RectTransform _left;
    private RectTransform _main;
    private RectTransform _right;
    private RectTransform _buttons;
    private GameObject _overlay;      //full-stage content (rewards) shown instead of the three columns
    private RectTransform _previewParent;
    private ArtCard _preview;

    private ArenaRunModel _run;
    private bool _isBusy;
    private bool _isDoingMatch;

    //effects (ArenaFX): the background ones follow the background image, the others live on a layer on top of the
    //stage that survives Refresh (the incinerator and the card flashes outlast the screen they start on)
    private RectTransform _background;
    private RectTransform _backgroundFX;
    private RectTransform _fx;
    private RectTransform _choiceBanner;
    private readonly List<RectTransform> _choiceCards = new List<RectTransform>();
    private int _flashSlot = -1;
    //wins/losses last shown, for the win/life sounds after a match (kept while the client runs)
    private static int _seenWins = -1, _seenLosses = -1;
    //beta UIArenaDeckPickerSlideInOut: offset 110 picker units (~2.7 card widths), 0.45 s, 0.05 s delay
    private const float SlideOffset = 357, SlideTime = 0.45f, SlideDelay = 0.05f;

    public static ArenaMenu Install(Button rankTileButton, Button casualTileButton)
    {
        var matchInfo = Resources.FindObjectsOfTypeAll<MatchInfo>().FirstOrDefault(x => x.gameObject.scene.IsValid());
        var rootCanvas = rankTileButton == null ? null : rankTileButton.GetComponentInParent<Canvas>();
        if (rootCanvas == null || matchInfo == null)
        {
            Debug.LogError($"ArenaMenu: could not install (tile={rankTileButton != null}, canvas={rootCanvas != null}, matchInfo={matchInfo != null})");
            return null;
        }
        var canvas = rootCanvas.rootCanvas.gameObject;
        var menu = canvas.AddComponent<ArenaMenu>();
        menu._matchInfo = matchInfo;
        menu._mainUI = matchInfo.MainUI;
        menu.CreateTile(rankTileButton, casualTileButton);
        menu.CreatePanel(canvas.transform);
        return menu;
    }

    //================================================================ building

    private void CreateTile(Button rankTileButton, Button casualTileButton)
    {
        var rankTile = (RectTransform)rankTileButton.transform.parent;
        var row = (RectTransform)rankTile.parent;
        //the "playing" counters and "opponent found" ribbons sit loose in MainUI at fixed positions;
        //move them into their tiles so they stay with the tiles when the row gets the extra arena tile
        LayoutRebuilder.ForceRebuildLayoutImmediate(row);
        var mainUI = _mainUI.transform;
        AttachToTile(mainUI.Find("UsersPlayingRank"), rankTile);
        AttachToTile(mainUI.Find("RankFlagObject"), rankTile);
        if (casualTileButton != null)
        {
            AttachToTile(mainUI.Find("UsersPlayingCasual"), casualTileButton.transform.parent);
            AttachToTile(mainUI.Find("CasualFlagObject"), casualTileButton.transform.parent);
        }

        var tile = Instantiate(rankTile.gameObject, row);
        tile.name = "ArenaMenuBg";
        tile.transform.SetSiblingIndex(rankTile.GetSiblingIndex() + 1);
        //Gaunter O'Dimm, the arena tile of the beta main menu
        var art = Resources.Load<Sprite>("Sprites/MainMenu/Arena");
        if (art != null)
            tile.GetComponent<Image>().sprite = art;
        var countLabel = tile.transform.Find("UsersPlayingRank/UserCountText");
        if (countLabel != null)
            countLabel.GetComponent<Text>().text = _translator.GetText("MainMenu_PlayingRankText");
        var count = tile.transform.Find("UsersPlayingRank/UserCountNum");
        _tileCount = count == null ? null : count.GetComponent<Text>();
        var flag = tile.transform.Find("RankFlagObject");
        if (flag != null)
        {
            _tileFlag = flag.gameObject;
            var flagText = flag.Find("FlagText");
            if (flagText != null)
                flagText.GetComponent<Text>().text = _translator.GetText("MainMenu_RankFlagText");
            _tileFlag.SetActive(false);
        }
        foreach (var text in tile.GetComponentsInChildren<Text>(true))
        {
            if (text.name == "Title")
                text.text = _translator.GetText("MainMenu_ArenaTitle");
            else if (text.name == "Description")
                text.text = _translator.GetText("MainMenu_ArenaDesc");
        }
        var button = tile.GetComponentInChildren<Button>(true);
        //replace the cloned (persistent) ranked click handlers
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(Open);
        FitTiles(row);
    }

    private static void AttachToTile(Transform item, Transform tile)
    {
        if (item != null && tile != null)
            item.SetParent(tile, true);
    }

    //five tiles are wider than the row: scale them all down evenly so the deck builder tile stays on screen
    private static void FitTiles(RectTransform row)
    {
        var layout = row.GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
            return;
        var tiles = row.Cast<RectTransform>().Where(x =>
        {
            var element = x.GetComponent<LayoutElement>();
            return x.gameObject.activeSelf && (element == null || !element.ignoreLayout);
        }).ToList();
        var width = tiles.Sum(x => x.sizeDelta.x);
        var gaps = layout.spacing * (tiles.Count - 1);
        var available = row.rect.width - 2 * layout.padding.left;
        if (width + gaps <= available)
            return;
        var scale = (available - gaps) / width;
        layout.childScaleWidth = layout.childScaleHeight = true;
        foreach (var tile in tiles)
            tile.localScale = new Vector3(scale, scale, 1);
    }

    //the arena tile counter and ribbon, polled like the casual/ranked ones (UserCounter)
    private Text _tileCount;
    private GameObject _tileFlag;
    private float _tileTimer;

    private async void UpdateTileStatus()
    {
        try
        {
            var playing = await _client.GetUsersInArenaCount();
            var waiting = await _client.GetIsArenaQueue();
            if (_tileCount != null)
                _tileCount.text = playing.ToString();
            if (_tileFlag != null)
                _tileFlag.SetActive(waiting > 0);
        }
        catch (Exception e)
        {
            Debug.LogWarning("ArenaMenu: could not read the arena counters: " + e.Message);
        }
    }

    private void CreatePanel(Transform canvas)
    {
        _panel = NewObject("ArenaUI", canvas);
        Stretch(_panel);
        //the panel blocks clicks to the menu below
        _panel.gameObject.AddComponent<Image>().color = Color.black;

        var background = NewObject("Background", _panel);
        Stretch(background);
        var backgroundImage = background.gameObject.AddComponent<Image>();
        backgroundImage.sprite = ArenaLayout.LoadSprite("arena_background");
        backgroundImage.raycastTarget = false;
        background.gameObject.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        background.GetComponent<AspectRatioFitter>().aspectRatio = StageWidth / StageHeight;
        _background = background;
        _backgroundFX = ArenaFX.Container("BackgroundFX", background);
        _backgroundFX.sizeDelta = new Vector2(StageWidth, StageHeight);
        ArenaFX.Background(_backgroundFX);

        _stage = NewObject("Stage", _panel);
        _stage.sizeDelta = new Vector2(StageWidth, StageHeight);

        _frame = ArenaLayout.Build("UIArenaPrefab", _stage);
        CenterRoot(_frame.Root);
        _left = _frame["Content/LeftContainer/LeftContent"];
        _main = _frame["Content/MainContainer/MainContent"];
        _right = _frame["Content/RightContainer/RightContent"];
        //the contract papers of the right column lie over the card preview (in the prefab they are below it)
        _frame["Content/RightContainer/Fluff_Right"].SetAsLastSibling();

        var title = ArenaLayout.Build("UIArenaTitle", _frame["TitleContainer"]);
        _title = title.Text("Label");
        _title.font = ArenaLayout.RegularFont;
        _title.color = ButtonTextColor;
        _title.alignment = TextAnchor.MiddleCenter;

        _buttons = _frame["Bottom/InputButtonContainerPrefab"];
        var buttonLayout = _buttons.GetComponent<HorizontalLayoutGroup>();
        buttonLayout.childAlignment = TextAnchor.MiddleCenter;
        buttonLayout.spacing = 36;
        buttonLayout.childControlWidth = buttonLayout.childControlHeight = true;
        buttonLayout.childForceExpandWidth = buttonLayout.childForceExpandHeight = false;

        _fx = ArenaFX.Container("FX", _stage);
        _fx.SetAsLastSibling();

        _panel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_mainUI != null && _mainUI.activeInHierarchy)
        {
            _tileTimer += Time.deltaTime;
            if (_tileTimer >= 1)
            {
                _tileTimer = 0;
                UpdateTileStatus();
            }
        }
        if (_panel == null || !_panel.gameObject.activeSelf)
            return;
        //fit the 1600x900 beta stage into the screen
        var size = _panel.rect.size;
        var scale = Mathf.Min(size.x / StageWidth, size.y / StageHeight);
        _stage.localScale = new Vector3(scale, scale, 1);
        var backgroundScale = _background.rect.width / StageWidth;
        _backgroundFX.localScale = new Vector3(backgroundScale, backgroundScale, 1);
    }

    //the rect of a UI element in FX layer units (stage units)
    private Rect FxRect(RectTransform target)
    {
        var bounds = BoundsIn(_fx, new[] { target });
        return new Rect(bounds.min, bounds.size);
    }

    private void FlashCard(RectTransform card)
    {
        if (card != null)
            ArenaFX.CardFlash(_fx, FxRect(card));
    }

    private static void CenterRoot(RectTransform root)
    {
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(0.5f, 0.5f);
        root.anchoredPosition = Vector2.zero;
    }

    private static RectTransform NewObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private static Text NewText(string name, Transform parent, int size, TextAnchor anchor, Color color, Font font = null)
    {
        var text = NewObject(name, parent).gameObject.AddComponent<Text>();
        text.font = font ?? ArenaLayout.RegularFont;
        text.fontSize = size;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = size;
        text.resizeTextMinSize = Mathf.Max(8, size / 2);
        text.alignment = anchor;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    //the row of actions below the panel, as the regular Gwent PC buttons (the main menu EXIT button)
    private void AddButton(string labelKey, Action onClick)
    {
        var buttonObject = Instantiate(Resources.Load<GameObject>("Prefab/GwentButton"), _buttons);
        buttonObject.name = labelKey;
        var label = buttonObject.GetComponentInChildren<Text>();
        label.font = ArenaLayout.RegularFont;
        label.fontSize = 25;
        label.color = ButtonLabelColor;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        //PF Din sits a little high in its line: centre the capitals between the metal edges (measured in v32)
        label.rectTransform.offsetMin = new Vector2(0, -0.5f);
        label.rectTransform.offsetMax = new Vector2(0, -0.5f);
        label.text = _translator.GetText(labelKey).ToUpper();
        var element = buttonObject.AddComponent<LayoutElement>();
        element.preferredWidth = Mathf.Max(100, label.preferredWidth + 56);
        element.preferredHeight = 46;
        var button = buttonObject.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => onClick());
    }

    //the deck counters: white PF Din with a black outline (the beta GwentNumbers material: white face, black outline + underlay)
    private static Text SetCount(Text text, int value, int size, Color color, Font font = null, Vector2 offset = default)
    {
        text.rectTransform.anchoredPosition += offset;
        text.font = font ?? ArenaLayout.MediumFont;
        text.fontSize = size;
        text.resizeTextForBestFit = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.text = value.ToString();
        text.rectTransform.localScale = Vector3.one;
        var outline = text.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.9f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);
        return text;
    }

    //the Gwent card digits (white with a black outline, like the beta GwentNumbers material), scaled to the given height
    private static Text SetNumber(Text text, int value, float height, Color? tint = null)
    {
        text.font = Resources.Load<Font>("FountInfo/GwentWFont");
        text.fontSize = 0;
        text.resizeTextForBestFit = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = tint ?? Color.white;
        text.text = value.ToString();
        //the glyphs of GwentWFont are 97 units high
        var scale = height / 97f;
        text.rectTransform.localScale = new Vector3(scale, scale, 1);
        return text;
    }

    private static void Clear(Transform parent)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
        parent.DetachChildren();
    }

    //---------------------------------------------------------------- cards

    private static CardStatus StatusOf(string cardId, LocalizationService translator) => new CardStatus(cardId)
    {
        Name = translator.GetCardName(cardId),
        Info = translator.GetCardInfo(cardId)
    };

    //a card scaled to fit the given size; the holder takes the size of the scaled card so the hover frame fits it.
    //Draft cards are the compact deck builder cards (art + strength banner), the side preview is the large art card.
    private RectTransform CreateCard(string cardId, Transform parent, Vector2 size, bool large = false, bool highlight = false)
    {
        var holder = NewObject("Card_" + cardId, parent);
        GameObject card;
        if (large)
        {
            card = Instantiate(_matchInfo.ShowArtCard.gameObject, holder);
            card.SetActive(true);
            card.GetComponent<ArtCard>().CurrentCore = StatusOf(cardId, _translator);
        }
        else
        {
            card = Instantiate(Resources.Load<GameObject>("Prefab/Cards/Editor/EditorUICoreCard"), holder);
            //no deck builder hover/click behaviour
            DestroyImmediate(card.GetComponent<EditorUICoreCard>());
            //load the art synchronously: the screen is rebuilt after every pick and asynchronous art leaves the
            //rebuilt cards empty for a frame (the drafted cards blinked); the art is cached after the first time
            card.GetComponent<CardShowInfo>().setCurrentCore(StatusOf(cardId, _translator), false);
        }
        var cardRect = (RectTransform)card.transform;
        cardRect.anchorMin = cardRect.anchorMax = cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition3D = Vector3.zero;
        var cardSize = cardRect.rect.size;
        var scale = cardSize.x > 0 && cardSize.y > 0 ? Mathf.Min(size.x / cardSize.x, size.y / cardSize.y) : 1;
        cardRect.localScale = new Vector3(scale, scale, 1);
        holder.sizeDelta = cardSize.x > 0 && cardSize.y > 0 ? cardSize * scale : size;
        foreach (var graphic in card.GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;
        GameObject frame = null;
        if (highlight)
        {
            //the beta navigation frame: its glow lies outside the line, so the frame is larger than the card
            var frameRect = NewObject("Highlight", holder);
            Stretch(frameRect);
            frameRect.offsetMin = new Vector2(-13, -5);
            frameRect.offsetMax = new Vector2(13, 5);
            var frameImage = frameRect.gameObject.AddComponent<Image>();
            frameImage.sprite = ArenaLayout.LoadSprite("navigation_hovered_frame");
            frameImage.type = Image.Type.Sliced;
            frameImage.pixelsPerUnitMultiplier = 2.5f;
            frameImage.fillCenter = false;
            frameImage.raycastTarget = false;
            frame = frameRect.gameObject;
            frame.SetActive(false);
        }
        //a transparent overlay catches hover and clicks, whatever the art card prefab contains
        var hit = NewObject("Hit", holder);
        Stretch(hit);
        hit.gameObject.AddComponent<Image>().color = Color.clear;
        var hover = hit.gameObject.AddComponent<ArenaHover>();
        var scaleId = "ArenaCardScale" + holder.GetInstanceID();
        hover.Enter = () =>
        {
            ShowPreview(cardId);
            if (frame == null) return;
            frame.SetActive(true);
            //the cards to choose from grow a little under the mouse (only the scale tween is replaced: the slide
            //tween of the same card must keep running)
            DOTween.Kill(scaleId);
            holder.DOScale(1.07f, 0.1f).SetId(scaleId);
            ArenaFX.Play("SelectionChanged");
        };
        hover.Exit = () =>
        {
            if (frame == null) return;
            frame.SetActive(false);
            DOTween.Kill(scaleId);
            holder.DOScale(1, 0.1f).SetId(scaleId);
        };
        return holder;
    }

    //the side preview (art card) on the draft and encounter screens: same size on both. It is placed by measuring
    //what is drawn: the card + info box are centred horizontally on the beta card frame of the right panel (CardBG),
    //the top of the card PreviewTopMargin below the top of that frame (as in the beta). The strength banner sticks
    //out on the left and the art inside the card mask is larger than the card, so neither is measured.
    private static readonly Vector2 PreviewSize = new Vector2(190, 272);
    private const float PreviewTopMargin = 9;
    private RectTransform _previewFrame;

    private void PlacePreview(RectTransform card)
    {
        if (_previewFrame == null)
            return;
        Canvas.ForceUpdateCanvases();
        var images = card.GetComponentsInChildren<Image>()
            .Where(x => x.enabled && x.gameObject.activeInHierarchy && x.color.a > 0.01f && x.sprite != null)
            .Where(x => !IsMaskedOrBanner(x.transform, card))
            .ToList();
        var cardBounds = BoundsIn(_previewParent, images.Select(x => x.rectTransform));
        var frameBounds = BoundsIn(_previewParent, new[] { _previewFrame });
        if (cardBounds.size == Vector3.zero || frameBounds.size == Vector3.zero)
            return;
        var shift = new Vector2(frameBounds.center.x - cardBounds.center.x, frameBounds.max.y - PreviewTopMargin - cardBounds.max.y);
        card.anchoredPosition += shift;
    }

    private static bool IsMaskedOrBanner(Transform item, Transform card)
    {
        for (var t = item; t != null && t != card; t = t.parent)
        {
            if (t.name == "FactionIcon")
                return true;
            if (t != item && (t.GetComponent<Mask>() != null || t.GetComponent<RectMask2D>() != null))
                return true;
        }
        return false;
    }

    //bounding box of the given rects in the local space of space
    private static Bounds BoundsIn(Transform space, IEnumerable<RectTransform> rects)
    {
        var corners = new Vector3[4];
        Bounds? bounds = null;
        foreach (var rect in rects)
        {
            rect.GetWorldCorners(corners);
            foreach (var corner in corners)
            {
                var point = space.InverseTransformPoint(corner);
                point.z = 0;
                if (bounds == null)
                    bounds = new Bounds(point, Vector3.zero);
                else
                {
                    var b = bounds.Value;
                    b.Encapsulate(point);
                    bounds = b;
                }
            }
        }
        return bounds ?? new Bounds();
    }

    private void ShowPreview(string cardId)
    {
        if (_previewParent == null || string.IsNullOrEmpty(cardId))
            return;
        if (_preview == null)
        {
            var card = CreateCard(cardId, _previewParent, PreviewSize, large: true);
            card.anchorMin = card.anchorMax = new Vector2(0.5f, 0.5f);
            card.anchoredPosition = Vector2.zero;
            Destroy(card.Find("Hit").gameObject);
            _preview = card.GetComponentInChildren<ArtCard>();
            PlacePreview(card);
        }
        else
            _preview.CurrentCore = StatusOf(cardId, _translator);
    }

    //================================================================ flow

    public async void Open()
    {
        _mainUI.SetActive(false);
        _panel.gameObject.SetActive(true);
        ArenaFX.Play("ArenaIntroSound");
        ArenaFX.Play("ArenaIntroSoundPaper");
        _isDoingMatch = false;
        _isBusy = true;
        try
        {
            _run = await _client.GetArenaRun();
        }
        finally
        {
            _isBusy = false;
        }
        Refresh();
    }

    public void Close()
    {
        if (_isDoingMatch || _isBusy)
            return;
        ArenaFX.Play("MatchmakingCanceled");
        _panel.gameObject.SetActive(false);
        _mainUI.SetActive(true);
        ClientGlobalInfo.IsToMatch = false;
    }

    private void Refresh()
    {
        Clear(_left);
        Clear(_main);
        Clear(_right);
        Clear(_buttons);
        if (_overlay != null)
            Destroy(_overlay);
        _frame["Content"].gameObject.SetActive(true);
        SetFrameColumns(true);
        _previewParent = null;
        _previewFrame = null;
        _preview = null;
        _choiceBanner = null;
        _choiceCards.Clear();
        PlayResultSound();

        if (_run == null)
            ShowIntro();
        else if (_run.State == ArenaRunState.Drafting)
            ShowDraft();
        else if (_run.State == ArenaRunState.Ready)
            ShowEncounters();
        else
            ShowRewards();
        _flashSlot = -1;
    }

    //back from a match: the beta sounds for a new win (the 9th has its own) or a lost life
    private void PlayResultSound()
    {
        if (_run == null)
        {
            _seenWins = _seenLosses = -1;
            return;
        }
        if (_seenWins >= 0 && _run.Wins > _seenWins)
            ArenaFX.Play(_run.Wins >= ArenaDraft.WinsToFinish ? "LastWinAmountChanged" : "WinsAmountChanged");
        else if (_seenLosses >= 0 && _run.Losses > _seenLosses)
            ArenaFX.Play("LifeLost" + Mathf.Clamp(_run.Losses, 1, 3));
        _seenWins = _run.Wins;
        _seenLosses = _run.Losses;
    }

    //---------------------------------------------------------------- intro

    //the beta intro shows only the paper with the welcome banner: no side panels, no wood frame and no title banner
    private void SetFrameColumns(bool visible)
    {
        _frame["Content/LeftContainer"].gameObject.SetActive(visible);
        _frame["Content/RightContainer"].gameObject.SetActive(visible);
        _frame["Content/Fluff_Center"].gameObject.SetActive(visible);
        _frame["Content/MainContainer/BG"].gameObject.SetActive(visible);
        _frame["TitleContainer"].gameObject.SetActive(visible);
    }

    private void ShowIntro()
    {
        SetFrameColumns(false);
        var intro = ArenaLayout.Build("UIArenaIntroContent", _main);
        intro["TicketsAndGoldParent"].gameObject.SetActive(false);
        intro["ChooseModeRoot"].gameObject.SetActive(false);
        //the green glow of the welcome banner (an animated material in the beta)
        intro.Image("Content/Title/TitleBackground/RewardsBannerBack").color = new Color(0.55f, 0.95f, 0.30f, 1);
        var welcome = intro.Text("Content/Title/TitleForeground/Welcome");
        welcome.font = ArenaLayout.BoldFont;
        welcome.color = Color.white;
        welcome.alignment = TextAnchor.MiddleCenter;
        welcome.text = _translator.GetText("Arena_IntroTitle").ToUpper();
        welcome.gameObject.AddComponent<Shadow>().effectColor = new Color(0.1f, 0.25f, 0.05f, 0.9f);
        //the banner smoke only shows on the green field of the banner: above the green, under the frame and the text
        var bannerBack = intro["Content/Title/TitleBackground/RewardsBannerBack"];
        bannerBack.gameObject.AddComponent<Mask>();
        ArenaFX.IntroBanner(bannerBack);
        var rulesTitle = intro.Text("Content/BuyTicket/RulesTitle");
        rulesTitle.font = ArenaLayout.BoldFont;
        rulesTitle.color = PaperTextColor;
        rulesTitle.alignment = TextAnchor.MiddleCenter;
        rulesTitle.text = _translator.GetText("Arena_RulesTitle").ToUpper();
        var rulesText = intro.Text("Content/BuyTicket/RulesText");
        rulesText.color = PaperTextColor;
        rulesText.alignment = TextAnchor.UpperCenter;
        rulesText.text = _translator.GetText("Arena_RulesText");
        //keep the top of the text below the divider of the title and let the extra lines grow downwards
        var rulesRect = intro["Content/BuyTicket/RulesText"];
        rulesRect.pivot = new Vector2(0.5f, 1);
        rulesRect.sizeDelta += new Vector2(0, 60);
        rulesRect.anchoredPosition = new Vector2(rulesRect.anchoredPosition.x, -68);
        //no entry fee: the requirements block is not used
        intro["Content/BuyTicket/RequirementsTitle"].gameObject.SetActive(false);
        intro["Content/BuyTicket/RequirementsContainer"].gameObject.SetActive(false);

        AddButton("Arena_SignButton", StartClick);
        AddButton("Arena_Exit", Close);
    }

    //---------------------------------------------------------------- draft

    private void ShowDraft()
    {
        var isLeader = ArenaDraft.IsLeaderPick(_run.PickNumber);
        _title.text = _translator.GetText("Arena_BuildDeckTitle");

        ShowDeckStats(showFactions: true);

        var build = ArenaLayout.Build("UIArenaBuildDeckMainContent", _main);
        //the beta rendered the cards through off-screen canvases; we place them directly instead
        build["InnerVerticalLayout"].gameObject.SetActive(false);
        build["OuterVerticalLayout"].gameObject.SetActive(false);
        build["CardSelectionBG/RendererMask/DeckCompletedText"].gameObject.SetActive(false);
        build["CardSelectionBG/RendererMask/BG"].gameObject.SetActive(!isLeader);
        build["CardSelectionBG/RendererMask/BG_Leader"].gameObject.SetActive(isLeader);
        var title = build.Text("TitleContainer/Title");
        title.font = ArenaLayout.RegularFont;
        title.color = ButtonTextColor;
        title.alignment = TextAnchor.MiddleCenter;
        title.text = _translator.GetText(isLeader ? "Arena_ChooseLeader" : "Arena_PickCard").ToUpper();

        //the cards to choose from, inside the banner
        var choices = NewObject("Choices", build["CardSelectionBG/RendererMask"]);
        Stretch(choices);
        var choiceLayout = choices.gameObject.AddComponent<HorizontalLayoutGroup>();
        choiceLayout.spacing = 14;
        choiceLayout.childAlignment = TextAnchor.MiddleCenter;
        choiceLayout.childControlWidth = choiceLayout.childControlHeight = false;
        choiceLayout.childForceExpandWidth = choiceLayout.childForceExpandHeight = false;
        foreach (var cardId in _run.Choices)
        {
            var card = CreateCard(cardId, choices, new Vector2(116, 145), highlight: true);
            var id = cardId;
            card.Find("Hit").gameObject.AddComponent<Button>().onClick.AddListener(() => PickClick(id, card));
            _choiceCards.Add(card);
        }
        _choiceBanner = build["CardSelectionBG/RendererMask"];
        //the new cards slide in from the left (the banner mask clips them), then the layout is fixed for the slide out
        LayoutRebuilder.ForceRebuildLayoutImmediate(choices);
        choiceLayout.enabled = false;
        foreach (var card in _choiceCards)
        {
            var x = card.anchoredPosition.x;
            card.anchoredPosition = new Vector2(x - SlideOffset, card.anchoredPosition.y);
            card.DOAnchorPosX(x, SlideTime).SetDelay(SlideDelay).SetEase(Ease.OutQuad);
        }
        ArenaFX.Play("SlideCardsIn");

        //overview of the drafted deck: 26 card slots + the leader slot, 9 per row (the beta "card list")
        var grid = NewObject("DeckGrid", build.Root);
        grid.anchorMin = grid.anchorMax = new Vector2(0.5f, 0.5f);
        grid.sizeDelta = new Vector2(780, 355);
        grid.anchoredPosition = new Vector2(0, -128.9f);
        var gridLayout = grid.gameObject.AddComponent<GridLayoutGroup>();
        //9 slots + 8 gaps fit inside the paper frame (the beta: 80 wide slots, ~6 apart)
        gridLayout.cellSize = new Vector2(81, 98.5f);
        gridLayout.spacing = new Vector2(6, 18.9f);
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 9;
        gridLayout.padding = new RectOffset(0, 0, 13, 0);
        for (var i = 0; i < ArenaDraft.TotalPicks; i++)
        {
            var isLeaderSlot = i == ArenaDraft.CardPicks;
            var slot = NewObject("Slot", grid);
            var slotImage = slot.gameObject.AddComponent<Image>();
            slotImage.sprite = ArenaLayout.LoadSprite(isLeaderSlot ? "arena_card_slot_leader" : "arena_card_slot");
            slotImage.color = new Color(1, 1, 1, 0.94f);
            slotImage.raycastTarget = false;
            var cardId = isLeaderSlot ? _run.Leader : (i < _run.Deck.Count ? _run.Deck[i] : null);
            if (!string.IsNullOrEmpty(cardId))
            {
                var card = CreateCard(cardId, slot, new Vector2(78, 97.5f));
                card.anchorMin = card.anchorMax = new Vector2(0.5f, 0.5f);
                card.anchoredPosition = Vector2.zero;
            }
        }
        //the card just picked lands in its slot with the blue flash
        if (_flashSlot >= 0 && _flashSlot < grid.childCount && grid.GetChild(_flashSlot).childCount > 0)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(grid);
            FlashCard((RectTransform)grid.GetChild(_flashSlot).GetChild(0));
        }

        var preview = ArenaLayout.Build("UIArenaCardSidePreviewContent", _right);
        _previewParent = preview["SidePreviewParent"];
        _previewFrame = preview["CardBG"];
        ShowPreview(_run.Choices.FirstOrDefault());

        AddButton("Arena_Abandon", AbandonClick);
        AddButton("Arena_Exit", Close);
    }

    //counters (gold/silver/bronze) at the top of the left column, then either the faction overview + hint or the deck list
    private void ShowDeckStats(bool showFactions)
    {
        var hint = ArenaLayout.Build("UIArenaBuildDeckHintContent", _left);
        var cards = _run.Deck.Select(x => GwentMap.CardMap[x]).ToList();
        SetCount(hint, "Content/CardCount/GoldCount/Text", cards.Count(x => x.Group == Group.Gold));
        SetCount(hint, "Content/CardCount/SilverCount/Text", cards.Count(x => x.Group == Group.Silver));
        SetCount(hint, "Content/CardCount/BronzeCount/Text", cards.Count(x => x.Group == Group.Copper));

        hint["Content/FactionGridContainer"].gameObject.SetActive(showFactions);
        hint["Content/Info"].gameObject.SetActive(showFactions);
        if (showFactions)
        {
            var factions = new[]
            {
                ("FactionInfoItemNeutral", Faction.Neutral), ("FactionInfoItemNorthRealms", Faction.NorthernRealms),
                ("FactionInfoItemNilfgard", Faction.Nilfgaard), ("FactionInfoItemMonster", Faction.Monsters),
                ("FactionInfoItemScoiatel", Faction.ScoiaTael), ("FactionInfoItemSkellige", Faction.Skellige)
            };
            foreach (var (node, faction) in factions)
            {
                var number = SetCount(hint.Text($"Content/FactionGridContainer/{node}/Banner/Number"), cards.Count(x => x.Faction == faction), 27, CountColor);
                number.rectTransform.anchoredPosition += new Vector2(5, 0);
            }
            var info = hint.Text("Content/Info/Text");
            info.text = _translator.GetText("Arena_HintText");
            info.alignment = TextAnchor.MiddleCenter;
        }
        else
            ShowDeckList(hint.Root);
    }

    private static void SetCount(ArenaLayout layout, string path, int value) => SetCount(layout.Text(path), value, 26, CountColor, offset: new Vector2(0, -2));

    //the drafted deck as rows (same rows as the match menu), below the counters
    private void ShowDeckList(RectTransform parent)
    {
        var list = NewObject("DeckList", parent);
        list.anchorMin = Vector2.zero;
        list.anchorMax = Vector2.one;
        list.offsetMin = new Vector2(12, 12);
        list.offsetMax = new Vector2(-12, -158);
        var scroll = list.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.scrollSensitivity = 30;
        list.gameObject.AddComponent<Image>().color = Color.clear;
        var viewport = NewObject("Viewport", list);
        Stretch(viewport);
        viewport.gameObject.AddComponent<RectMask2D>();
        var content = NewObject("Content", viewport);
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.sizeDelta = Vector2.zero;
        var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 1;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childScaleWidth = layout.childScaleHeight = true;
        layout.childControlWidth = layout.childControlHeight = false;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.viewport = viewport;
        scroll.content = content;

        if (!string.IsNullOrEmpty(_run.Leader))
        {
            var leader = Instantiate(_matchInfo.LaderPrefab);
            leader.GetComponent<LeaderShow>().SetLeader(_run.Leader);
            leader.transform.SetParent(content, false);
            PreviewOnHover(leader, _run.Leader);
        }
        _run.Deck.Select(x => GwentMap.CardMap[x])
            .OrderByDescending(x => x.Group).ThenByDescending(x => x.Strength)
            .GroupBy(x => x.CardId)
            .ToList()
            .ForEach(x =>
            {
                var row = Instantiate(_matchInfo.CardPrefab);
                row.GetComponent<ListCardShowInfo>().SetCardInfo(x.Key, x.Count());
                row.transform.SetParent(content, false);
                FitWidth((RectTransform)row.transform, 300);
                PreviewOnHover(row, x.Key);
            });
        foreach (RectTransform child in content)
            FitWidth(child, 300);
    }

    //hovering a row shows that card in the side preview (it starts with the leader)
    private void PreviewOnHover(GameObject row, string cardId)
    {
        row.AddComponent<ArenaHover>().Enter = () => ShowPreview(cardId);
    }

    private static void FitWidth(RectTransform rect, float width)
    {
        if (rect.rect.width > width)
            rect.localScale = Vector3.one * (width / rect.rect.width);
    }

    //---------------------------------------------------------------- encounters (9 scrolls)

    private void ShowEncounters()
    {
        _title.text = _translator.GetText("MainMenu_ArenaTitle");
        ShowDeckStats(showFactions: false);
        ShowEncounterGrid();
        ShowEncounterInfo();
        AddButton("MatchmakingMenu_PlayButton", MatchClick);
        AddButton("Arena_Abandon", AbandonClick);
        AddButton("Arena_Exit", Close);
    }

    private void ShowEncounterGrid()
    {
        var encounters = ArenaLayout.Build("UIArenaSelectEncounterMainContent", _main);
        var template = encounters["_Templates/UIArenaEncounterItem"];
        var grid = encounters["Layout"];
        var gridLayout = grid.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = grid.gameObject.AddComponent<GridLayoutGroup>();
        var size = grid.rect.size;
        gridLayout.cellSize = new Vector2(size.x / 3, size.y / 3);
        gridLayout.spacing = Vector2.zero;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        //matches already played = wins + losses; the next one is the active scroll
        var played = _run.Wins + _run.Losses;
        for (var i = 0; i < ArenaDraft.WinsToFinish; i++)
        {
            var cell = NewObject("Encounter" + (i + 1), grid);
            var item = Instantiate(template.gameObject, cell);
            item.SetActive(true);
            var itemRect = (RectTransform)item.transform;
            itemRect.anchorMin = itemRect.anchorMax = itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.anchoredPosition = Vector2.zero;
            var canvas = item.transform.Find("ButtonControl/View/ScrollImageCanvas");
            var sprite = ArenaLayout.LoadSprite($"arena_scroll_sealed_{i + 1}");
            canvas.Find("ScrollImageBackground").GetComponent<Image>().sprite = sprite;
            canvas.Find("ScrollImageBackgroundShadow").GetComponent<Image>().sprite = sprite;
            var number = SetNumber(canvas.Find("ScrollNumber").GetComponent<Text>(), i + 1, 46, new Color(0.97f, 0.83f, 0.04f));
            var isPlayed = i < played;
            var isActive = i == played;
            canvas.Find("ScrollImageLock").gameObject.SetActive(!isPlayed && !isActive);
            number.gameObject.SetActive(isActive);
            var opponent = canvas.Find("OpponentName");
            opponent.gameObject.SetActive(isPlayed);
            if (isPlayed)
            {
                var name = opponent.Find("Name").GetComponent<Text>();
                name.alignment = TextAnchor.MiddleCenter;
                name.text = i < _run.Opponents.Count ? _run.Opponents[i] : "";
            }
        }
    }

    private void ShowEncounterInfo()
    {
        var info = ArenaLayout.Build("UIArenaEncounterInfoContent", _right);
        _previewParent = info["Content/SidePreviewParent"];
        _previewFrame = info["Content/CardBG"];
        ShowPreview(_run.Leader);

        //wins shield + lives a bit smaller than the beta prefab so they stay inside the right panel
        info["Content/Stats"].localScale = new Vector3(0.85f, 0.85f, 1);
        SetCount(info.Text("Content/Stats/Wins/Shield/WinsNumber"), _run.Wins, 40, WinsColor, ArenaLayout.BoldFont);

        var lifeTemplate = info["_Templates/LifeWidget"];
        var livesLayout = info["Content/Stats/Lives/LivesLayout"];
        var layout = livesLayout.GetComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = false;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        for (var i = 0; i < ArenaDraft.LossesToFinish; i++)
        {
            var life = Instantiate(lifeTemplate.gameObject, livesLayout);
            life.SetActive(true);
            var lost = i >= ArenaDraft.LossesToFinish - _run.Losses;
            life.transform.Find("Available").gameObject.SetActive(!lost);
            life.transform.Find("Lost").gameObject.SetActive(lost);
        }
    }

    //---------------------------------------------------------------- matchmaking

    private Text _searchingText;

    private void ShowSearching()
    {
        Clear(_main);
        Clear(_buttons);
        var matchmaking = ArenaLayout.Build("UIArenaMatchmakingContent", _main);
        //the glowing green mirror of the beta (animated smoke there)
        matchmaking.Image("Mirror/MirrorBackground").color = new Color(0.33f, 0.62f, 0.33f, 1);
        var spinner = matchmaking["Mirror/Container/SpinnerOrAvatarTemplate/Spinner"];
        var diamond = NewObject("Diamond", spinner);
        diamond.sizeDelta = new Vector2(190, 190);
        diamond.anchoredPosition = new Vector2(0, -2);
        diamond.gameObject.AddComponent<Image>().sprite = ArenaLayout.LoadSprite("loader_bg");
        var progressRect = NewObject("ProgressBar", diamond);
        Stretch(progressRect);
        var progress = progressRect.gameObject.AddComponent<Image>();
        progress.sprite = ArenaLayout.LoadSprite("progress_fill_mask");
        progress.type = Image.Type.Filled;
        progress.fillMethod = Image.FillMethod.Radial360;
        progress.fillOrigin = (int)Image.Origin360.Top;
        progress.fillClockwise = true;
        progress.raycastTarget = false;
        var hourglass = NewObject("Hourglass", diamond);
        hourglass.sizeDelta = new Vector2(100, 100);
        hourglass.gameObject.AddComponent<Image>().sprite = ArenaLayout.LoadSprite("timer_icon");
        var spinnerAnimation = diamond.gameObject.AddComponent<ArenaFX.Spinner>();
        spinnerAnimation.Progress = progress;
        spinnerAnimation.Hourglass = hourglass;
        //the smoke stays inside the glass of the mirror
        var mirrorGlass = matchmaking["Mirror/MirrorBackground"];
        mirrorGlass.gameObject.AddComponent<Mask>();
        ArenaFX.Mirror(mirrorGlass);
        _searchingText = NewText("Searching", matchmaking["Mirror/Container/SpinnerOrAvatarTemplate"], 30, TextAnchor.MiddleCenter, Color.white, ArenaLayout.BoldFont);
        _searchingText.rectTransform.sizeDelta = new Vector2(270, 80);
        _searchingText.rectTransform.anchoredPosition = new Vector2(0, -128);
        _searchingText.text = _translator.GetText("Arena_Searching").ToUpper();
        _searchingText.gameObject.AddComponent<Shadow>().effectColor = new Color(0, 0.15f, 0, 0.9f);
        AddButton("Arena_StopSearching", MatchClick);
    }

    //---------------------------------------------------------------- rewards

    private void ShowRewards()
    {
        _title.text = _translator.GetText("Arena_RewardsTitle");
        _frame["Content"].gameObject.SetActive(false);
        var rewards = ArenaLayout.Build("UIArenaRewardsPrefab", _stage);
        CenterRoot(rewards.Root);
        _overlay = rewards.Root.gameObject;
        rewards["TicketWidget"].gameObject.SetActive(false);
        rewards["Bottom"].gameObject.SetActive(false);
        rewards["Middle/ChooseModeRoot"].gameObject.SetActive(false);
        var title = rewards.Text("Middle/TitleGroup/TitleForeground/Title");
        title.font = ArenaLayout.BoldFont;
        title.color = new Color(0.27f, 0.18f, 0.09f);
        title.text = $"{_translator.GetText("Arena_Wins")}: {_run.Wins} / {ArenaDraft.WinsToFinish}";
        var flavour = rewards.Text("Middle/TitleGroup/TitleForeground/Flavour");
        flavour.color = new Color(0.27f, 0.18f, 0.09f);
        flavour.text = _translator.GetText(RewardTextKey(_run.Wins));
        //keep the frame buttons on top of the rewards panel
        _frame["Bottom"].SetAsLastSibling();
        _frame.Root.SetAsLastSibling();
        _fx.SetAsLastSibling();

        AddButton("Arena_PlayAgain", StartClick);
        AddButton("Arena_Exit", Close);
    }

    private static string RewardTextKey(int wins) => wins >= 7 ? "Arena_RewardsHigh" : (wins >= 3 ? "Arena_RewardsMedium" : "Arena_RewardsLow");

    //================================================================ actions

    private async void StartClick()
    {
        if (_isBusy) return;
        ArenaFX.Play(_run == null ? "PaperBurningStarted" : "PlayAgain");
        _isBusy = true;
        try
        {
            var run = await _client.StartArena();
            if (run == null)
                await ShowError();
            else
                _run = run;
        }
        finally
        {
            _isBusy = false;
        }
        Refresh();
    }

    //the beta pick: the card flashes blue and disappears, the incinerator flares up at the right end of the banner and
    //the other cards slide into it; after the server answer the new cards slide in and the pick flashes in its slot
    private async void PickClick(string cardId, RectTransform picked)
    {
        if (_isBusy) return;
        _isBusy = true;
        var isLeader = ArenaDraft.IsLeaderPick(_run.PickNumber);
        ArenaFX.Play(isLeader ? "LeaderCardClicked" : "CardClicked");
        FlashCard(picked);
        picked.gameObject.SetActive(false);
        if (_choiceBanner != null)
        {
            var banner = FxRect(_choiceBanner);
            //clipped at the pillars only: above and below the banner the glow fades out by itself
            ArenaFX.Incinerator(_fx, new Rect(banner.xMin + 30, banner.yMin - 150, banner.width - 60, banner.height + 300));
            ArenaFX.Play("IncineratorVfx");
        }
        ArenaFX.Play("SlideCardsOut");
        foreach (var card in _choiceCards)
            if (card != null && card != picked)
                card.DOAnchorPosX(card.anchoredPosition.x + SlideOffset, SlideTime).SetDelay(SlideDelay).SetEase(Ease.InQuad);
        var slideOut = System.Threading.Tasks.Task.Delay((int)((SlideTime + SlideDelay) * 1000));
        try
        {
            var run = await _client.ArenaPick(cardId);
            if (run == null)
            {
                await ShowError();
                run = await _client.GetArenaRun();
            }
            else
                _flashSlot = isLeader ? ArenaDraft.CardPicks : run.Deck.Count - 1;
            _run = run;
            await slideOut;
        }
        finally
        {
            _isBusy = false;
        }
        if (_run != null && _run.State == ArenaRunState.Ready)
            ArenaFX.Play("DeckCompleted");
        Refresh();
    }

    private async void AbandonClick()
    {
        if (_isBusy || _isDoingMatch) return;
        ArenaFX.Play("AbandonRequested");
        if (!await _UIService.YNMessageBox(_translator.GetText("Arena_AbandonConfirmTitle"), _translator.GetText("Arena_AbandonConfirmText")))
            return;
        ArenaFX.Play("AbandonConfirmed");
        _isBusy = true;
        try
        {
            if (await _client.AbandonArena())
                _run = null;
            else
                await ShowError();
        }
        finally
        {
            _isBusy = false;
        }
        Refresh();
    }

    private async void MatchClick()
    {
        if (_isBusy) return;
        try
        {
            if (_isDoingMatch)
            {
                ArenaFX.Play("MatchmakingCanceled");
                await _client.StopMatch();
                return;
            }
            ArenaFX.Play("MatchClicked");
            if (!await _client.ArenaMatch())
            {
                await ShowError();
                return;
            }
            _isDoingMatch = true;
            _client.ClientState = ClientState.Match;
            ShowSearching();
            if (await _client.MatchResult())
            {
                if (_searchingText != null)
                    _searchingText.text = _translator.GetText("Arena_MatchFound").ToUpper();
                Clear(_buttons);
                await System.Threading.Tasks.Task.Delay(1500);
                ClientGlobalInfo.IsToMatch = false;
                ClientGlobalInfo.IsPreviousRankMatch = false;
                ClientGlobalInfo.IsPreviousArenaMatch = true;
#if UNITY_STANDALONE_WIN
                ClientGlobalInfo.OpenWindow("UnityWndClass", "MyGwent");
#endif
                ClientGlobalInfo.IsReconnecting = false;
                SceneManager.LoadScene("GamePlay");
                _client.ClientState = ClientState.Play;
                return;
            }
            _isDoingMatch = false;
            _client.ClientState = ClientState.Standby;
            Refresh();
        }
        catch
        {
            SceneManager.LoadScene("LoginScene");
            _client.ClientState = ClientState.Standby;
        }
    }

    private System.Threading.Tasks.Task<bool> ShowError() =>
        _UIService.YNMessageBox(_translator.GetText("Arena_ErrorTitle"), _translator.GetText("Arena_ErrorText"), isOnlyYes: true);
}

//hover callbacks only: unlike EventTrigger it does not take the scroll and drag events, so a ScrollRect
//around the hovered item keeps scrolling
public class ArenaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action Enter;
    public Action Exit;

    public void OnPointerEnter(PointerEventData eventData) => Enter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => Exit?.Invoke();
}
