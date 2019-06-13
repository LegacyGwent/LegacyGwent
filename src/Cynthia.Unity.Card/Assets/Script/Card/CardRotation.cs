using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CardRotation : MonoBehaviour
{

    public RectTransform CardFace;
    public RectTransform CardBack;
    public Transform targetFacePoint;
    public Collider col;
    private bool showingBack;
    // Update is called once per frame
    
    void Update()
    {
        RaycastHit[] hits;
        //Debug.DrawLine(Camera.main.transform.position, (-Camera.main.transform.position + targetFacePoint.position).normalized);
        hits = Physics.RaycastAll(origin: Camera.main.transform.position,
                                  direction: (-Camera.main.transform.position + targetFacePoint.position).normalized,
                                  maxDistance: (-Camera.main.transform.position + targetFacePoint.position).magnitude);
        Debug.DrawLine(Camera.main.transform.position,(-Camera.main.transform.position + targetFacePoint.position).normalized);
        bool passedThroughColliderOnCard = false;
        Debug.Log(hits.Length);
        foreach (RaycastHit h in hits)
        {
            Debug.DrawLine(Camera.main.transform.position, h.transform.position);
            Debug.Log(h.transform.position);
            if(h.collider == col)
            {
                passedThroughColliderOnCard = true;
            }
        }
        if(passedThroughColliderOnCard != showingBack)
        {
            showingBack = passedThroughColliderOnCard;
            if(showingBack)
            {
                CardFace.gameObject.SetActive(false);
                CardBack.gameObject.SetActive(true);
            }
            else
            {
                CardFace.gameObject.SetActive(true);
                CardBack.gameObject.SetActive(false);
            }
        }
    }
}
