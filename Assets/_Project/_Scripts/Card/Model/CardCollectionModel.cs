using UnityEngine;

namespace BigTwo.Card
{
    [CreateAssetMenu(fileName = "Card Collection", menuName = "Big Two/Card/Card Collection")]
    public class CardCollectionModel : ScriptableObject
    {
        public CardModel[] Cards;
    }
}