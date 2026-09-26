using System;
using System.Collections;
using System.Linq;
using Autofac;
using Assets.Script.Localization;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    public sealed class PremiumCollectionPanel : MonoBehaviour
    {
        private EditorInfo editor;
        private Font font;
        private Text wallet, status, craftLabel, selectLabel;
        private Button craft, select;
        private RectTransform actions;
        private righclickLogic detailsOwner;
        private CardStatus current;
        private bool busy, opened;
        private bool catalogAvailable;
        private string error;
        private string pendingRequestId, pendingCardId, pendingAccountId;
        private Button[] showFilters, deckFilters;
        private LegacyTierFilterButton[] groupFilters;
        private static readonly Group?[] CollectionGroups = { null, Group.Gold, Group.Silver, Group.Copper };
        private LocalizationService translator;
        public bool Busy => busy;
        public string CurrentCardId => current?.CardId;
        public void Initialize(EditorInfo owner)
        {
            editor = owner;
            if (!ClientContent.HasPremiumContent) return;
            translator = DependencyResolver.Container.Resolve<LocalizationService>();
            font = owner.ShowSearch.textComponent.font;
            catalogAvailable = PremiumCollectionClient.Ready;
            var currency = Rect("MeteoritePowder", owner.EditorUI.transform, new Vector2(1, 1), new Vector2(-225, -66), new Vector2(370, 65));
            Icon(currency, "Powder", new Vector2(-128, 0), new Vector2(46, 52));
            wallet = Label(currency, "PowderBalance", new Vector2(35, 0), new Vector2(280, 60), 24);
            showFilters = Filters(owner.ShowSearch);
            deckFilters = Filters(owner.EditorSearch);
            BuildGroupFilters();
            actions = Rect("PremiumCraftActions", owner.EditorUI.transform, new Vector2(.5f, .5f), Vector2.zero, new Vector2(380, 100));
            status = Label(actions, "Ownership", new Vector2(0, 51), new Vector2(380, 48), 24);
            craft = Button(actions, "CraftPremium", Vector2.zero, new Vector2(380, 60), out craftLabel);
            craftLabel.fontSize = craftLabel.resizeTextMaxSize = 26;
            craftLabel.resizeTextMinSize = 22;
            Icon(craft.transform, "Powder", new Vector2(-143, 0), new Vector2(25, 32));
            craft.onClick.AddListener(CraftClicked);
            select = Button(actions, "SelectCardVersion", new Vector2(0, -41), new Vector2(350, 32), out selectLabel);
            selectLabel.fontSize = 17;
            select.onClick.AddListener(SelectClicked);
            RefreshLabels();
        }

        public void AttachDetails(RectTransform parent, righclickLogic owner)
        {
            if (actions == null) return;
            detailsOwner=owner;
            actions.SetParent(parent,false);
            actions.anchorMin=actions.anchorMax=new Vector2(.5f,.5f);
            actions.anchoredPosition=new Vector2(407,-367);
            RefreshLabels();
        }
        public void DetachDetails(righclickLogic owner)
        {
            if(detailsOwner!=owner)return;
            detailsOwner=null;
            if(actions!=null && editor!=null) { actions.SetParent(editor.EditorUI.transform,false);actions.gameObject.SetActive(false); }
        }

        private void OnEnable() { PremiumCollectionClient.Changed += AccountChanged; TextLocalization.LanguageChanged += RefreshLabels; }
        private void OnDisable() { PremiumCollectionClient.Changed -= AccountChanged; TextLocalization.LanguageChanged -= RefreshLabels; opened = false; }
        public async void Open()
        {
            if (!ClientContent.HasPremiumContent) return;
            opened = true; current = null; error = null;
            RefreshLabels();
            try { await PremiumCollectionClient.Refresh(); }
            catch (Exception e) { error = "Premium_SyncError"; Debug.LogWarning(e.Message); }
            if (this != null) RefreshLabels();
        }

        public void Preview(CardStatus card)
        {
            if (busy) return;
            if (current?.CardId != card.CardId) error = null;
            current = card;
            RefreshLabels();
        }

        private void AccountChanged()
        {
            if (editor == null) return;
            RefreshLabels();
            if (!opened || busy) return;
            if (!catalogAvailable && PremiumCollectionClient.Ready)
            {
                catalogAvailable = true;
                if (editor.EditorStatus == EditorStatus.ShowCards) editor.AutoSetShowCards();
                else if (editor.EditorStatus == EditorStatus.EditorDeck) editor.AutoSetEditorCards();
            }
            else editor.RefreshPremiumCards();
        }

        private Button[] Filters(InputField search)
        {
            var sr = (RectTransform)search.transform;
            var pos = sr.anchoredPosition;
            sr.sizeDelta = new Vector2(340, sr.sizeDelta.y);
            sr.anchoredPosition = pos + new Vector2(-170, 0);
            var row = Rect("CardVersionFilters", sr.parent, sr.anchorMin, pos + new Vector2(195, 12), new Vector2(416, 76));
            row.anchorMax = sr.anchorMax;
            var labels = new[] { "Premium_Standard", "Premium_Premium", "Premium_All", "Premium_Owned" };
            var icons = new[] { "db_filter_premium_standard", "db_filter_premium_premium", "db_filter_premium_all", "db_filter_owned_owned" };
            var result = new Button[4];
            for (int i = 0; i < result.Length; i++)
            {
                int filter = i;
                Text label;
                result[i] = Button(row, "Filter" + i, new Vector2((i - 1.5f) * 104, 0), new Vector2(50, 50), out label);
                StyleButton(result[i],"btn_square_idle","btn_square_hovered","btn_square_down");
                var selected=Rect("Selected",result[i].transform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(50,50)).gameObject.AddComponent<Image>();
                selected.sprite=Resources.Load<Sprite>("PremiumCrafting/btn_square_toggle_frame");selected.raycastTarget=false;
                label.fontSize = 22; label.resizeTextMaxSize = 22; label.resizeTextMinSize = 18;
                label.rectTransform.sizeDelta = new Vector2(102, 30); label.rectTransform.anchoredPosition = new Vector2(0, -36);
                LocalizedLabel.Set(label, labels[i]);
                Icon((RectTransform)result[i].transform, icons[i], Vector2.zero, new Vector2(28, 32));
                result[i].onClick.AddListener(() => { if (busy) return; current = null; if(filter==3)editor.SetOwnedFilter(!editor.OnlyOwned);else editor.SetPremiumFilter(filter); RefreshLabels(); });
            }
            return result;
        }

        private void BuildGroupFilters()
        {
            var search = editor.ShowSearch.GetComponent<RectTransform>();
            var factions = (RectTransform)editor.ShowButtons[0].transform.parent;
            factions.anchoredPosition -= new Vector2(170, 0);
            search.anchoredPosition = new Vector2(factions.anchoredPosition.x, search.anchoredPosition.y);
            search.sizeDelta = new Vector2(500, search.sizeDelta.y);

            // Fixed header: two rows of four original controls, separate from the masked card list.
            var grid = Rect("CollectionFilters", search.parent, new Vector2(1, 1),
                new Vector2(-174, -70), new Vector2(304, 118));
            var layout = grid.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(70, 56); layout.spacing = new Vector2(8, 6);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount; layout.constraintCount = 4;
            var template = Resources.Load<LegacyTierFilterButton>("LegacyCollectionFilters/TierTemplate");
            groupFilters = new LegacyTierFilterButton[CollectionGroups.Length];
            for (int i = 0; i < groupFilters.Length; i++)
            {
                int index = i;
                var slot = Rect("TierSlot" + i, grid, new Vector2(.5f, .5f), Vector2.zero, layout.cellSize);
                var button = Instantiate(template, slot, false);
                button.name = "GroupFilter" + i;
                button.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                button.transform.localScale = Vector3.one * 1.4f;
                button.SetTier(i);
                groupFilters[i] = button;
                button.onClick.AddListener(() => { if (!busy) editor.SetShowGroup(CollectionGroups[index]); });
            }
            var oldVersionRow = showFilters[0].transform.parent.gameObject;
            for (int i = 0; i < showFilters.Length; i++)
            {
                var slot = Rect("VersionSlot" + i, grid, new Vector2(.5f, .5f), Vector2.zero, layout.cellSize);
                var button = showFilters[i];
                var rect = (RectTransform)button.transform;
                rect.SetParent(slot, false);
                rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one * 1.12f;
                button.GetComponentInChildren<Text>().gameObject.SetActive(false);
            }
            Destroy(oldVersionRow);
            RefreshGroupSelection();
        }

        public void RefreshGroupSelection()
        {
            if (groupFilters == null) return;
            for (int i = 0; i < groupFilters.Length; i++)
            {
                groupFilters[i].SetSelected(editor.ShowGroup == CollectionGroups[i]);
                groupFilters[i].interactable = !busy;
            }
        }

        private void RefreshLabels()
        {
            if (wallet == null) return;
            RefreshGroupSelection();
            wallet.text = LocalizedLabel.Get("Premium_Wallet", PremiumCollectionClient.Ready ? PremiumCollectionClient.Account.MeteoritePowder.ToString("N0", translator.TextLocalization.Culture) : "—");
            actions.gameObject.SetActive(detailsOwner != null && current?.IsPremium == true &&
                (editor.EditorStatus == EditorStatus.ShowCards || editor.EditorStatus == EditorStatus.EditorDeck));
            foreach (var group in new[] { showFilters, deckFilters })
                if (group != null) for (int i = 0; i < group.Length; i++)
                { group[i].transform.Find("Selected").gameObject.SetActive(i == 3 ? editor.OnlyOwned : i == editor.PremiumFilter); group[i].interactable = !busy; }
            if (current == null) return;
            int count = PremiumCollectionClient.Count(current.CardId, true);
            int limit = CardInventory.Limit(current.CardId);
            int cost; bool available = PremiumCollectionClient.Costs.TryGetValue(current.CardId, out cost);
            bool full = count >= limit;
            status.text = (error == null ? null : LocalizedLabel.Get(error)) ?? (PremiumCollectionClient.Ready
                ? LocalizedLabel.Get("Premium_Ownership", CardInventory.Limit(current.CardId), count, limit)
                : LocalizedLabel.Get("Premium_Syncing"));
            craftLabel.text = busy ? LocalizedLabel.Get("Premium_Crafting") : full ? LocalizedLabel.Get("Premium_Full") :
                pendingRequestId != null && pendingCardId == current.CardId ? LocalizedLabel.Get("Premium_Retry") :
                LocalizedLabel.Get("Premium_Craft", available ? cost.ToString() : "—");
            craft.interactable = !busy && !full && available && PremiumCollectionClient.Ready &&
                (PremiumCollectionClient.Account.MeteoritePowder >= cost || pendingRequestId != null);
            craft.gameObject.SetActive(current.IsPremium == true);
            if (!available && PremiumCollectionClient.Ready && error == null)
                status.text = LocalizedLabel.Get("Premium_Unavailable");
            else if (!full && available && PremiumCollectionClient.Ready && PremiumCollectionClient.Account.MeteoritePowder < cost && error == null)
                status.text += LocalizedLabel.Get("Premium_NeedPowder", cost - PremiumCollectionClient.Account.MeteoritePowder);
            select.gameObject.SetActive(false);
        }

        public async void CraftClicked()
        {
            if (!ClientContent.HasPremiumContent) return;
            if (busy || detailsOwner==null || current?.IsPremium != true || !craft.interactable) return;
            string card = current.CardId;
            string accountId = PremiumCollectionClient.Account.Id;
            if (pendingCardId != card || pendingAccountId != accountId)
            { pendingRequestId = null; pendingCardId = card; pendingAccountId = accountId; }
            if (pendingRequestId == null) pendingRequestId = Guid.NewGuid().ToString("N");
            busy = true; RefreshLabels();
            try
            {
                if (this == null || !isActiveAndEnabled || PremiumCollectionClient.Account?.Id != accountId) return;
                var result = await PremiumCollectionClient.Craft(card, pendingRequestId);
                pendingRequestId = null;
                if (this == null || !isActiveAndEnabled || PremiumCollectionClient.Account?.Id != accountId) return;
                if (result.Success)
                {
                    var container = editor.EditorStatus == EditorStatus.EditorDeck ? editor.EditorCardsContext : editor.ShowCardsContent;
                    var targets = container.GetComponentsInChildren<CardShowInfo>()
                        .Where(x => x.CurrentCore.CardId == card && x.CurrentCore.IsPremium == true)
                        .Select(x => x.CardBorder.rectTransform).ToList();
                    var preview = editor.EditorStatus == EditorStatus.EditorDeck ? editor.EditorArtCard : editor.ShowArtCard;
                    if (preview.gameObject.activeInHierarchy && preview.CurrentCore?.CardId == card && preview.CurrentCore.IsPremium == true)
                        targets.Add(preview.CardBorder.rectTransform);
                    var effect = gameObject.AddComponent<PremiumCraftEffect>();
                    var detail=detailsOwner;
                    if(detail!=null && detail.DisplayID==card)targets.Add(detail.CardBorder.rectTransform);
                    await effect.Play(targets, () => {
                        if (this != null && PremiumCollectionClient.Account?.Id == accountId)
                        {
                            editor.RefreshPremiumCards(card);
                            if(detail!=null && detail.DisplayID==card)detail.RefreshCardVisual();
                        }
                    });
                    if (this != null) { current = new CardStatus(card) { IsPremium = true }; error = null; }
                }
                else error = ErrorText(result.Status);
            }
            catch (Exception e)
            {
                error = "Premium_Uncertain"; Debug.LogWarning(e.Message);
                try { await PremiumCollectionClient.Refresh(); } catch { }
            }
            finally
            {
                busy = false;
                if (this != null) { AccountChanged(); RefreshLabels(); }
            }
        }

        public async void SelectClicked()
        {
            if (!ClientContent.HasPremiumContent) return;
            if (busy || current == null || !PremiumCollectionClient.Owns(current.CardId)) return;
            string card = current.CardId;
            busy = true; RefreshLabels();
            try
            {
                var result = await PremiumCollectionClient.Select(card, !PremiumCollectionClient.Selected(card));
                if (this != null)
                {
                    error = result.Success ? null : ErrorText(result.Status);
                    current = new CardStatus(card) { IsPremium = PremiumCollectionClient.Selected(card) };
                    var preview = editor.EditorStatus == EditorStatus.EditorDeck ? editor.EditorArtCard : editor.ShowArtCard;
                    preview.CurrentCore = current;
                }
            }
            catch (Exception e) { error = "Premium_EquipError"; Debug.LogWarning(e.Message); }
            finally { busy = false; if (this != null) AccountChanged(); }
        }

        private string ErrorText(string code) => code == "already_owned" ? "Premium_AlreadyOwned" :
            code == "insufficient_powder" ? "Premium_InsufficientPowder" : "Premium_OperationError";

        public static void MarkCard(CardShowInfo view, CardStatus card)
        {
            PremiumCardAppearance.Apply(view.CardImg, card, view.CardBorder, view.FactionIcon);
        }

        internal static RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchorMin = r.anchorMax = anchor; r.anchoredPosition = position; r.sizeDelta = size;
            return r;
        }
        private Text Label(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize)
        {
            var t = Rect(name, parent, new Vector2(.5f, .5f), pos, size).gameObject.AddComponent<Text>();
            t.font = font; t.fontSize = fontSize; t.alignment = TextAnchor.MiddleCenter; t.color = new Color(.94f, .87f, .66f); t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.resizeTextForBestFit=true; t.resizeTextMinSize=Mathf.Max(10,fontSize-5); t.resizeTextMaxSize=fontSize; return t;
        }
        private Button Button(Transform parent, string name, Vector2 pos, Vector2 size, out Text label)
        {
            var r = Rect(name, parent, new Vector2(.5f, .5f), pos, size);
            var image = r.gameObject.AddComponent<Image>();
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = image;
            StyleWideButton(b);
            label = Label(r, "Label", Vector2.zero, size - new Vector2(8, 4), 21);
            return b;
        }
        // Original InputButtonContainerPrefab/Button_BasicUI, also used by the craft button.
        internal static void StyleWideButton(Button button)
        {
            StyleButton(button, "btn_wide_idle_300", "btn_wide_hovered_300", "btn_wide_down_300", "btn_wide_inactive_300");
        }

        private static void StyleButton(Button button,string normal,string hover,string down,string disabled=null)
        {
            button.image.sprite=Resources.Load<Sprite>("PremiumCrafting/"+normal);
            button.image.color=Color.white;
            button.transition=Selectable.Transition.SpriteSwap;
            button.spriteState=new SpriteState { highlightedSprite=Resources.Load<Sprite>("PremiumCrafting/"+hover),
                pressedSprite=Resources.Load<Sprite>("PremiumCrafting/"+down),
                disabledSprite=disabled==null?button.image.sprite:Resources.Load<Sprite>("PremiumCrafting/"+disabled) };
        }
        private static void Icon(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            var image = Rect(name, parent, new Vector2(.5f, .5f), pos, size).gameObject.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("PremiumCrafting/" + name); image.preserveAspect = true; image.raycastTarget = false;
        }
    }
}
