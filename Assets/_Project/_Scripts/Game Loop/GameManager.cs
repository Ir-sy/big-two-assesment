using BigTwo.Card;
using BigTwo.Character;
using BigTwo.Player;
using BigTwo.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigTwo.GameLoop
{
    public enum GameFlowEnum
    {
        None = 0,
        CharacterSelection = 1,
        Gameplay = 2,
        GameFinish = 3,
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Essentials")]
        [SerializeField]
        private CharacterModelCollectionSO _characterCollection;
        [SerializeField]
        private CardCollectionModel _cardCollections;
        [SerializeField]
        private GameState _gameState;

        [Header("Player Settings")]
        [SerializeField, Range(2,4)]
        private int _numberOfPlayer = 2;
        [SerializeField]
        private PlayerSeat[] _playerSeats;

        [Header("Character Selection")]
        [SerializeField]
        private Canvas _characterSelectionCanvas;
        [SerializeField]
        private CharacterSelectionUI _characterSelectionUI;

        [Header("Gameplay")]
        [SerializeField]
        private Canvas _gameplayCanvas;
        [SerializeField]
        private TableTopUI _tableTopUI;

        [Header("Game Over")]
        [SerializeField]
        private Canvas _gameOverCanvas;
        [SerializeField]
        private GameOverUI _gameOverUI;

        private GameFlowEnum _gameFlowEnum;

        private List<PlayerState> _players = new(4);
        private List<CharacterModel> _availableCharacters = new(4);

        private CharacterModel _selectedCharacter = null;

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

            foreach (var characterSO in _characterCollection.Characters)
            {
                _availableCharacters.Add(characterSO.ToCharacterModel());
            }
        }

        private void Start()
        {
            _characterSelectionUI.OnCharacterSelectedEvent += OnCharacterSelected;
            _characterSelectionUI.OnPlayButtonPressedEvent += OnPlayButtonPressed;

            _gameState.OnGameEnd += OnGameEnd;

            SwitchGameFlow(GameFlowEnum.CharacterSelection);
        }

        private void OnDestroy()
        {
            _characterSelectionUI.OnCharacterSelectedEvent -= OnCharacterSelected;
            _characterSelectionUI.OnPlayButtonPressedEvent -= OnPlayButtonPressed;

            _gameState.OnGameEnd -= OnGameEnd;
        }

        private void OnCharacterSelected(CharacterModel character)
        {
            _selectedCharacter = character;
        }

        private void OnPlayButtonPressed()
        {
            SwitchGameFlow(GameFlowEnum.Gameplay);
        }

        private void OnGameEnd()
        {
            SwitchGameFlow(GameFlowEnum.GameFinish);
        }

        private void SwitchGameFlow(GameFlowEnum gameFlow)
        {
            if (_gameFlowEnum == gameFlow)
            {
                Debug.LogWarning($"Game Flow is already {gameFlow}");
                return;
            }

            switch (gameFlow)
            {
                case GameFlowEnum.None:
                    _gameFlowEnum = GameFlowEnum.None;
                    break;

                case GameFlowEnum.CharacterSelection:
                    OnGameFlowSwitchedToCharacterSelection();
                    break;

                case GameFlowEnum.Gameplay:
                    OnGameFlowSwitchedToGameplay();
                    break;

                case GameFlowEnum.GameFinish:
                    OnGameFlowSwitchedToGameFinish();
                    break;
            }
        }

        private void OnGameFlowSwitchedToCharacterSelection()
        {
            _gameFlowEnum = GameFlowEnum.CharacterSelection;

            _selectedCharacter = null;

            _gameplayCanvas.gameObject.SetActive(false);
            _characterSelectionCanvas.gameObject.SetActive(true);

            _characterSelectionUI.InitialiseAndShow(_characterCollection);
        }

        private void OnGameFlowSwitchedToGameplay()
        {
            _characterSelectionCanvas.gameObject.SetActive(false);
            _gameplayCanvas.gameObject.SetActive(true);

            CharacterModel localPlayerSelectedCharacter = 
                _availableCharacters.Find(character => character.Name == _selectedCharacter.Name);

            _availableCharacters.Remove(localPlayerSelectedCharacter);

            for (int i = 0; i < _numberOfPlayer; i++)
            {
                bool isHumanPlayer = i == 0;
                PlayerState playerState = isHumanPlayer ? new HumanPlayerState(i) :
                                                          new BotPlayerState(i);

                CharacterModel character;

                if (isHumanPlayer)
                {
                    character = _selectedCharacter;
                }
                else
                {
                    character = _availableCharacters[0];

                    _availableCharacters.RemoveAt(0);
                }

                playerState.SetCharacter(character);

                _players.Add(playerState);
            }

            for (int i = 0; i < _players.Count; i++)
            {
                _playerSeats[i].Initialise(_gameState, _players[i]);
            }

            _gameState.Initialise(_playerSeats.ToList(), _cardCollections);

            _tableTopUI.Initialise(_gameState, _players);

            _gameState.StartGame();
        }

        private void OnGameFlowSwitchedToGameFinish()
        {
            _gameOverCanvas.gameObject.SetActive(true);

            bool localPlayerWin = _gameState.GameInfo.Winner == _players[0];

            _gameOverUI.Initialise(localPlayerWin);
        }
    }
}