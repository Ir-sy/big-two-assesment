using BigTwo.Card;
using BigTwo.Character;
using BigTwo.GameLoop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BigTwo.Player
{
    public abstract class PlayerState
    {
        public event Action<PlayerState, List<CardModel>> OnInitialCardsDealt;
        public event Action<PlayerState, GameInfo> OnPlayerAction;
        public event Action<PlayerState, List<CardModel>> OnPlayerPlays;
        public event Action<PlayerState> OnPlayerPasses;

        public int PlayerIndex { get; protected set; }
        public int TurnIndex { get; protected set; }
        public CharacterModel Character { get; protected set; }

        public bool IsActiveInGame { get; protected set; }
        public bool IsActiveInRound { get; protected set; }

        public List<CardModel> Hand { get; protected set; } = new();

        public PlayerState(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public void Initialise(int turnIndex)
        {
            TurnIndex = turnIndex;
        }

        public void SetCharacter(CharacterModel character)
        {
            Character = character;
        }

        public void SetInitialCards(List<CardModel> cards)
        {
            Hand = new(cards);
            Hand = Hand.Sorted();

            InvokeOnInitialCardsDealt(Hand);
        }

        public void StartGame()
        {
            IsActiveInGame = true;
        }

        public void StartRound()
        {
            IsActiveInRound = true;
        }

        public void StartTurn() { }

        public abstract void Action(GameInfo gameInfo);

        protected void ForcePlayLowestCard()
            => Play(new List<CardModel>() { Hand[0] });

        public void Play(List<CardModel> cards)
        {
            foreach (CardModel card in cards)
            {
                Hand.Remove(card);
            }

            InvokeOnPlayerPlays(cards);
        }

        public void Pass()
        {
            IsActiveInRound = false;

            InvokeOnPlayerPass();
        }

        public CardModel GetLowestCard()
        {
            if (Hand.Count <= 0)
            {
                return null;
            }

            return Hand[0];
        }

        protected void InvokeOnInitialCardsDealt(List<CardModel> cards)
        {
            OnInitialCardsDealt?.Invoke(this, cards);
        }

        protected void InvokeOnPlayerAction(GameInfo gameInfo)
        {
            OnPlayerAction?.Invoke(this, gameInfo);
        }

        protected void InvokeOnPlayerPlays(List<CardModel> cards)
        {
            OnPlayerPlays?.Invoke(this, cards);
        }

        protected void InvokeOnPlayerPass()
        {
            OnPlayerPasses?.Invoke(this);
        }
    }
}