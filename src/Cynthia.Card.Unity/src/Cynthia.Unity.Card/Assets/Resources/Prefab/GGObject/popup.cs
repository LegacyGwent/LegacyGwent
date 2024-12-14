using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class popup : MonoBehaviour
{
    [SerializeField] Text ggmessage;
    public string gg_message;
    void Start()
    {
        ggmessage.text = gg_message;
        Destroy(gameObject, 5f);

    }
}
