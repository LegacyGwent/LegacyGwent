using UnityEngine;
using System.Collections;

public class CardArtAnimator : MonoBehaviour
{
    [Header("Animation time")]
    public float animationTime = 0.5f;

    [Header("Scale Settings")]
    public float scale12 = 0.8f;
    public float scale9 = 1f;
    public float scale6 = 1.2f;

    [Header("Pivot Settings")]
    [Tooltip("Offset to the right of the card where rotation happens")]
    public float pivotOffset = 3f;

    private bool isAnimating = false;

    private void Start()
    {
        // Nothing to offset on start
        // Card will start at parent position
    }
    [ContextMenu("Play Enter Animation")]
    public void PlayEnter()
    {
        Debug.Log("ENTER");
        SetTo12();
        StartCoroutine(AnimateCardRelative(-90f, scale12, scale9));
    }

    public void PlayExit()
    {
        Debug.Log("EXIT");
        StartCoroutine(AnimateCardRelative(-90f, scale9, scale6, true));
    }

    private IEnumerator AnimateCardRelative(float deltaAngleTotal, float startScaleValue, float endScaleValue, bool destroy = false)
    {
        isAnimating = true;
        float elapsed = 0f;
        float rotatedSoFar = 0f;

        Vector3 startScale = Vector3.one * startScaleValue;
        Vector3 endScale = Vector3.one * endScaleValue;

        while (elapsed < animationTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animationTime);

            float targetRotation = Mathf.Lerp(0f, deltaAngleTotal, t);
            float step = targetRotation - rotatedSoFar;

            // Rotate around pivot relative to current card position
            Vector3 pivotWorldPos = transform.position + transform.right * pivotOffset;
            transform.RotateAround(pivotWorldPos, Vector3.up, step);
            rotatedSoFar += step;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        isAnimating = false;

        if (destroy)
        {
            // Destroy parent of this animator (the prefab root)
            Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
        }
    }

    public void SetTo12()
    {
        // Determine card width in world units
        float cardWidth = 1f;

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
            cardWidth = rt.rect.width;
        else
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
                cardWidth = rend.bounds.size.x;
        }

        // Move the card to the right so the pivot is at the parent's position
        float worldOffset = pivotOffset * cardWidth;
        transform.localPosition = Vector3.right * worldOffset;

        // Set rotation and scale
        transform.localRotation = Quaternion.Euler(0, 90f, 0);
        transform.localScale = Vector3.one * scale12;
    }

    public void SetTo9()
    {
        transform.localRotation = Quaternion.Euler(0, 0f, 0);
        transform.localScale = Vector3.one * scale9;
    }

    public void SetTo6()
    {
        transform.localRotation = Quaternion.Euler(0, 270f, 0);
        transform.localScale = Vector3.one * scale6;
    }
}
