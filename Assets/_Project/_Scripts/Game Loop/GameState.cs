using BigTwo.Card;
using BigTwo.Player;
using BigTwo.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace BigTwo.GameLoop
{
    public enum GameStateEnum
    {
        None,
        GameStart,
        RoundStart,
        TurnStart,
        PlayerAction,
        TurnEnd,
        RoundEnd,
        GameEnd,
    }

    public class GameState : MonoBehaviour
    {
        public const int INITIAL_NUMBER_OF_CARDS_TO_DEAL = 13;

        public event Action OnGameStart;
        public event Action OnRoundStart;
        public event Action<PlayerState> OnTurnStart;
        public event Action<PlayerState> OnPlayerDoAction;
        public event Action<PlayerState> OnTurnEnd;
        public event Action OnRoundEnd;
        public event Action OnGameEnd;

        [SerializeField]
        private float _startingTurnDelay = .25f;
        [SerializeField]
        private float _endingRoundDelay = 1.5f;

        public GameInfo GameInfo { get; private set; }

        public IReadOnlyList<PlayerState> Players => _players;

        private List<PlayerState> _players = new();

        private CardCollectionModel _cardCollections;

        private GameStateEnum _currentGameState;

        private WaitForSeconds _waitOnTurnStart;
        private WaitForSeconds _waitBeforeEndRound;

        public void Initialise(List<PlayerSeat> playerSeats, CardCollectionModel cardCollection)
        {
            GameInfo = new GameInfo();
            _waitOnTurnStart = new WaitForSeconds(_startingTurnDelay);
            _waitBeforeEndRound = new WaitForSeconds(_endingRoundDelay);

            _players ??= new List<PlayerState>();
            _players.Clear();

            foreach (PlayerSeat seat in playerSeats)
            {
                if (seat == null || seat.Player == null)
                {
                    continue;
                }

                _players.Add(seat.Player);
            }

            _players = _players.OrderBy(player => player.TurnIndex).ToList();

            _cardCollections = cardCollection;
        }

        private void SwitchGameState(GameStateEnum gameState)
        {
            if (_currentGameState == gameState)
            {
                Debug.LogWarning($"SwitchToGameState : It's already in {gameState} state");
                return;
            }

            switch (gameState)
            {
                case GameStateEnum.GameStart:
                    OnGameStateSwitchedToGameStart();
                    break;

                case GameStateEnum.RoundStart:
                    OnGameStateSwitchedToRoundStart();
                    break;

                case GameStateEnum.TurnStart:
                    OnGameStateSwitchedToTurnStart();
                    break;

                case GameStateEnum.PlayerAction:
                    OnGameStateSwitchedToPlayerAction();
                    break;

                case GameStateEnum.TurnEnd:
                    OnGameStateSwitchedToTurnEnd();
                    break;

                case GameStateEnum.RoundEnd:
                    OnGameStateSwitchedToRoundEnd();
                    break;

                case GameStateEnum.GameEnd:
                    OnGameStateSwitchedToGameEnd();
                    break;
            }
        }

        public void StartGame() 
            => SwitchGameState(GameStateEnum.GameStart);

        private void OnGameStateSwitchedToGameStart()
        {
            _currentGameState = GameStateEnum.GameStart;

            BindToPlayerActions();

            foreach (PlayerState player in _players)
            {
                if (player == null)
                {
                    Debug.LogWarning("GameLoop : Found null ref player");
                    continue;
                }

                player.StartGame();
            }

            GameInfo.StartGame(_players);

            DealInitialCards();

            PlayerState playerInTurn = GetPlayerWithLowestCard();

            GameInfo.SetPlayerInTurn(playerInTurn);

            OnGameStart?.Invoke();

            SwitchGameState(GameStateEnum.RoundStart);
        }

        private void OnGameStateSwitchedToRoundStart()
        {
            _currentGameState = GameStateEnum.RoundStart;

            foreach (PlayerState player in _players)
            {
                player.StartRound();
            }

            GameInfo.StartRound();

            OnRoundStart?.Invoke();

            SwitchGameState(GameStateEnum.TurnStart);
        }

        private void OnGameStateSwitchedToTurnStart()
            => StartCoroutine(OnTurnStartCoroutine());

        private IEnumerator OnTurnStartCoroutine()
        {
            _currentGameState = GameStateEnum.TurnStart;

            GameInfo.StartTurn();

            PlayerState playerInTurn = GameInfo.PlayerInTurn;
            playerInTurn.StartTurn();

            OnTurnStart?.Invoke(playerInTurn);

            yield return _waitOnTurnStart;

            SwitchGameState(GameStateEnum.PlayerAction);
        }

        private void OnGameStateSwitchedToPlayerAction()
        {
            _currentGameState = GameStateEnum.PlayerAction;

            PlayerState playerInTurn = GameInfo.PlayerInTurn;
            playerInTurn.Action(GameInfo);
        }

        private void OnPlayerPlays(PlayerState player, List<CardModel> cards)
        {
            GameInfo.PlayerPlays(cards);

            OnPlayerDoAction?.Invoke(player);

            SwitchGameState(GameStateEnum.TurnEnd);
        }

        private void OnPlayerPasses(PlayerState player)
        {
            GameInfo.OnPlayerPasses();

            OnPlayerDoAction?.Invoke(player);

            SwitchGameState(GameStateEnum.TurnEnd);
        }

        private void OnGameStateSwitchedToTurnEnd()
        {
            _currentGameState = GameStateEnum.TurnEnd;

            GameInfo.EndTurn();

            PlayerState playerInTurn = GameInfo.PlayerInTurn;

            bool playerInTurnWinsTheGame = IsWinner(playerInTurn);

            if (playerInTurnWinsTheGame)
            {
                GameInfo.SetWinner(playerInTurn);

                OnTurnEnd?.Invoke(playerInTurn);

                SwitchGameState(GameStateEnum.GameEnd);

                return;
            }

            int playerActiveInRound = GameInfo.PlayerActiveInRoundCount;

            bool playerStillCompeting = playerActiveInRound > 1;
            bool hasReachedMaximumValueInRound = GameInfo.HasReachedMaximumValue;
            bool roundStillActive = playerStillCompeting && !hasReachedMaximumValueInRound;

            PlayerState nextPlayerInTurn = playerInTurn;

            if (playerStillCompeting && !hasReachedMaximumValueInRound)
            {
                nextPlayerInTurn = GetNextTurnPlayer(playerInTurn.TurnIndex);
            }
            else if (!playerStillCompeting)
            {
                nextPlayerInTurn = _players.Find(player => player.IsActiveInRound);
            }

            GameInfo.SetPlayerInTurn(nextPlayerInTurn);

            GameStateEnum nextGameState = roundStillActive ? GameStateEnum.TurnStart : GameStateEnum.RoundEnd;

            OnTurnEnd?.Invoke(playerInTurn);

            SwitchGameState(nextGameState);
        }

        private void OnGameStateSwitchedToRoundEnd()
            => StartCoroutine(OnGameStateSwitchedToRoundEndCoroutine());

        private IEnumerator OnGameStateSwitchedToRoundEndCoroutine()
        {
            yield return _waitBeforeEndRound;

            _currentGameState = GameStateEnum.RoundEnd;

            GameInfo.EndRound();

            OnRoundEnd?.Invoke();

            if (GameInfo.Winner == null)
            {
                SwitchGameState(GameStateEnum.RoundStart);
            }
            else
            {
                SwitchGameState(GameStateEnum.GameEnd);
            }
        }

        private void OnGameStateSwitchedToGameEnd()
        {
            PlayerState winner = GameInfo.Winner;
            string winnerString = winner != null ? winner.TurnIndex.ToString() : "NONE";

            _currentGameState = GameStateEnum.GameEnd;

            GameInfo.EndGame();

            UnbindFromPlayerActions();

            OnGameEnd?.Invoke();
        }

        private void BindToPlayerActions()
        {
            if (_players == null)
            {
                Debug.LogWarning("ListenToPlayerActions : Player list is null");
                return;
            }

            foreach (PlayerState player in _players)
            {
                if (player == null)
                {
                    Debug.LogWarning("ListenToPlayerActions : Null Ref player detected");
                    continue;
                }

                player.OnPlayerPlays += OnPlayerPlays;
                player.OnPlayerPasses += OnPlayerPasses;
            }
        }

        private void UnbindFromPlayerActions()
        {
            if (_players == null)
            {
                Debug.LogWarning("UnlistenFromPlayerActions : Player list is null");
                return;
            }

            foreach (PlayerState player in _players)
            {
                if (player == null)
                {
                    Debug.LogWarning("UnlistenFromPlayerActions : Null Ref player detected");
                    continue;
                }

                player.OnPlayerPlays -= OnPlayerPlays;
                player.OnPlayerPasses -= OnPlayerPasses;
            }
        }

        private void DealInitialCards()
        {
            if (_cardCollections == null)
            {
                Debug.LogWarning("DealToPlayers : Card collections ref is null");
                return;
            }

            List<CardModel> deck = new(_cardCollections.Cards);
            List<CardModel> cardsToDeal = new(INITIAL_NUMBER_OF_CARDS_TO_DEAL);

            for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
            {
                cardsToDeal.Clear();

                for (int card = 0; card < INITIAL_NUMBER_OF_CARDS_TO_DEAL; card++)
                {
                    if (deck.Count <= 0)
                    {
                        continue;
                    }

                    int selectedCardIndex = deck.Count == 1 ? 0 : URandom.Range(0, deck.Count);

                    CardModel selectedCard = deck[selectedCardIndex];

                    cardsToDeal.Add(selectedCard);
                    deck.RemoveAt(selectedCardIndex);
                }

                _players[playerIndex].SetInitialCards(cardsToDeal);
            }
        }

        private PlayerState GetNextTurnPlayer(int currentTurnIndex)
        {
            int nextTurnIndex = (currentTurnIndex + 1) % _players.Count;

            int loopTryCount = 100;
            int loopCount = 1;

            while (nextTurnIndex != currentTurnIndex)
            {
                if (loopCount > loopTryCount)
                {
                    Debug.LogError($"Break out infinite Loop");
                    break;
                }

                PlayerState supposedPlayerInTurn = _players[nextTurnIndex];

                if (supposedPlayerInTurn.IsActiveInRound)
                {
                    return supposedPlayerInTurn;
                }

                nextTurnIndex = (nextTurnIndex + 1) % _players.Count;

                loopTryCount++;
            }

            return null;
        }

        private PlayerState GetPlayerWithLowestCard()
        {
            CardStruct currentLowestCard = new(CardSuitEnum.Spade, CardValueEnum.Two);

            PlayerState playerWithLowestCard = _players[0];

            foreach (PlayerState player in _players)
            {
                if (player == null)
                {
                    continue;
                }

                CardStruct playerLowestCard = player.GetLowestCard().AsStruct();

                if (playerLowestCard < currentLowestCard)
                {
                    playerWithLowestCard = player;
                    currentLowestCard = playerLowestCard;
                }
            }

            return playerWithLowestCard;
        }

        private bool IsWinner(PlayerState player)
        {
            if (player == null)
            {
                Debug.LogWarning("IsWinner : Player is null");
                return false;
            }

            return player.Hand.Count <= 0;
        }
    }
}