using BigTwo.Character;
using BigTwo.GameLoop;
using BigTwo.Player;
using UnityEngine;

namespace BigTwo.UserInterface
{
    public class PlayerSeat : MonoBehaviour
    {
        public PlayerState Player { get; private set; }

        public bool IsSeatActive => Player != null;

        [SerializeField]
        private int _turnIndex;
        [SerializeField]
        private PlayerUI _playerUI;
        [SerializeField]
        private CharacterUI _characterUI;

        private GameState _gameState;

        public void Initialise(GameState gameState, PlayerState player)
        {
            Player = player;
            Player.Initialise(_turnIndex);

            _playerUI.Initialise(player);

            _characterUI.Initialise(gameState, player);
        }

        public void SetToDefault()
        {
            Player = null;

            _playerUI.SetToDefault();
        }
    }
}