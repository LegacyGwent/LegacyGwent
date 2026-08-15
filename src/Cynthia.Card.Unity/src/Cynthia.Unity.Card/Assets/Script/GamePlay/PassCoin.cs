using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card;
using DG.Tweening;

public class PassCoin : MonoBehaviour
{
    private const float PassHoldDuration = 1.5f;
    private const float PassReleaseDuration = .38f;
    private const int ProgressSegments = 96;

    public GameObject Coin;
    public GameObject FactionIcon;
    public Faction MyFaction;
    public Faction EnemyFaction;
    public Sprite RedIcon;
    public Sprite BlueIcon;
    public Sprite NorthernRealmsIcon;
    public Sprite ScoiaTaelIcon;
    public Sprite MonstersIcon;
    public Sprite SkelligeIcon;
    public Sprite NilfgaardIcon;
    public bool IsCanUse = false;

    public bool IsMyRound
    {   get => _isMyRound;
        set
        {
            PlayCoinTransition(value);
            _isMyRound = value;
        }
    }
    private bool _isMyRound;
    private Vector3 _restPosition;
    private Vector3 _restScale;
    private readonly List<SpriteRenderer> _rimLayers = new List<SpriteRenderer>();
    private SpriteRenderer _faceRenderer;
    private SpriteRenderer _depthShadow;
    private GameObject _progressRoot;
    private LineRenderer _progressGlow;
    private LineRenderer _progressLine;
    private Material _progressMaterial;
    private bool _isPassHolding;
    private bool _isTransitioning;
    private bool _autoPassReady;
    private float _passProgress;

    private void Awake()
    {
        var legacyAnimator = GetComponent<Animator>();
        if (legacyAnimator != null) legacyAnimator.enabled = false;
        _restPosition = transform.localPosition;
        _restScale = transform.localScale;
        BuildCoinRim();
        BuildPassProgressRing();
    }

    private void Update()
    {
        if (_isPassHolding && IsCanUse)
        {
            _passProgress = Mathf.Min(1f, _passProgress + Time.unscaledDeltaTime / PassHoldDuration);
            var intensity = Mathf.SmoothStep(.2f, 1f, _passProgress);
            var time = Time.unscaledTime;
            transform.localPosition = _restPosition + new Vector3(
                Mathf.Sin(time * 39f) * .009f * intensity,
                Mathf.Sin(time * 47f) * .005f * intensity,
                0f);
            transform.localRotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Sin(time * 43f) * .55f * intensity);
            if (_passProgress >= .999f)
            {
                _passProgress = 1f;
                _isPassHolding = false;
                _autoPassReady = true;
            }
        }
        else if (_autoPassReady)
        {
            // Keep the completed ring visible until GameEvent consumes the
            // confirmation in its own update loop.
            _passProgress = 1f;
        }
        else
        {
            if (_isPassHolding && !IsCanUse)
                _isPassHolding = false;

            if (_passProgress > 0f)
                _passProgress = Mathf.MoveTowards(
                    _passProgress,
                    0f,
                    Time.unscaledDeltaTime / PassReleaseDuration);

            if (!_isTransitioning)
            {
                transform.localPosition = _restPosition;
                transform.localRotation = Quaternion.identity;
            }
        }

        SetPassProgress(_passProgress);
    }

    public bool BeginPassHold()
    {
        if (!IsCanUse || _isTransitioning) return false;
        _isPassHolding = true;
        return true;
    }

    public bool EndPassHold()
    {
        _isPassHolding = false;
        transform.localPosition = _restPosition;
        transform.localRotation = Quaternion.identity;
        // A completed hold is consumed automatically by GameEvent. Releasing
        // early only starts the visual rollback and never confirms the pass.
        return false;
    }

    public bool ConsumeAutoConfirmedPass()
    {
        if (!_autoPassReady) return false;
        _autoPassReady = false;
        _passProgress = 0f;
        SetPassProgress(0f);
        transform.localPosition = _restPosition;
        transform.localRotation = Quaternion.identity;
        return true;
    }

    public void CancelPassHold()
    {
        _isPassHolding = false;
        if (!_isTransitioning)
        {
            transform.localPosition = _restPosition;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void ResetPassHold()
    {
        _isPassHolding = false;
        _autoPassReady = false;
        _passProgress = 0f;
        SetPassProgress(0f);
        if (!_isTransitioning)
        {
            transform.localPosition = _restPosition;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void PlayCoinTransition(bool toBlue)
    {
        transform.DOKill(false);
        transform.localPosition = _restPosition;
        transform.localScale = _restScale;
        transform.localRotation = Quaternion.identity;

        var targetSprite = toBlue ? BlueIcon : RedIcon;
        _isTransitioning = true;
        ResetPassHold();
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(_restPosition.y + .34f, .16f).SetEase(Ease.OutCubic));
        sequence.Join(transform.DOLocalRotate(new Vector3(0f, 87f, -7f), .16f, RotateMode.Fast).SetEase(Ease.InQuad));
        sequence.AppendCallback(() =>
        {
            if (_faceRenderer != null) _faceRenderer.sprite = targetSprite;
            if (_depthShadow != null) _depthShadow.sprite = targetSprite;
            _rimLayers.ForEach(x => { if (x != null) x.sprite = targetSprite; });
            transform.localRotation = Quaternion.Euler(0f, -87f, 7f);
        });
        sequence.Append(transform.DOLocalMoveY(_restPosition.y, .19f).SetEase(Ease.InQuad));
        sequence.Join(transform.DOLocalRotate(Vector3.zero, .19f, RotateMode.Fast).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOLocalMoveY(_restPosition.y + .045f, .045f).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOLocalMoveY(_restPosition.y, .065f).SetEase(Ease.InQuad));
        sequence.OnComplete(() =>
        {
            transform.localPosition = _restPosition;
            transform.localScale = _restScale;
            transform.localRotation = Quaternion.identity;
            _isTransitioning = false;
        });
    }

    private void BuildCoinRim()
    {
        _faceRenderer = Coin != null ? Coin.GetComponent<SpriteRenderer>() : null;
        if (_faceRenderer == null || _rimLayers.Count > 0) return;

        _depthShadow = new GameObject("CoinDepthShadow").AddComponent<SpriteRenderer>();
        _depthShadow.transform.SetParent(_faceRenderer.transform, false);
        _depthShadow.transform.localPosition = new Vector3(.025f, -.055f, .075f);
        _depthShadow.transform.localScale = new Vector3(1.025f, 1.025f, 1f);
        _depthShadow.sprite = _faceRenderer.sprite;
        _depthShadow.sortingLayerID = _faceRenderer.sortingLayerID;
        _depthShadow.sortingOrder = _faceRenderer.sortingOrder - 8;
        _depthShadow.color = new Color32(24, 29, 31, 225);

        for (var i = 6; i >= 1; i--)
        {
            var layer = new GameObject($"CoinRim{i}").AddComponent<SpriteRenderer>();
            layer.transform.SetParent(_faceRenderer.transform, false);
            layer.transform.localPosition = new Vector3(.004f * i, -.012f * i, .014f * i);
            layer.transform.localScale = Vector3.one;
            layer.sprite = _faceRenderer.sprite;
            layer.sortingLayerID = _faceRenderer.sortingLayerID;
            layer.sortingOrder = _faceRenderer.sortingOrder - i;
            var shade = (byte)(58 + (6 - i) * 9);
            layer.color = new Color32(shade, (byte)(shade - 6), (byte)(shade - 16), 245);
            _rimLayers.Add(layer);
        }
    }

    private void BuildPassProgressRing()
    {
        if (_progressRoot != null || transform.parent == null) return;

        _progressRoot = new GameObject("PassHoldProgress");
        _progressRoot.transform.SetParent(transform.parent, false);
        _progressRoot.transform.localPosition = _restPosition;
        _progressRoot.transform.localRotation = Quaternion.identity;
        _progressRoot.transform.localScale = _restScale;

        var shader = Shader.Find("Sprites/Default");
        if (shader == null) return;
        _progressMaterial = new Material(shader) { name = "PassHoldProgressMaterial" };

        _progressGlow = CreateProgressLine("Glow", .19f, new Color32(15, 116, 255, 70), 7);
        _progressLine = CreateProgressLine("Core", .068f, new Color32(85, 205, 255, 250), 8);
        SetPassProgress(0f);
    }

    private LineRenderer CreateProgressLine(string name, float width, Color color, int sortingOffset)
    {
        var lineObject = new GameObject(name);
        lineObject.transform.SetParent(_progressRoot.transform, false);
        var line = lineObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = false;
        line.alignment = LineAlignment.View;
        line.textureMode = LineTextureMode.Stretch;
        line.numCornerVertices = 4;
        line.numCapVertices = 4;
        line.startWidth = width;
        line.endWidth = width;
        line.startColor = color;
        line.endColor = new Color(
            Mathf.Min(1f, color.r + .18f),
            Mathf.Min(1f, color.g + .12f),
            1f,
            color.a);
        line.material = _progressMaterial;
        line.sortingLayerID = _faceRenderer != null ? _faceRenderer.sortingLayerID : 0;
        line.sortingOrder = (_faceRenderer != null ? _faceRenderer.sortingOrder : 0) + sortingOffset;
        return line;
    }

    private void SetPassProgress(float progress)
    {
        if (_progressGlow == null || _progressLine == null) return;
        progress = Mathf.Clamp01(progress);
        var visible = progress > .001f;
        _progressGlow.enabled = visible;
        _progressLine.enabled = visible;
        if (!visible) return;

        var isComplete = progress >= .999f;
        _progressGlow.loop = isComplete;
        _progressLine.loop = isComplete;
        var exactSegments = progress * ProgressSegments;
        var completeSegments = Mathf.FloorToInt(exactSegments);
        var partialSegment = exactSegments - completeSegments;
        var pointCount = isComplete
            ? ProgressSegments
            : completeSegments + 1 + (partialSegment > .001f && completeSegments < ProgressSegments ? 1 : 0);
        pointCount = Mathf.Max(2, pointCount);
        _progressGlow.positionCount = pointCount;
        _progressLine.positionCount = pointCount;

        for (var index = 0; index < pointCount; index++)
        {
            var segment = index;
            if (index == pointCount - 1 && partialSegment > .001f && completeSegments < ProgressSegments)
                segment = completeSegments;
            var fraction = index == pointCount - 1 && partialSegment > .001f && completeSegments < ProgressSegments
                ? (completeSegments + partialSegment) / ProgressSegments
                : Mathf.Min(segment, ProgressSegments) / (float)ProgressSegments;
            // Begin at twelve o'clock and fill clockwise, matching a familiar
            // hold-to-confirm progress dial.
            var radians = (90f - 360f * fraction) * Mathf.Deg2Rad;
            var position = new Vector3(Mathf.Cos(radians) * 2.87f, Mathf.Sin(radians) * 2.87f, -.03f);
            _progressGlow.SetPosition(index, position);
            _progressLine.SetPosition(index, position);
        }
    }

    private void OnDestroy()
    {
        if (_progressMaterial != null)
            Destroy(_progressMaterial);
    }

    private void CoinToRed()
    {
        Coin.GetComponent<SpriteRenderer>().sprite = RedIcon;
        gameObject.GetComponent<Animator>().Play("CoinShow");
    }
    private void CoinToBlue()
    {
        Coin.GetComponent<SpriteRenderer>().sprite = BlueIcon;
        gameObject.GetComponent<Animator>().Play("CoinShow");
    }
}
