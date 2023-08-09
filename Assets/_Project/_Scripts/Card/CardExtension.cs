using BigTwo.GameLoop;
using System.Collections.Generic;
using System.Linq;

namespace BigTwo.Card
{
    public static class CardExtension
    {
        public static List<CardModel> Sorted(this List<CardModel> unsortedCards)
        {
            return unsortedCards.OrderBy(card => card.Suit)
                                .ThenBy(card => card.Value)
                                .ToList();
        }

        public static List<CardModel> SortedDescending(this List<CardModel> unsortedCards)
        {
            return unsortedCards.OrderByDescending(card => card.Suit)
                                .ThenByDescending(card => card.Value)
                                .ToList();
        }

        public static CardValueEnum GetCardsValue(this List<CardModel> cards)
        {
            if (cards == null || cards.Count <= 0)
            {
                return CardValueEnum.None;
            }

            return cards[0].Value;
        }

        public static CardSuitEnum GetCardsSuit(this List<CardModel> cards)
        {
            if (cards == null || cards.Count <= 0)
            {
                return CardSuitEnum.None;
            }

            return cards[0].Suit;
        }

        public static HandTypeEnum GetCardsHandType(this List<CardModel> cards)
        {
            if (cards == null)
            {
                return HandTypeEnum.None;
            }

            switch (cards.Count)
            {
                case 1:
                    return HandTypeEnum.Single;

                case 2:
                    return ValidAsPairs(HandTypeEnum.Pair, cards);

                case 3:
                    return ValidAsPairs(HandTypeEnum.ThreePair, cards);

                case 4:
                    return ValidAsPairs(HandTypeEnum.FourPair, cards);

                default:
                    return HandTypeEnum.None;
            }
        }

        private static HandTypeEnum ValidAsPairs(HandTypeEnum pairHandType, List<CardModel> cards)
        {
            bool allCardsAreTheSame = cards.All(card => card.Value == cards[0].Value);

            HandTypeEnum handType = allCardsAreTheSame ? pairHandType : HandTypeEnum.None;

            return handType;
        }

        public static CardStruct AsStruct(this CardModel card)
        {
            if (card == null)
            {
                return default;
            }

            return new CardStruct(card.Suit, card.Value);
        }

        public static string Print(this CardModel card)
        {
            if (card == null)
            {
                return string.Empty;
            }

            return $"{card.Suit} {card.Value}";
        }
    }
}