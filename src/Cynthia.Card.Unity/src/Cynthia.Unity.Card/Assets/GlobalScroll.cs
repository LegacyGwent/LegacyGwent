using UnityEngine;
using UnityEngine.UI;

public class GlobalScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 1f;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Scroll vertically
            float newPos = scrollRect.verticalNormalizedPosition + scroll * scrollSpeed;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPos);
        }
    }
}
