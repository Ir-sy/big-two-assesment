using BigTwo.Card;
using BigTwo.GameLoop;
using BigTwo.Player;
using System.Collections.Generic;
using UnityEngine;

namespace BigTwo.UserInterface
{
    public class OtherPlayerUI : PlayerUI
    {
        protected override void OnInitialCardsDealt(PlayerState player, List<CardModel> cards)
        {
            if (player == null)
            {
                Debug.LogWarning("OnPlayerDealtCards : Given player ref is null");
                return;
            }

            if (cards == null)
            {
                Debug.LogWarning("OnPlayerDealtCards : Given cards ref is null");
                return;
            }

            if (_handContainer == null)
            {
                Debug.LogWarning("OnPlayerDealtCards : Hand container is null");
                return;
            }

            if (_cardFactory == null)
            {
                Debug.LogWarning("OnPlayerDealtCards : Card factory is null");
                return;
            }

            foreach (CardModel card in cards)
            {
                CardEntity cardEntity = _cardFactory.GetCard();

                if (cardEntity == null)
                {
                    continue;
                }

                cardEntity.SetData(card);
                cardEntity.SetInteractable(false);
                cardEntity.SetImageColourToNonLocalPlayer();
                cardEntity.transform.SetParent(_handContainer.transform);
                cardEntity.transform.localScale = Vector3.one;
                cardEntity.gameObject.SetActive(true);

                _cardsOnHand.Add(cardEntity);
            }

            if (_handCounterText != null)
            {
                _handCounterText.SetText(player.Hand.Count.ToString());
            }
        }

        protected override void OnPlayerAction(PlayerState player, GameInfo gameInfo) { }

        protected override void OnPlayerPasses(PlayerState player) { }

        protected override void OnPlayerPlays(PlayerState player, List<CardModel> cards)
        {
            if (_handCounterText != null)
            {
                _handCounterText.SetText(player.Hand.Count.ToString());
            }

            RemoveCardsFromHand(cards);
        }
    }
}