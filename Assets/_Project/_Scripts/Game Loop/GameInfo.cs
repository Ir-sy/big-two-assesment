using BigTwo.Card;
using BigTwo.Player;
using System.Collections.Generic;

namespace BigTwo.GameLoop
{
    public class GameInfo
    {
        public HandTypeEnum CurrentHandType { get; private set; }
        public CardSuitEnum CurrentHighestSuit { get; private set; }
        public CardValueEnum CurrentHighestValue { get; private set; }

        public bool IsForcingLowestCard => GameRoundIndex == 0 && GameTurnIndex == 0;
        public bool HasReachedMaximumValue => CurrentHighestValue == CardValueEnum.Two;

        public int GameRoundIndex { get; private set; }
        public int GameTurnIndex { get; private set; }

        public List<PlayerState> Players { get; private set; }
        public int PlayerActiveInRoundCount { get; private set; }

        public PlayerState PlayerInTurn { get; private set; }
        public int WinningPlayerHandCount { get; private set; }
        public PlayerState Winner { get; private set; }

        public void StartGame(List<PlayerState> players)
        {
            GameRoundIndex = 0;
            GameTurnIndex = 0;

            Players = players;
        }

        public void EndGame() { }

        public void StartRound()
        {
            CurrentHandType = HandTypeEnum.None;
            CurrentHighestSuit = CardSuitEnum.None;
            CurrentHighestValue = CardValueEnum.None;

            PlayerActiveInRoundCount = Players.FindAll(player =>
                                                       player.IsActiveInRound == true &&
                                                       player.IsActiveInGame == true).Count;
        }

        public void EndRound()
        {
            GameRoundIndex++;

            GameTurnIndex = 0;
        }

        public void StartTurn() { }

        public void EndTurn()
        {
            GameTurnIndex++;

            int currentSmallestHand = int.MaxValue;

            foreach (PlayerState player in Players)
            {
                int playerHandCount = player.Hand.Count;

                if (playerHandCount < currentSmallestHand)
                {
                    currentSmallestHand = playerHandCount;
                }
            }

            WinningPlayerHandCount = currentSmallestHand;
        }

        public void PlayerPlays(List<CardModel> cards)
        {
            if (CurrentHandType == HandTypeEnum.None)
            {
                CurrentHandType = cards.GetCardsHandType();
            }

            foreach (CardModel card in cards)
            {
                if (card.Value > CurrentHighestValue)
                {
                    CurrentHighestValue = card.Value;
                }
            }
        }

        public void OnPlayerPasses()
        {
            PlayerActiveInRoundCount--;
        }

        public void SetPlayerInTurn(PlayerState player)
        {
            PlayerInTurn = player;
        }

        public void SetWinner(PlayerState player)
        {
            Winner = player;
        }
    }
}