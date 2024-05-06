using System.Collections;
using System.Collections.Generic;
//using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBase : MonoBehaviour
{

    [SerializeField]
    public RectTransform _cursor;

    //Script Variables
    public PhysicalButton currentPhysicalButton;
    public RectTransform _rectTransform;
    public Vector2 _halfBounds;



    void Start()
    {
        _rectTransform = transform as RectTransform;
        _halfBounds = new Vector2(_rectTransform.sizeDelta.x / 2, _rectTransform.sizeDelta.y / -2);
    }

    private void RenderCursor(Vector2 position)
    {
        _cursor.anchoredPosition = position;
    }



    #region Public Access

    public void SetRaycatHit(Vector3 position)
    {
        _cursor.gameObject.SetActive(true);

        Vector3 pos = transform.InverseTransformPoint(position);
        RenderCursor(new Vector2(pos.x, pos.y));
    }



    public void SetNoRaycastHit()
    {
        _cursor.gameObject.SetActive(false);
    }

    #endregion



    #region Internal


    #endregion

    public bool CalculateHoverButton(PhysicalButton button)
    {
        Vector3 pos = _cursor.anchoredPosition; //+ _halfBounds;

        //Debug.Log("Pos X" + pos.x);
        //if (button.gameObject.activeSelf)
        //	{


        if (pos.x > button.RectTransform.anchoredPosition.x && pos.x < button.RectTransform.anchoredPosition.x + button.RectTransform.sizeDelta.x)
        {
            if (pos.y > button.RectTransform.anchoredPosition.y && pos.y < button.RectTransform.anchoredPosition.y + button.RectTransform.sizeDelta.y)
            {

                if (currentPhysicalButton != button)
                {
                    StopCurrentHover();
                    currentPhysicalButton = button;
                    currentPhysicalButton.EV_MainTriggerStart();
                    //_currentHoveredButton.SetHover(true);
                }

                return true;

            }
        }

        return false;
        //	}

        //	return false;
    }


    public void StopCurrentHover()
    {
        currentPhysicalButton?.StopHover();
        currentPhysicalButton = null;
    }
}
