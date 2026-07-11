using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameEventHandler.SwitchWeapon();
    }
}