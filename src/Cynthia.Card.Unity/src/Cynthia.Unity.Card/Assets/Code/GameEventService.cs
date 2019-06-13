using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynthia.Card.Client
{
    public class GameEventService
    {
        public void OnMouseDown()
        {
            Debug.Log("鼠标按下");
        }
        public void OnMouseUp()
        {
            Debug.Log("鼠标抬起");
        }
    }
}
