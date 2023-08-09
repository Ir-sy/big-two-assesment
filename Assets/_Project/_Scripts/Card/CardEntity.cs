using BigTwo.UserInterface;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BigTwo.Card
{
    public class CardEntity : MonoBehaviour, IPointerClickHandler
    {
        public event Action<CardEntity> OnLifetimeEnded;
        public event Action<CardEntity> OnClicked;

        public CardModel CardModel { get; private set; }

        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _cardImage;
        [SerializeField]
        private Color _nonInteractableColour;
        [SerializeField]
        private Color _nonLocalPlayerColour;

        private Color _defaultColour;
        private Sprite _defaultCardSprite;
        private Vector2 _defaultPosition;

        private void Awake()
        {
            _defaultColour = _cardImage.color;
        }

        public void SetDataAndImage(CardModel cardModel)
        {
            if (cardModel == null)
            {
                Debug.LogWarning("SetDataAndImage : Attempt to assign null card model");
                return;
            }

            SetData(cardModel);

            SetImage(cardModel.CardImage);

            SetImageColourToDefault();
        }

        public void SetData(CardModel cardModel)
        {
            CardModel = cardModel;
        }

        public void SetImage(Sprite sprite)
        {
            _cardImage.sprite = sprite;
        }

        public void SetInteractableWithColour(bool shouldBeInteractable)
        {
            Color targetColour = shouldBeInteractable ? _defaultColour :
                                                        _nonInteractableColour;

            SetImageColour(targetColour);

            SetInteractable(shouldBeInteractable);
        }

        public void SetImageColourToDefault()
        {
            SetImageColour(_defaultColour);
        }

        public void SetImageColourToNonLocalPlayer()
        {
            SetImageColour(_nonLocalPlayerColour);
        }

        private void SetImageColour(Color colour)
        {
            _cardImage.color = colour;
        }

        public void SetInteractable(bool shouldBeInteractable)
        {
            _canvasGroup.interactable = shouldBeInteractable;
        }

        public void SetPositionToSelected()
        {
            _rectTransform.anchoredPosition += Vector2.up * 50f;
        }

        public void SetPositionToDefault()
        {
            _rectTransform.anchoredPosition -= Vector2.up * 50f;
        }

        public void Recycle()
        {
            SetData(null);

            SetImage(_defaultCardSprite);

            SetInteractable(false);

            OnLifetimeEnded?.Invoke(this);
        }

        #region Drag Interface Implementations
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
        #endregion
    }
}