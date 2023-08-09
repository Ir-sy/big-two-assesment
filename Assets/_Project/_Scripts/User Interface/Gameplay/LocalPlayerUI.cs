using BigTwo.Card;
using BigTwo.GameLoop;
using BigTwo.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo.UserInterface
{
    public class LocalPlayerUI : PlayerUI
    {
        [Header("Local Interaction UI")]
        [SerializeField]
        private Button _buttonPlay;
        [SerializeField]
        private Button _buttonPass;

        private GameInfo _currentGameInfo;
        private List<CardModel> _cardsToPlay;

        private void Awake()
        {
            _cardsToPlay = new List<CardModel>();

            _buttonPlay.onClick.AddListener(PlayButtonPressed);
            _buttonPass.onClick.AddListener(PassButtonPressed);
        }

        private void OnDestroy()
        {
            _buttonPlay.onClick.RemoveListener(PlayButtonPressed);
            _buttonPass.onClick.RemoveListener(PassButtonPressed);
        }

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

            List<CardModel> sortedCards = cards.Sorted();

            foreach (CardModel card in sortedCards)
            {
                CardEntity cardEntity = _cardFactory.GetCard();

                if (cardEntity == null)
                {
                    continue;
                }

                cardEntity.SetDataAndImage(card);
                cardEntity.SetInteractableWithColour(false);
                cardEntity.transform.SetParent(_handContainer.transform);
                cardEntity.transform.localScale = Vector3.one;
                cardEntity.gameObject.SetActive(true);

                cardEntity.OnClicked += OnCardClicked;

                _cardsOnHand.Add(cardEntity);
            }

            SetLocalInteractionButtonsVisible(false);
        }

        protected override void OnPlayerAction(PlayerState player, GameInfo gameInfo)
        {
            _currentGameInfo = gameInfo;

            _cardsToPlay.Clear();

            SetLocalInteractionUIAsOnActionStart();
        }

        protected override void OnPlayerPlays(PlayerState player, List<CardModel> cards)
        {
            RemoveCardsFromHand(cards);

            SetLocalInteractionUIAsOnActionEnd();
        }

        protected override void OnPlayerPasses(PlayerState player)
        {
            SetLocalInteractionUIAsOnActionEnd();
        }

        private void OnCardClicked(CardEntity card)
        {
            bool isToPlay = _cardsToPlay.Contains(card.CardModel);

            if (isToPlay)
            {
                _cardsToPlay.Remove(card.CardModel);

                card.SetPositionToDefault();
            }
            else
            {
                _cardsToPlay.Add(card.CardModel);

                card.SetPositionToSelected();
            }

            UpdateInteractionUI();
        }

        private void PlayButtonPressed()
        {
            _assignedPlayer.Play(_cardsToPlay);
        }

        private void PassButtonPressed()
        {
            _assignedPlayer.Pass();
        }

        private void UpdateInteractionUI()
        {
            bool cardsToPlayValid = _currentGameInfo.ValidateCards(_cardsToPlay);

            SetPlayButtonInteractability(cardsToPlayValid);
        }

        private void SetLocalInteractionUIAsOnActionStart()
        {
            foreach (CardEntity card in _cardsOnHand)
            {
                card.SetInteractableWithColour(true);
            }

            SetLocalInteractionButtonsVisible(true);

            SetPlayButtonInteractability(false);

            SetPassButtonInteractability(true);

            UpdateInteractionUI();
        }

        private void SetLocalInteractionUIAsOnActionEnd()
        {
            foreach (CardEntity card in _cardsOnHand)
            {
                card.SetInteractableWithColour(false);
            }

            SetPlayButtonInteractability(false);

            SetPassButtonInteractability(false);

            SetLocalInteractionButtonsVisible(false);
        }

        private void SetLocalInteractionButtonsVisible(bool shouldBeVisible)
        {
            _buttonPlay.gameObject.SetActive(shouldBeVisible);
            _buttonPass.gameObject.SetActive(shouldBeVisible);
        }

        private void SetPlayButtonInteractability(bool shouldBeInteractable)
        {
            _buttonPlay.interactable = shouldBeInteractable;
        }

        private void SetPassButtonInteractability(bool shouldBeInteractable)
        {
            _buttonPass.interactable = shouldBeInteractable;
        }
    }
}