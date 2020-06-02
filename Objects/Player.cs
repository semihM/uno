namespace Objects
{
    public class Player
    {
        public string name;

        public Card[] Hand = new Card[Card.N_CARDS];
        
        public Player(string Name)
        {
            name = Name;
        }

        public static Card UseCard(Card[] hand,int i)
        {
            Card temp = Card.CreateCard(hand[i].Val,hand[i].Color,hand[i].Type);

            for(int j = i; j < Card.GetLen(hand)-1; j++)
            {
                hand[j] = Card.CreateCard(hand[j+1].Val, hand[j+1].Color, hand[j+1].Type);
            }
            hand[Card.GetLen(hand) - 1] = null;
            return temp;
        }

        public static void AddToHand(Player player,Card card)
        {
            int handlen = Card.GetLen(player.Hand);
            player.Hand[handlen] = card;

        }

        public static void PrintHand(Player player)
        {
            Card.PrintDeck(player.Hand);
        }
    }
}
