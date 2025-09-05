using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldSubmit : MonoBehaviour
{
    public InputField inputField; // assign in Inspector
    public Button submitButton;   // assign in Inspector

    void Start()
    {
        // Add listener to detect submit (Enter key)
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    void OnEndEdit(string text)
    {
        // Only trigger if Enter was pressed (not just losing focus)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            submitButton.onClick.Invoke(); // simulate button click
            EventSystem.current.SetSelectedGameObject(null); // remove focus from input
        }
    }
}
