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

    void Start()
    {
        if (fields != null && fields.Length > 0)
        {
            // Add listeners to all fields
            foreach (InputField field in fields)
                field.onEndEdit.AddListener((_) => OnEndEdit(field));

            // Focus first field at start and move caret to end
            StartCoroutine(FocusFirstField());
        }
    }

    IEnumerator FocusFirstField()
    {
        // Wait one frame for UI to initialize
        yield return null;

        EventSystem.current.SetSelectedGameObject(fields[0].gameObject);

        // Wait another frame to override Unity's auto-select-all
        yield return null;

        int end = fields[0].text.Length;
        fields[0].caretPosition = end;
        fields[0].selectionAnchorPosition = end;
        fields[0].selectionFocusPosition = end;
    }

    void OnEndEdit(InputField field)
    {
        // Handle Enter key
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (submitButton != null)
                submitButton.onClick.Invoke();

            // Unfocus input
            EventSystem.current.SetSelectedGameObject(null);
        }
        // Handle Tab (and Shift+Tab)
        else if (Input.GetKeyDown(KeyCode.Tab))
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
}
