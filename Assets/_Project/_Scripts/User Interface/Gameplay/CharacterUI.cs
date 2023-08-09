using BigTwo.Character;
using BigTwo.GameLoop;
using BigTwo.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo.UserInterface
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textPlayerName;
        [SerializeField]
        private Image _imagePlayer;
        [SerializeField]
        private GameObject _statusPanel;
        [SerializeField]
        private Image _statusPanelImage;
        [SerializeField]
        private TextMeshProUGUI _statusText;
        [SerializeField]
        private Color _inTurnStatusColour;
        [SerializeField]
        private Color _passStatusColour;

        private PlayerState _assignedPlayer;
        private CharacterModel _characterModel;

        private GameState _gameState;
        private bool _hasBindedGameStateEvents;

        public void Initialise(GameState gameState, PlayerState player)
        {
            _gameState = gameState;

            if (!_hasBindedGameStateEvents)
            {
                _gameState.OnRoundEnd += OnRoundEnd;
                _gameState.OnTurnStart += OnTurnStart;
                _gameState.OnTurnEnd += OnTurnEnd;

                _hasBindedGameStateEvents = true;
            }

            SetAssignedPlayerAndCharacter(player);
        }

        private void OnDestroy()
        {
            if (_hasBindedGameStateEvents)
            {
                _gameState.OnRoundEnd -= OnRoundEnd;
                _gameState.OnTurnStart -= OnTurnStart;
                _gameState.OnTurnEnd -= OnTurnEnd;
            }
        }

        private void SetAssignedPlayerAndCharacter(PlayerState player) 
        {
            _assignedPlayer = player;

            _characterModel = _assignedPlayer.Character;

            if (_characterModel != null)
            {
                SetPlayerImageAndTint(_characterModel.HappySprite, _characterModel.TintColour);

                SetPlayerName(_characterModel.Name);
            }
        }

        private void OnTurnStart(PlayerState playerState)
        {
            if (playerState != _assignedPlayer)
            {
                return;
            }

            ShowInTurnStatusPanel();
        }

        private void OnTurnEnd(PlayerState playerState)
        {
            if (playerState == _assignedPlayer)
            {
                if (!playerState.IsActiveInRound)
                {
                    ShowPassStatusPanel();
                }
                else
                {
                    HideStatusPanel();
                }
            }

            if (_characterModel != null)
            {
                int winningPlayerHandCount = _gameState.GameInfo.WinningPlayerHandCount;
                int assignedPlayerHandCount = _assignedPlayer.Hand.Count;

                bool otherPlayerWinning = winningPlayerHandCount != assignedPlayerHandCount;

                Sprite characterSprite = otherPlayerWinning ? _characterModel.AngrySprite : _characterModel.HappySprite;

                SetPlayerImage(characterSprite);
            }
        }

        private void OnRoundEnd()
        {
            HideStatusPanel();
        }

        private void ShowPassStatusPanel()
        {
            ShowStatusPanel("Passed");

            SetStatusPanelColour(_passStatusColour);
        }

        private void ShowInTurnStatusPanel()
        {
            ShowStatusPanel("In Turn");

            SetStatusPanelColour(_inTurnStatusColour);
        }

        private void ShowStatusPanel(string statusText)
        {
            _statusPanel.SetActive(true);

            _statusText.SetText(statusText);
        }

        private void HideStatusPanel()
        {
            _statusPanel.SetActive(false);
        }

        private void SetStatusPanelColour(Color colour)
        {
            _statusPanelImage.color = colour;
        }

        private void SetPlayerImageAndTint(Sprite sprite, Color tintColour)
        {
            _imagePlayer.color = tintColour;

            SetPlayerImage(sprite);
        }

        private void SetPlayerImage(Sprite sprite)
        {
            _imagePlayer.sprite = sprite;
        }

        private void SetPlayerName(string name)
        {
            _textPlayerName.SetText(name);
        }
    }
}