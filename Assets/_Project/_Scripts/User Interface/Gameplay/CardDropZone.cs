using BigTwo.Card;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BigTwo.UserInterface
{
    public class CardDropZone : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            CardEntity cardEntity = eventData.pointerDrag.GetComponent<CardEntity>();

            //cardEntity.OnDropped(this);
        }
    }
}