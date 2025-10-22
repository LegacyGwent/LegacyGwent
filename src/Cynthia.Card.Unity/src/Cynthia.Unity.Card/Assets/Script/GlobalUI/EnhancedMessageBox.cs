using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Assets.Script.Localization;
using Autofac;
using UnityEngine.AddressableAssets;
using System.Linq;
using DG.Tweening;

public class EnhancedMessageBox : MessageBox
{
    public Text MessageText2;
    public Text MessageText3;
    public GameObject Border;
    public ScrollRect scrollRect;
    public GameObject slidingArea;
    public GameObject RewardsGridAvatars;
    public Image RewardsGridAvatars_mask;
    public GameObject RewardsGridBorders;
    public Image RewardsGridBorders_mask;
    public GameObject RewardsGridTitles;
    public Image RewardsGridTitles_mask;
    public GameObject TitleRewardPefab;
    public GameObject LeftImage;
    public GameObject RightImage;
    public GameObject MessageBottom;
    private float leftImageStartX;
    private float rightImageStartX;
    private bool isExpanding = false;
    private bool isAnimFinished = true;
    private Sequence fullSequence;
    void Start()
    {
        leftImageStartX = LeftImage.transform.localPosition.x;
        rightImageStartX = RightImage.transform.localPosition.x;
    }
    void ScrollDown()
    {
        if (isExpanding && scrollRect.verticalNormalizedPosition > 0f)
            scrollRect.verticalNormalizedPosition -= 1.0f * Time.deltaTime;

        if (!isExpanding)
            scrollRect.verticalNormalizedPosition -= 1.0f * Time.deltaTime;

        if (!isExpanding && scrollRect.verticalNormalizedPosition <= 0.1f)
        {
            scrollRect.enabled = true;
            slidingArea.SetActive(true);
            Image imageHandle = slidingArea.transform.GetChild(0).GetComponent<Image>();
            imageHandle.color = new Color(imageHandle.color.r, imageHandle.color.g, imageHandle.color.b, 0f);
            imageHandle.DOFade(1.0f, 1.0f);

            isAnimFinished = true;

            CancelInvoke();
            scrollRect.verticalNormalizedPosition = 0.0f;
        }
            
    }
    public Task<bool> Show(
        string title,
        string message,
        string yes = "PopupWindow_YesButton",
        string no = "PopupWindow_NoButton",
        bool isOnlyYes = true,
        string message2 = "",
        string message3 = "",
        IList<string> avatars = null,
        IList<string> borders = null,
        IList<string> titles = null
    )
    {

        const float _scrollLayoutHeight = 0.7f;
        ClearImages();
        for (int i = 0; i < Buttons.transform.childCount; i++)
        {
            Image img = Buttons.transform.GetChild(i).GetComponent<Image>();
            img.DOFade(0.0f, 0.0f);
        }
        leftImageStartX = 0.0f;
        rightImageStartX = 0.0f;

        scrollRect.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollRect.gameObject.GetComponent<RectTransform>().sizeDelta.x, Screen.height * _scrollLayoutHeight);

        scrollRect.enabled = false;
        slidingArea.SetActive(false);

        if (isOnlyYes)
            yes = "PopupWindow_OkButton";
        gameObject.SetActive(true);

        fullSequence = DOTween.Sequence();
        isAnimFinished = false;
        Color currentColor = gameObject.GetComponent<Image>().color;

        fullSequence.Append(gameObject.GetComponent<Image>().DOColor(new Color(currentColor.r, currentColor.g, currentColor.b, 0.93f), 1f).SetEase(Ease.InOutQuad));


        Border.transform.position = new Vector3(Border.transform.position.x, -1000.0f, Border.transform.position.z);
        fullSequence.Join(Border.transform.DOLocalMoveY(0.0f, 1.0f, false).SetEase(Ease.OutElastic));
        float maskFillDuration = 0.38f;
        if (avatars != null && avatars.Any())
        {
            RewardsGridAvatars_mask.gameObject.SetActive(true);
            RewardsGridAvatars.SetActive(true);
            fullSequence.Append(DOTween.To(() => RewardsGridAvatars_mask.fillAmount, x => RewardsGridAvatars_mask.fillAmount = x, 1, maskFillDuration));
            AddImagesToGrid(avatars, fullSequence, 1.2f, RewardsGridAvatars);
        }
        if (borders != null && borders.Any())
        {
            fullSequence.AppendCallback(() =>
            {
                RewardsGridBorders_mask.gameObject.SetActive(true);
                RewardsGridBorders.SetActive(true);
            });
            fullSequence.Append(DOTween.To(() => RewardsGridBorders_mask.fillAmount, x => RewardsGridBorders_mask.fillAmount = x, 1, maskFillDuration));
            AddImagesToGrid(borders, fullSequence, 1.4f, RewardsGridBorders);
        }
        if (titles != null && titles.Any())
        {
            fullSequence.AppendCallback(() =>
            {
                RewardsGridTitles_mask.gameObject.SetActive(true);
                RewardsGridTitles.SetActive(true);
            });
            fullSequence.Append(DOTween.To(() => RewardsGridTitles_mask.fillAmount, x => RewardsGridTitles_mask.fillAmount = x, 1, maskFillDuration));
            AddTitlesToGrid(titles, fullSequence, 1.0f);
        }

        for (int i = 0; i < Buttons.transform.childCount; i++)
        {
            Image img = Buttons.transform.GetChild(i).GetComponent<Image>();
            fullSequence.Append(img.DOFade(1.0f, 0.65f));
        }

        Buttons.SetActive(true);
        if (isOnlyYes)
        {
            YesButton.SetActive(true);
            NoButton.SetActive(false);
        }
        else
        {
            YesButton.SetActive(true);
            NoButton.SetActive(true);
        }

        fullSequence.OnComplete(() =>
        {
            isExpanding = false;
        });



        TitleText.text = _translator.GetText(title);
        MessageText.text = _translator.GetText(message);
        if (message2 != "")
        {
            MessageBottom.SetActive(true);
            MessageText2.gameObject.SetActive(true);
            MessageText2.text = _translator.GetText(message2);
        }
        if (message3 != "")
        {
            MessageText3.gameObject.SetActive(true);
            MessageText3.text = _translator.GetText(message3);
        }

        YesText.text = _translator.GetText(yes);
        NoText.text = _translator.GetText(no);

        scrollRect.verticalNormalizedPosition = 1.0f;
        isExpanding = true;
        InvokeRepeating("ScrollDown", 0.2f, 0.01f);
        RebuildLayouts();
        fullSequence.Play();


        TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        var yesButton = Buttons.transform.GetChild(0).GetComponent<Button>();
        var noButton = Buttons.transform.GetChild(1).GetComponent<Button>();

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            _tcs.TrySetResult(true);
        });

        noButton.onClick.AddListener(() =>
        {
            _tcs.TrySetResult(false);
        });


        return _tcs.Task;

    }
        void AddImagesToGrid(IList<string> images, Sequence full_sequence, float final_scale, GameObject grid)
        {
            float DURATION = 0.22f;
            
            foreach (string image_name in images)
            {
                Image imageObject = new GameObject("Image").AddComponent<Image>();
                imageObject.transform.SetParent(grid.transform, false);
                imageObject.GetComponent<RectTransform>().pivot = new Vector2(0.8f, 0.0f);
                var op = Addressables.LoadAssetAsync<Sprite>(image_name);
                Sprite avatar_img = op.WaitForCompletion();
                imageObject.sprite = avatar_img;

                imageObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                Sequence sequence = DOTween.Sequence();

                sequence.AppendCallback(() =>
                {
                    imageObject.transform.gameObject.SetActive(true);
                });

                sequence.Append(imageObject.transform.DOScale(new Vector3(3.8f, 3.8f, 3.8f), 0.0f).SetEase(Ease.InOutQuad)).OnComplete(() =>
                {
                    imageObject.maskable = false;
                });
                sequence.Append(imageObject.DOFade(0, 0.0f));
                sequence.Append(imageObject.DOFade(1, 0.4f));
                sequence.Join(imageObject.transform.DOScale(new Vector3(final_scale, final_scale, final_scale), DURATION).SetEase(Ease.InOutQuad)).OnComplete(() =>
                {
                    imageObject.maskable = true;


                });
                full_sequence.Append(sequence);
                imageObject.transform.gameObject.SetActive(false);
            }
        }

        void AddTitlesToGrid(IList<string> titlesNames, Sequence full_sequence, float final_scale)
        {
            RewardsGridTitles.SetActive(true);
            const float DURATION = 0.22f;
            foreach (string title_name in titlesNames)
            {

                GameObject titleObject = Instantiate(TitleRewardPefab);
                titleObject.transform.SetParent(RewardsGridTitles.transform, false);
                titleObject.GetComponent<RectTransform>().pivot = new Vector2(0.8f, 0.0f);
                Text titleText = titleObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                titleText.text = _translator.GetText(title_name+"Name");
                titleText.color = ColorMap.colormap[Cynthia.Card.TrinketMap.GetTitles().FirstOrDefault(x => x.ID == title_name)?.TitleColor];
                titleObject.GetComponent<Image>().color = titleText.color;

                titleObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                Sequence sequence = DOTween.Sequence();

                sequence.AppendCallback(() =>
                {
                    titleObject.SetActive(true);
                });


                sequence.Append(titleObject.transform.DOScale(new Vector3(1.9f, 1.9f, 1.9f), 0.0f).SetEase(Ease.InOutQuad)).OnComplete(() =>
                {
                    titleObject.GetComponent<Image>().maskable = false;
                    titleText.maskable = false;
                });

                sequence.Append(titleObject.GetComponent<Image>().DOFade(0.0f, 0.0f));
                sequence.Join(titleText.DOFade(0.0f, 0.0f));
                sequence.Append(titleObject.GetComponent<Image>().DOFade(1.0f, 0.4f));
                sequence.Join(titleText.DOFade(1.0f, 0.4f));

                sequence.Join(titleObject.transform.DOScale(new Vector3(final_scale, final_scale, final_scale), DURATION).SetEase(Ease.InOutQuad)).OnComplete(() =>
                {
                    titleObject.GetComponent<Image>().maskable = true;
                    titleText.maskable = true;
                });

                full_sequence.Append(sequence);
                titleObject.SetActive(false);
            }
        }
    private void ClearImages()
    {
        Color currentColor = gameObject.GetComponent<Image>().color;
        gameObject.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.447f);

        foreach (var grid in new List<Transform>() { RewardsGridAvatars.transform, RewardsGridBorders.transform, RewardsGridTitles.transform })
        {
            for (int i = grid.childCount - 1; i >= 0; i--)
            {
                GameObject child = grid.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
        }
        RewardsGridAvatars_mask.fillAmount = 0;
        RewardsGridBorders_mask.fillAmount = 0;
        RewardsGridTitles_mask.fillAmount = 0;

        MessageText2.gameObject.SetActive(false);
        MessageText3.gameObject.SetActive(false);
        MessageBottom.SetActive(false);
        RewardsGridAvatars.SetActive(false);
        RewardsGridAvatars_mask.gameObject.SetActive(false);
        RewardsGridBorders.SetActive(false);
        RewardsGridBorders_mask.gameObject.SetActive(false);
        RewardsGridTitles.SetActive(false);
        RewardsGridTitles_mask.gameObject.SetActive(false);
        Buttons.SetActive(false);
    }
    
    private void RebuildLayouts()
    {
        Context.gameObject.SetActive(false);
        Context.gameObject.SetActive(true);
    }

    public override void YesClick()
    {
        if (isAnimFinished)
            base.YesClick();
        Destroy(gameObject);
    }
}
