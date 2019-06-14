using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Cynthia.Card.Client;
using Autofac;
using DG.Tweening;

public class CardMoveInfo : MonoBehaviour
{
    public CardShowInfo CardShowInfo;
    public Vector2 ResetPoint;//应该在的位置
    public float ZPosition
    {
        get => _zPosition; set
        {
            _zPosition = value;
            transform.position = new Vector3(transform.position.x, transform.position.y, _zPosition);
        }
    }
    private float _zPosition;
    public bool IsCanDrag = true;//是否能拖动
    public bool IsCanSelect = true;
    public bool IsStay = false;
    public bool IsDrag = false;//是否正在拖动
    public bool IsTem = false;//是否是临时卡
    //private bool IsRestore = false;//是否还原
    public bool IsOn
    {
        get => _isOn; set
        {
            if (value)
            {
                //transform.localScale = Vector3.one * 1.15f;
                CardShowInfo.ScaleTo(1.2f);
                if (!_isOn) ZPosition -= 2;
                _isOn = value;
            }
            else
            {
                if (_isOn) ZPosition += 2;
                _isOn = value;
                var p = transform.parent.GetComponent<CardsPosition>();
                if (p != null)
                    p.ResetCards();
            }
        }
    }//是否处于抬起状态(锁定变大)
    private bool _isOn = false;
    public float Speed = 35f;
    public CardUseInfo CardUseInfo = CardUseInfo.AnyPlace;


    void FixedUpdate()
    {
        var tagetText = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //下一帧应该移动到的位置
        if (IsStay) return;
        if (IsDrag)
        {
            //if (IsRestore) IsRestore = false;
            Speed = 30f;
            SetNextPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), Speed, Space.World);
            var taget = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.2f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, ZPosition);
        }
        else
        {
            //if (!IsRestore)
            //{
            var taget = Vector2.Lerp(transform.localPosition, ResetPoint, 0.25f);
            transform.localPosition = new Vector3(taget.x, taget.y, ZPosition);
            //transform.DOLocalMove(new Vector3(ResetPoint.x,ResetPoint.y,ZPosition),0.5f).SetEase(Ease.Linear);
            //IsRestore = true;
            //}
        }
    }
    public bool SetNextPosition(Vector2 taget, float speed, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
            Debug.DrawLine(taget, transform.localPosition);
        else
            Debug.DrawLine(taget, transform.position);
        var dir = default(Vector2);
        if (relativeTo == Space.World)
            dir = taget - new Vector2(transform.position.x, transform.position.y);//指定方向
        else
            dir = taget - new Vector2(transform.localPosition.x, transform.localPosition.y);//指定方向
        float distance = speed * Time.deltaTime;
        if (dir.magnitude <= distance)
        {
            if (relativeTo == Space.World)
                transform.position = new Vector3(taget.x, taget.y, transform.position.z);
            else
                transform.localPosition = new Vector3(taget.x, taget.y, transform.localPosition.z);
            return false;
        }
        var nextPoint = dir.normalized * distance;
        transform.Translate(new Vector3(nextPoint.x, nextPoint.y, 0));
        return true;
    }
    public void SetResetPoint(Vector3 rPoint)
    {
        //设置默认位置
        ResetPoint = rPoint;
        ZPosition = rPoint.z;
    }
    public void Destroy()
    {
        var row = transform.parent.GetComponent<CardsPosition>();
        transform.SetParent(null);
        Destroy(gameObject);
        row?.ResetCards();
    }
}
