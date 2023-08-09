using BigTwo.Card;
using BigTwo.GameLoop;
using BigTwo.Player;
using BigTwo.System;
using System.Collections.Generic;
using UnityEngine;

namespace BigTwo.UserInterface
{
    public class TableTopUI : MonoBehaviour
    {
        private List<PlayerState> _players;
        private List<CardEntity> _cardsOnTable;

        private CardFactory _cardFactory;

        private GameState _gameState;

        private bool _hasBindedEvents = false;

        private void OnDestroy()
        {
            if (_hasBindedEvents)
            {
                foreach (PlayerState player in _players)
                {
                    player.OnPlayerPlays -= OnPlayerPlays;
                }

                _gameState.OnRoundEnd -= OnRoundEnd;

                _hasBindedEvents = false;
            }
        }

        public void Initialise(GameState gameState, List<PlayerState> players)
        {
            _players = players;
            _cardsOnTable = new List<CardEntity>();

            _gameState = gameState;
            _gameState.OnRoundEnd += OnRoundEnd;

            _cardFactory = CardFactory.Instance;

            foreach (PlayerState player in players)
            {
                player.OnPlayerPlays += OnPlayerPlays;
            }

            _hasBindedEvents = true;
        }

        private void OnRoundEnd()
        {
            RemoveCardsFromTable();
        }

        private void OnPlayerPlays(PlayerState player, List<CardModel> cards)
        {
            RemoveCardsFromTable();

            foreach (CardModel card in cards)
            {
                CardEntity cardEntity = _cardFactory.GetCard();

                if (cardEntity == null)
                {
                    return;
                }

                cardEntity.SetDataAndImage(card);
                cardEntity.SetInteractable(false);
                cardEntity.transform.SetParent(transform);
                cardEntity.transform.localScale = Vector3.one;
                cardEntity.gameObject.SetActive(true);

                _cardsOnTable.Add(cardEntity);
            }
        }

        private void RemoveCardsFromTable()
        {
            foreach (CardEntity card in _cardsOnTable)
            {
                card.Recycle();
            }

            _cardsOnTable.Clear();
        }
    }
}