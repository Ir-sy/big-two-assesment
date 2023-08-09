using BigTwo.Card;
using BigTwo.System;
using System.Collections.Generic;

namespace BigTwo.GameLoop
{
    public static class GameInfoExtension
    {
        public static bool ValidateCards(this GameInfo gameInfo, List<CardModel> cards)
        {
            if (gameInfo == null)
            {
                return false;
            }

            if (cards == null)
            {
                return false;
            }

            if (gameInfo.IsForcingLowestCard)
            {
                HandTypeEnum handType = cards.GetCardsHandType();
                CardValueEnum value = cards.GetCardsValue();
                CardSuitEnum suit = cards.GetCardsSuit();

                return handType == HandTypeEnum.Single && 
                       value == CardValueEnum.Three && 
                       suit == CardSuitEnum.Diamond;
            }
            else
            {
                bool handTypeValid = ValidateCardsHandType(gameInfo, cards);
                bool valueValid = ValidateCardsValue(gameInfo, cards);

                return handTypeValid && valueValid;
            }
        }

        public static bool ValidateCardsHandType(this GameInfo gameInfo, List<CardModel> cards)
        {
            if (gameInfo == null)
            {
                return false;
            }

            if (cards == null)
            {
                return false;
            }

            HandTypeEnum cardsHandType = cards.GetCardsHandType();

            if (gameInfo.CurrentHandType == HandTypeEnum.None)
            {
                return cardsHandType != HandTypeEnum.None;
            }
            else
            {
                return gameInfo.CurrentHandType == cardsHandType;
            }
        }

        public static bool ValidateCardsValue(this GameInfo gameInfo, List<CardModel> cards)
        {
            CardValueEnum cardsValue = cards.GetCardsValue();

            return cardsValue > gameInfo.CurrentHighestValue;
        }
    }
}