using BigTwo.Character;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo.UserInterface
{
    public class CharacterSelectionUI : MonoBehaviour
	{
        public event Action<CharacterModel> OnCharacterSelectedEvent;
        public event Action OnPlayButtonPressedEvent;

        [Header("Character Items")]
        [SerializeField]
        private CharacterItem _characterItemBlueprint;
        [SerializeField]
        private Transform _characterItemContainer;
        [SerializeField]
        private ToggleGroup _characterItemToggleGroup;

        [Header("Buttons")]
        [SerializeField]
        private Button _playButton;

        private List<CharacterItem> _characterItemsOnScreen = new List<CharacterItem>();
        private Queue<CharacterItem> _characterItemsPool = new Queue<CharacterItem>();

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonPressed);
        }

        private void OnDestroy()
        {
            ClearCharacterItemsOnScreen();

            foreach (CharacterItem item in _characterItemsPool)
            {
                item.OnCharacterSelected -= OnCharacterSelected;
            }

            _playButton.onClick.RemoveListener(OnPlayButtonPressed);
        }

        private void OnPlayButtonPressed()
        {
            OnPlayButtonPressedEvent?.Invoke();
        }

        public void InitialiseAndShow(CharacterModelCollectionSO characterCollection)
		{
            ClearCharacterItemsOnScreen();

            foreach (CharacterModelSO characterModelSO in characterCollection.Characters)
            {
                CharacterModel characterModel = characterModelSO.ToCharacterModel();
                CharacterItem characterItem = GetCharacterItem();

                characterItem.gameObject.SetActive(true);
                characterItem.transform.SetParent(_characterItemContainer);
                characterItem.transform.localScale = Vector3.one;

                characterItem.SetData(characterModel);
                characterItem.SetToggleGroup(_characterItemToggleGroup);

                _characterItemsOnScreen.Add(characterItem);
            }

            if (_characterItemsOnScreen.Count > 0)
            {
                _characterItemsOnScreen[0].SetToggleActive(true);
            }
		}

        private void OnCharacterSelected(CharacterModel characterModel)
            => OnCharacterSelectedEvent?.Invoke(characterModel);

        private void ClearCharacterItemsOnScreen()
        {
            foreach (CharacterItem characterItem in _characterItemsOnScreen)
            {
                characterItem.Recycle();
                characterItem.gameObject.SetActive(false);

                _characterItemsPool.Enqueue(characterItem);
            }

            _characterItemsOnScreen.Clear();
        }

        private CharacterItem GetCharacterItem()
        {
            if (_characterItemsPool.Count > 0)
            {
                return _characterItemsPool.Dequeue();
            }

            CharacterItem newCharacterItem = Instantiate(_characterItemBlueprint);
            newCharacterItem.OnCharacterSelected += OnCharacterSelected;

            return newCharacterItem;
        }
	}
}