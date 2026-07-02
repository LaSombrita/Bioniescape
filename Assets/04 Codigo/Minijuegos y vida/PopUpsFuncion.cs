using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpsFuncion : MonoBehaviour, IPointerClickHandler
{
    private PopUpMinijuego manager;

    public void Init(PopUpMinijuego m)
    {
        manager = m;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnThoughtClicked(gameObject);
    }



}
