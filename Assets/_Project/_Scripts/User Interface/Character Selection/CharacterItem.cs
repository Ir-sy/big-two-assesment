using BigTwo.Character;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo.UserInterface
{
    public class CharacterItem : MonoBehaviour
    {
        public event Action<CharacterModel> OnCharacterSelected;

        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private Image _backgroundImage;
        [SerializeField]
        private Image _characterImage;
        [SerializeField]
        private TextMeshProUGUI _characterNameText;

        private CharacterModel _character;

        private void Awake()
        {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool value)
            => OnSelectedValueChanged(value);

        private void OnSelectedValueChanged(bool isSelected)
        {
            Color backgroundColour = isSelected ? _character.TintColour : Color.white;

            _backgroundImage.color = backgroundColour;

            if (isSelected)
            {
                OnCharacterSelected?.Invoke(_character);
            }
        }

        public void SetToggleActive(bool shouldToggleActive)
        {
            _toggle.isOn = shouldToggleActive;
        }

        public void SetToggleGroup(ToggleGroup toggleGroup)
        {
            _toggle.group = toggleGroup;
        }

        public void SetData(CharacterModel character)
        {
            _character = character;

            _characterImage.sprite = _character != null ? character.HappySprite : null;
            _characterImage.color = _character != null ? character.TintColour : Color.white;

            _characterNameText.SetText(_character?.Name ?? string.Empty);
        }

        public void Recycle()
        {
            SetToggleGroup(null);

            SetData(null);
        }
    }
}