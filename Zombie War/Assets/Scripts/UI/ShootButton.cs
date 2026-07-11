using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameEventHandler.StartShooting();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameEventHandler.StopShooting();
    }
}
