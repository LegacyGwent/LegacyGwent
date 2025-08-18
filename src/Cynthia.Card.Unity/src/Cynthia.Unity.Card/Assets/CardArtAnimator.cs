using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardArtAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationTime = 0.25f;
    public float scale12 = 0.8f;
    public float scale9 = 1f;
    public float scale6 = 1.2f;
    public float pivotOffset = 3f;

    [Header("Concurrency Settings")]
    public static int maxConcurrentAnimations = 1;
    private static List<CardArtAnimator> activeAnimations = new List<CardArtAnimator>();

    private Coroutine animatingCoroutine = null;
    private bool isAnimating = false;

    public void PlayEnter()
    {
        // Respect max concurrent animations
        if (activeAnimations.Count >= maxConcurrentAnimations)
        {
            CardArtAnimator oldest = activeAnimations[0];
            activeAnimations.RemoveAt(0);
            StopAnimationImmediately(oldest);
        }

        // Add self to active animations
        activeAnimations.Add(this);

        SetTo12();
        if (animatingCoroutine != null)
            StopCoroutine(animatingCoroutine);

        animatingCoroutine = StartCoroutine(AnimateCardRelative(-90f, scale12, scale9));
    }

    public void PlayExit()
    {
        if (animatingCoroutine != null)
            StopCoroutine(animatingCoroutine);

        animatingCoroutine = StartCoroutine(AnimateCardRelative(-90f, scale9, scale6, true));
    }

    private IEnumerator AnimateCardRelative(float deltaAngleTotal, float startScaleValue, float endScaleValue, bool destroy = false)
    {
        //yield return new WaitForSeconds(0.1f);
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

            Vector3 pivotWorldPos = transform.position + transform.right * pivotOffset;
            transform.RotateAround(pivotWorldPos, Vector3.up, step);
            rotatedSoFar += step;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        isAnimating = false;
        animatingCoroutine = null;

        if (destroy)
        {
            if (transform.parent != null)
                transform.parent.gameObject.SetActive(false);
            else
                gameObject.SetActive(false);

            activeAnimations.Remove(this);
        }
    }

    public void SetTo12()
    {
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

        float worldOffset = pivotOffset * cardWidth;
        transform.localPosition = Vector3.right * worldOffset;
        transform.localRotation = Quaternion.Euler(0, 90f, 0);
        transform.localScale = Vector3.one * scale12;
    }

    public void SetTo6()
    {
        transform.localRotation = Quaternion.Euler(0, 270f, 0);
        transform.localScale = Vector3.one * scale6;
    }

    private void StopAnimationImmediately(CardArtAnimator animator)
    {
        if (animator.animatingCoroutine != null)
            animator.StopCoroutine(animator.animatingCoroutine);

        animator.animatingCoroutine = null;
        animator.isAnimating = false;

        // Snap instantly to exit state
        animator.SetTo6();
        if (animator.transform.parent != null)
            animator.transform.parent.gameObject.SetActive(false);
        else
            animator.gameObject.SetActive(false);

        activeAnimations.Remove(animator);
    }
}
