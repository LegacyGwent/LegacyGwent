using UnityEngine;
using UnityEngine.UI;

public class EnterKeyButtonTrigger : MonoBehaviour
{
    public Button targetButton; // Assign your button in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (targetButton != null && targetButton.interactable)
            {
                targetButton.onClick.Invoke();
            }
        }
    }
}
