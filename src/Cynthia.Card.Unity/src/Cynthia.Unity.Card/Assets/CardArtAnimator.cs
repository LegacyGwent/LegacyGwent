using UnityEngine;
using System.Collections;

public class CardArtAnimator : MonoBehaviour
{
    [Header("Animation time")]
    public float animationTime = 0.5f;

    [Header("Scale Settings")]
    public float scale12 = 0.8f;   // far away (12 o'clock, deep in screen)
    public float scale9 = 1f;      // side view (9 o'clock)
    public float scale6 = 1.2f;    // close to camera (6 o'clock)

    [Header("Pivot Settings")]
    [Tooltip("Offset to the right of the card where rotation happens")]
    public float pivotOffset = 3f;

    private bool isAnimating = false;
    private Vector3 pivotPoint; // fixed pivot

    private void Start()
    {
        // Store the fixed pivot position once
        pivotPoint = transform.position + transform.right * pivotOffset;

        // Start hidden
        SetTo12();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) PlayEnter();
        if (Input.GetKeyDown(KeyCode.X)) PlayExit();
    }

    public void PlayEnter()
    {
        if (isAnimating) return;
        StopAllCoroutines();
        SetTo12();
        StartCoroutine(AnimateCardRelative(-90f, scale12, scale9)); // 12 → 9
    }

    public void PlayExit()
    {
        //if (isAnimating) return;
        //StopAllCoroutines();
        //SetTo9();
        StartCoroutine(AnimateCardRelative(-90f, scale9, scale6)); // 9 → 6
    }

    private IEnumerator AnimateCardRelative(float deltaAngleTotal, float startScaleValue, float endScaleValue)
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
            transform.RotateAround(pivotPoint, Vector3.up, step);
            rotatedSoFar += step;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        isAnimating = false;
    }

    [ContextMenu("Set to 12")]
    public void SetTo12()
    {
        transform.position = pivotPoint + Quaternion.Euler(0, 90f, 0) * Vector3.left * pivotOffset;
        transform.rotation = Quaternion.Euler(0, 90f, 0);
        transform.localScale = Vector3.one * scale12;
    }

    [ContextMenu("Set to 9")]
    public void SetTo9()
    {
        transform.position = pivotPoint + Quaternion.Euler(0, 0f, 0) * Vector3.left * pivotOffset;
        transform.rotation = Quaternion.Euler(0, 0f, 0);
        transform.localScale = Vector3.one * scale9;
    }

    [ContextMenu("Set to 6")]
    public void SetTo6()
    {
        transform.position = pivotPoint + Quaternion.Euler(0, 270f, 0) * Vector3.left * pivotOffset;
        transform.rotation = Quaternion.Euler(0, 270f, 0);
        transform.localScale = Vector3.one * scale6;
    }

    private void SetRotationAndScale(float targetAngle, float scale)
    {
        float currentAngle = GetCurrentYRotationAroundPivot(pivotPoint);
        transform.RotateAround(pivotPoint, Vector3.up, targetAngle - currentAngle);
        transform.localScale = Vector3.one * scale;
    }

    private float GetCurrentYRotationAroundPivot(Vector3 pivot)
    {
        Vector3 dir = transform.position - pivot;
        return Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }
}
