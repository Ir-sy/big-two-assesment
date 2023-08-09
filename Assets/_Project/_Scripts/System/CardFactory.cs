using BigTwo.Card;
using System.Collections.Generic;
using UnityEngine;

namespace BigTwo.System
{
    public class CardFactory : MonoBehaviour
    {
        public static CardFactory Instance;

        [SerializeField]
        private CardEntity _cardEntityBlueprint;

        private Queue<CardEntity> _cardQueue = new(53);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Pool(CardEntity card)
        {
            if (card == null)
            {
                Debug.LogWarning("Pool : Attempt to pool a null ref card");
                return;
            }

            card.transform.SetParent(transform);
            card.gameObject.SetActive(false);
            card.OnLifetimeEnded -= Pool;

            _cardQueue.Enqueue(card);
        }

        public CardEntity GetCard()
        {
            if (_cardEntityBlueprint == null)
            {
                Debug.LogWarning("GetCard : Card entity prefab is not assigned");
                return null;
            }

            CardEntity card = _cardQueue.Count > 0 ? _cardQueue.Dequeue() : Instantiate(_cardEntityBlueprint);
            card.OnLifetimeEnded += Pool;

            return card;
        }
    }
}