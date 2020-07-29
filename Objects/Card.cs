using System;

namespace Objects
{   
    abstract public class Card
    {
        public string Type;

        public dynamic Val;

        public char Color { get; set; }
        
        public static int N_CARDS = 108;

        public static Card[] CreateDeck()
        {
            char[] colors = { 'R', 'G', 'B', 'Y' };
            string[] specs = { "Reverse", "Block", "Draw2" };
            char rowcolor;
            int c, rownum;

            // CLASSICAL 108 CARDS OF UNO
            Card[] Deck = new Card[Card.N_CARDS];

            // FIRST 4 SETS WITH ZEROS AND WILD CARD
            for (int row = 0; row < 4; row++)
            {
                rownum = row * 14;
                rowcolor = colors[row % 4];
                for (int col = 0; col < 10; col++)
                {
                    Deck[rownum + col] = new NumCard(col, rowcolor);
                }

                c = 10;
                foreach (string s in specs)
                {
                    Deck[rownum + c] = new ColorActCard(s, rowcolor);
                    c++;
                }

                Deck[rownum + 13] = new NoColorActCard("Wild");
            }

            // LAST 4 SETS WITHOUT ZEROS, WITH WILD DRAW 4'S
            for (int row = 4; row < 8; row++)
            {
                rownum = 56 + 13 * (row % 4);
                rowcolor = colors[row % 4];
                for (int col = 1; col < 10; col++)
                {
                    Deck[rownum + (col - 1)] = new NumCard(col, rowcolor);
                }

                c = 9;
                foreach (string s in specs)
                {
                    Deck[rownum + c] = new ColorActCard(s, rowcolor);
                    c++;
                }

                Deck[rownum + 12] = new NoColorActCard("Draw4");

            }
            return Deck;
        }
        public static Card DrawCard(Card[] deck)
        {
            int decklen = deck.Length;
            int i = 1;

            while (deck[decklen - i] == null)
            {
                i++;
            }
            
            if (decklen-i == -1)
            {
                throw new Exception("No cards left");
            }
            else
            {
                Card topCard = deck[decklen - i];
                deck[decklen - i] = null;
                return topCard;
            }

        }

        public static void Shuffle(Card[] deck)
        {
            var rng = new Random();
            int n = Card.N_CARDS;
            while (n > 1)
            {
                int k = rng.Next(n--);
                Card temp = Card.CopyCard(deck[n]);
                deck[n] = Card.CopyCard(deck[k]);
                deck[k] = temp;
            }
        }
        
        public static bool Same(Card card1,Card card2)
        {
            if (card1.Type == card2.Type)
            {   
                return ((card1.Color == card2.Color) & (card1.Val == card2.Val));
            }
            else 
            { 
                return false;
            }
        }
        public static int Index(Card[] deck,Card card)
        {
            for (int j = 0; j < Card.GetLen(deck); j++)
            {
                if (Card.Same(deck[j], card))
                {
                    return j;
                }

            }
            return -1;
        }
        public static int GetLen(Card[] deck)
        {
            int decklen = deck.Length;
            int i = 1;
            if (decklen == 0)
            {
                return 0;
            }
            while (deck[decklen - i] == null)
            {   
                if (i == decklen)
                {
                    return 0;
                }
                i++;
            }

            return decklen - i + 1;

        }


        public static void PrintDeck(Card[] Deck)
        {   

            for (int i = 0; i < Card.GetLen(Deck); i++)
            {
                if (Deck[i] == null)
                {
                    break;
                }


                Console.WriteLine($"{i+1}: [{Deck[i].Color} : {Deck[i].Val}], ");

            }

            Console.WriteLine("");
        }

        public static void PrintCard(Card card)
        {   
            if (card.Type == "NoColorAction")
            {
                Console.WriteLine($"{card.Val}");
            }
            else
            {
                Console.WriteLine($"{card.Color}:{card.Val}");
            }
            
        }
        public static Card GetTop(Card[] deck)
        {
            int len = Card.GetLen(deck);
            if (len == 0)
            {
                return deck[0];
            }

            return deck[len - 1];
        }

        public static Card[] GetUsableCards(Card[] hand, Card middle, char newcolor = 'N')
        {

            string type = middle.Type;
            dynamic val = middle.Val;
            char c;
            int handlen = Card.GetLen(hand);
            Card[] usables = new Card[handlen];

            if (newcolor == 'N')
            {
                c = middle.Color;
            }
            else
            {
                c = newcolor;
            }

            bool midcardstringval = ((middle.Type == "NoColorActCard") | (middle.Type == "ColorActCard"));
            int j = 0;

            for (int i = 0; i < handlen; i++)
            {
                if (hand[i].Type == "NoColorActCard")
                {
                    usables[j] = hand[i];
                    j++;
                }
                else if (hand[i].Type == "ColorActCard")
                {
                    if (hand[i].Color == c)
                    {
                        usables[j] = hand[i];
                        j++;
                    }
                    else if (midcardstringval)
                    {
                        if (hand[i].Val == val)
                        {
                            usables[j] = hand[i];
                            j++;
                        }
                    }
                }
                else if (hand[i].Type == "NumCard")
                {
                    if (hand[i].Color == c)
                    {
                        usables[j] = hand[i];
                        j++;
                    }

                    else if (!midcardstringval)
                    {
                        if (hand[i].Val == val)
                        {
                            usables[j] = hand[i];
                            j++;
                        }
                    }
                }

            }

            return usables;

        }
        public static Card CopyCard(Card old)
        {
            return Card.CreateCard(old.Val, old.Color, old.Type);
        }
        public static Card CreateCard(dynamic val,char color='W',string type="NoColorActCard")
        {
            if (type == "NumCard")
            {
                return new NumCard(val, color);
            }
            else if (type == "ColorActCard")
            {
                return new ColorActCard(val, color);
            }
            else
            {
                return new NoColorActCard(val);
            }
        }

        public static void Nullify(Card[] deck)
        {
            for(int i = 0; i < Card.GetLen(deck); i++)
            {
                deck[i] = null;
            }
        }
    }
    public class NumCard : Card
    {
        public NumCard(int number,char color)
        {
            Val = number;
            Type = "NumCard";
            Color = color;
        }
    }

    public class ColorActCard : Card
    {
        public ColorActCard(string spec, char color)
        {   
            Type = "ColorActCard";
            Val = spec;
            Color = color;
        }
    }

    public class NoColorActCard : Card
    {
        public NoColorActCard(string spec)
        {
            Type = "NoColorActCard";
            Color = 'N';
            Val = spec;
        }
    }
}
