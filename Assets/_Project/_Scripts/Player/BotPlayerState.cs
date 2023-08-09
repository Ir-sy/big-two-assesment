using BigTwo.Card;
using BigTwo.GameLoop;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigTwo.Player
{
    public class BotPlayerState : PlayerState
    {
        public BotPlayerState(int playerIndex) : base(playerIndex)
        {
        }

        public override void Action(GameInfo gameInfo)
        {
            if (gameInfo.IsForcingLowestCard)
            {
                ForcePlayLowestCard();
            }
            else
            {
                List<CardModel> cardsToPlay = GetCardsToPlay(gameInfo);

                if (cardsToPlay.Count > 0)
                {
                    Play(cardsToPlay);
                }
                else
                {
                    Pass();
                }
            }
        }

        private List<CardModel> GetCardsToPlay(GameInfo gameInfo)
        {
            if (gameInfo.CurrentHandType == HandTypeEnum.None)
            {
                List<CardModel> pairs = TryGetHighestNonLimitingPairsWithLowestValue();

                if (pairs.Count > 0)
                {
                    return pairs;
                }

                return new List<CardModel>(1) { Hand[0] };
            }
            else if (gameInfo.CurrentHandType == HandTypeEnum.Single)
            {
                List<CardModel> potentialCards = Hand.FindAll(card => card.Value > gameInfo.CurrentHighestValue);
                List<CardModel> cardsToPlay = new List<CardModel>(1);

                if (potentialCards.Count > 0)
                {
                    cardsToPlay.Add(potentialCards[0]);
                }

                return cardsToPlay;
            }
            else
            {
                return GetLimitingPairs(gameInfo.CurrentHandType, gameInfo.CurrentHighestValue);
            }
        }

        private List<CardModel> TryGetHighestNonLimitingPairsWithLowestValue()
        {
            HandTypeEnum handTypeEnum = HandTypeEnum.FourPair;

            int tryCount = 0;
            int maxTryCount = 99;

            while (handTypeEnum > HandTypeEnum.Single)
            {
                if (tryCount >= maxTryCount)
                {
                    Debug.Log($"Bailing out from infinite loop");
                    break;
                }

                List<CardModel> supposedPairs = GetNonLimitingPairs(handTypeEnum);

                if (supposedPairs.Count > 0)
                {
                    return supposedPairs;
                }

                handTypeEnum--;

                tryCount++;
            }

            return new List<CardModel>(0);
        }

        private List<CardModel> GetNonLimitingPairs(HandTypeEnum handType)
        {
            var pairs = Hand.GroupBy(card => card.Value)
                            .Where(group => group.Count() == (int)handType)
                            .ToList();

            return GetLowestValuePairs(pairs);
        }

        private List<CardModel> GetLimitingPairs(HandTypeEnum handType, CardValueEnum minimumValue)
        {
            var pairs = Hand.Where(card => card.Value > minimumValue)
                            .GroupBy(card => card.Value)
                            .Where(group => group.Count() == (int)handType)
                            .ToList();

            return GetLowestValuePairs(pairs);
        }

        private List<CardModel> GetLowestValuePairs(List<IGrouping<CardValueEnum, CardModel>> pairs)
        {
            if (pairs.Count <= 0)
            {
                return new List<CardModel>();
            }

            pairs = pairs.OrderBy(pair => pair.Key).ToList();

            CardValueEnum lowestPairKey = pairs[0].Key;

            return Hand.Where(card => card.Value == lowestPairKey).ToList();
        }
    }
}