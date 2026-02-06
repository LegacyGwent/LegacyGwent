using UnityEngine;
using UnityEngine.UI;

public class DeckMooveToggle : MonoBehaviour
{
    public static bool DeckMooveMode = false;

    private Button button;
    private Image image;

    public Sprite offSprite; // Button
    public Sprite onSprite;  // ButtonOn

    void Awake()
    {
        button = GetComponent<Button>();
        image = button.targetGraphic as Image;

        ApplyVisualState();
    }

    public void ToggleDeckMooveMode()
    {
        DeckMooveMode = !DeckMooveMode;
        //Debug.Log("DeckMooveMode changed to: " + DeckMooveMode);
        if (DeckMooveMode==false)
        {
            //Debug.Log("Clearing clicks");
            DeckShufler.x=null;
            DeckShufler.y=null;
        }

        ApplyVisualState();
    }

    void ApplyVisualState()
    {
        image.sprite = DeckMooveMode ? onSprite : offSprite;
    }
    public void TurnOff()
    {
        DeckMooveMode=false;
        ApplyVisualState();
    }
}