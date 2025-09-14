using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class KeyListener : MonoBehaviour
{
    [Header("Fields in tab order")]
    public InputField[] fields;

    [Header("Submit button (pressed on Enter)")]
    public Button submitButton;
    [Header("Button blocker")]
    public GameObject blocker;
    void Start()
    {
        if (fields != null && fields.Length > 0)
        {
            foreach (InputField field in fields)
            {
                field.onEndEdit.AddListener((_) => OnEndEdit(field));

                // Add EventTrigger to detect Tab while selected
                EventTrigger trigger = field.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = field.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.UpdateSelected;
                entry.callback.AddListener((data) => { OnUpdateSelected(field); });
                trigger.triggers.Add(entry);
            }

            // Focus first field at start
#if !UNITY_ANDROID            
            StartCoroutine(FocusFirstField());
#endif
        }
    }

    IEnumerator FocusFirstField()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(fields[0].gameObject);
        yield return null;

        int end = fields[0].text.Length;
        fields[0].caretPosition = end;
        fields[0].selectionAnchorPosition = end;
        fields[0].selectionFocusPosition = end;
    }

    void OnEndEdit(InputField field)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!blocker.activeInHierarchy)
            {
                submitButton.onClick.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                StartCoroutine(WaitAndLogin());
            }
        }
    }

    void OnUpdateSelected(InputField field)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool forward = !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            SelectNextField(field, forward);
        }
    }

    void SelectNextField(InputField currentField, bool forward)
    {
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i] == currentField)
            {
                int next = forward ? (i + 1) % fields.Length : (i - 1 + fields.Length) % fields.Length;
                EventSystem.current.SetSelectedGameObject(fields[next].gameObject);
                break;
            }
        }
    }
    IEnumerator WaitAndLogin()
    {
        while (blocker.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }

        submitButton.onClick.Invoke();
        EventSystem.current.SetSelectedGameObject(null);
    }
}
