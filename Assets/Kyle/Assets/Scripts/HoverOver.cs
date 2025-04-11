using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOver : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.fontSize += 5;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.fontSize -= 5;
    }
}
