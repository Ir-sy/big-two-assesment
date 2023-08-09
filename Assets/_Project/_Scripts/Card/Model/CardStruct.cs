namespace BigTwo.Card
{
    public struct CardStruct
    {
        public CardSuitEnum Suit { get; private set; }
        public CardValueEnum Value { get; private set; }

        public CardStruct(CardSuitEnum suit, CardValueEnum value)
        {
            Suit = suit;
            Value = value;
        }

        public static bool operator >(CardStruct lhs, CardStruct rhs)
        {
            if (lhs.Suit > rhs.Suit)
            {
                return true;
            }

            if (lhs.Suit < rhs.Suit)
            {
                return false;
            }

            return lhs.Value > rhs.Value;
        }

        public static bool operator <(CardStruct lhs, CardStruct rhs)
        {
            if (lhs.Suit < rhs.Suit)
            {
                return true;
            }

            if (lhs.Suit > rhs.Suit)
            {
                return false;
            }

            return lhs.Value < rhs.Value;
        }
    }
}