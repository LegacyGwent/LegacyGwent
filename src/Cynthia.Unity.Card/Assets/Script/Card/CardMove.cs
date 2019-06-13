using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMove : MonoBehaviour {
    public bool UsePointerDisplacement = true;
    private bool dragging = false;
    private Vector3 pointerDisplacement = Vector3.zero;
    private float zDisplacement;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (dragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x-pointerDisplacement.x,mousePos.y-pointerDisplacement.y,transform.position.z);
        }
	}
    private void OnMouseDown()
    {
        dragging = true;
        zDisplacement = -Camera.main.transform.position.z + transform.position.z;
        if (UsePointerDisplacement)
        {
            pointerDisplacement = -Camera.main.transform.position + MouseInWorldCoords();
        }
        else
        {
            pointerDisplacement = Vector3.zero;
        }
    }
    private void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
        }
    }
    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        screenMousePos.z = zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
}
