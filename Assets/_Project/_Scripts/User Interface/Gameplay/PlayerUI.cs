using BigTwo.Card;
using BigTwo.GameLoop;
using BigTwo.Player;
using BigTwo.System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BigTwo.UserInterface
{
    public abstract class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        protected Canvas _handCanvas;
        [SerializeField]
        protected Transform _handContainer;
        [SerializeField]
        protected TextMeshProUGUI _handCounterText;

        protected CardFactory _cardFactory;
        protected List<CardEntity> _cardsOnHand;

        protected PlayerState _assignedPlayer;

        public virtual void Initialise(PlayerState playerState)
        {
            _cardFactory = CardFactory.Instance;
            _cardsOnHand = new List<CardEntity>(13);

            _assignedPlayer = playerState;
            _assignedPlayer.OnPlayerAction += OnPlayerAction;
            _assignedPlayer.OnInitialCardsDealt += OnInitialCardsDealt;
            _assignedPlayer.OnPlayerPlays += OnPlayerPlays;
            _assignedPlayer.OnPlayerPasses += OnPlayerPasses;
        }

        public virtual void SetToDefault()
        {
            _cardFactory = null;
            _cardsOnHand = new List<CardEntity>(13);

            _assignedPlayer.OnInitialCardsDealt -= OnInitialCardsDealt;
            _assignedPlayer.OnPlayerAction -= OnPlayerAction;
            _assignedPlayer.OnPlayerPlays -= OnPlayerPlays;
            _assignedPlayer.OnPlayerPasses -= OnPlayerPasses;
            _assignedPlayer = null;
        }

        protected void RemoveCardsFromHand(List<CardModel> cards)
        {
            List<CardEntity> entitiesToRemove = new List<CardEntity>();

            foreach (CardModel card in cards)
            {
                foreach (CardEntity entity in _cardsOnHand)
                {
                    if (entity.CardModel == card)
                    {
                        entitiesToRemove.Add(entity);
                        break;
                    }
                }
            }

            foreach (CardEntity entity in entitiesToRemove)
            {
                _cardsOnHand.Remove(entity);

                entity.Recycle();
            }
        }

        protected abstract void OnInitialCardsDealt(PlayerState player, List<CardModel> cards);

        protected abstract void OnPlayerAction(PlayerState player, GameInfo gameInfo);

        protected abstract void OnPlayerPlays(PlayerState player, List<CardModel> cards);

        protected abstract void OnPlayerPasses(PlayerState player);
    }
}