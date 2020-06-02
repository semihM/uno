using System;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Objects
{
   
    class game
    {
        static void Main(string[] args)
        {
            bool quit = false;

            while (!quit)
            {
                // Initialize
                Console.WriteLine("How many players are there? ");
                int N_Players = Int32.Parse(Console.ReadLine());
                Console.WriteLine("How many cards will each play start with? ");
                int N_StartingCards = Int32.Parse(Console.ReadLine());

                if (N_Players * N_StartingCards >= 107)
                {
                    throw new Exception($"Can't create {N_Players} players with {N_StartingCards} cards.");
                }

                // Create and shuffle the deck
                Card[] Deck = Card.CreateDeck();
                Card.Shuffle(Deck);
                Console.WriteLine("\nCreated and shuffled the deck of cards.\n");

                // Create players
                Player[] Players = new Player[N_Players];

                for (int i = 0; i < N_Players; i++)
                {
                    Console.WriteLine($"Enter a name for player {i}");
                    Players[i] = new Player(Console.ReadLine());

                    for (int j = 0; j < N_StartingCards; j++)
                    {
                        Player.AddToHand(Players[i], Card.DrawCard(Deck));
                    }

                    Console.WriteLine($"Player {Players[i].name} created");
                }

                // Start the game
                Console.WriteLine($"\nGame starts with {Players.Length} players. {Players[0].name} starts.");
                
                string winnername = "";
                int turn = 0;
                int middlelength,chosencardindex,Availablelength;
                bool chosecard = false;
                bool running = true;
                bool validcolor;
                char pickedcolorwild='N';
                char[] colors = { 'R', 'G', 'B', 'Y' };
                int orderindexer = 1;
                
                Player currentPlayer = Players[0];
                Card chosenCard;
                Card drawnCard;
                Card[] Middle = new Card[Card.N_CARDS];
                Card[] Available = new Card[Card.N_CARDS];

                /////////////  Starting round
                Console.WriteLine($"\nIt's {currentPlayer.name}'s turn.");
                Console.WriteLine("\nHand:");
                Player.PrintHand(currentPlayer);

                while (!chosecard)
                {
                    Availablelength = Card.GetLen(currentPlayer.Hand);
                    Console.WriteLine("Enter the index for the card to use: ");
                    chosencardindex = Int32.Parse(Console.ReadLine());

                    if ((chosencardindex< Availablelength) | (chosencardindex > 0))
                    {
                        chosecard = true;
                        chosenCard = currentPlayer.Hand[chosencardindex - 1];
                        // Use the card
                        Middle[0] = Card.CreateCard( chosenCard.Val, chosenCard.Color,chosenCard.Type);
                        // Get the card out of the hand
                        Player.UseCard(currentPlayer.Hand, chosencardindex - 1);
                    }

                }
                chosecard = false;
                ///////////// Check if played card is an action card
                if (Middle[0].Type == "ColorActCard")
                    {
                    if (Middle[0].Val == "Reverse")
                    {
                        orderindexer *= -1;
                        if (N_Players == 2)
                        {
                            turn += orderindexer;
                        }
                    }
                    else if (Middle[0].Val == "Block")
                    {
                        turn += orderindexer;
                    }
                    else
                    {
                        turn += orderindexer;
                        Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));
                        if (Card.GetLen(Deck) != 0)
                        {
                            Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));
                        }
                    }
                }
                    else if (Middle[0].Type == "NoColorActCard")
                {
                    if (Middle[0].Val == "Draw4")
                    {
                        turn += orderindexer;
                        Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));

                        for (int k = 0; k < 3; k++)
                        {
                            if (Card.GetLen(Deck) != 0)
                            {
                                Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));
                            }

                        }
                        validcolor = false;
                        while (!validcolor)
                        {
                            Console.WriteLine("Pick a color: (R,G,B,Y)");
                            pickedcolorwild = (Console.ReadLine()).ToCharArray()[0];
                            foreach (char clr in colors)
                            {
                                if (pickedcolorwild == clr)
                                {
                                    validcolor = true;
                                }
                            }
                        }
                        Middle[0].Color = pickedcolorwild;

                    }
                    else
                    {
                        validcolor = false;
                        while (!validcolor)
                        {
                            Console.WriteLine("Pick a color: (R,G,B,Y)");
                            pickedcolorwild = (Console.ReadLine()).ToCharArray()[0];
                            foreach (char clr in colors)
                            {
                                if (pickedcolorwild == clr)
                                {
                                    validcolor = true;
                                }
                            }
                        }
                        Middle[0].Color = pickedcolorwild;
                    }
                }

                turn += orderindexer;
                /////////////////////////////////////////////////////////////////////////


                // Next rounds
                while (running)
                {   
                    Console.Clear();

                    Console.WriteLine($"\n{Card.GetLen(Deck)} cards left in deck");
                    Console.WriteLine($"\n{Card.GetLen(Middle)} cards are in the middle");
                    Console.WriteLine("#################################################");
                    
                    turn %= N_Players;
                    if (turn < 0)
                    {
                        turn += N_Players;
                    }

                    currentPlayer = Players[turn];
                    // Deck is empty
                    if (Card.GetLen(Deck) == 0)
                    {
                        Array.Copy(Middle, Deck, Card.N_CARDS);
                        Card.Nullify(Middle);
                        Middle[0] = Card.DrawCard(Deck);
                        Card.Shuffle(Deck);

                    }
                    middlelength = Card.GetLen(Middle);

                    Console.WriteLine($"It's {currentPlayer.name}'s turn.");

                    Console.WriteLine("\nMiddle:");
                    Card.PrintCard(Card.GetTop(Middle));

                    // No playable cards in hand
                    while (Card.GetLen(Card.GetUsableCards(currentPlayer.Hand, Middle[middlelength - 1])) == 0)
                    {
                        drawnCard = Card.DrawCard(Deck);
                        Console.WriteLine("\n!!!No playable card, drawing a card!!!");
                        Console.WriteLine("\n Drawn card:");
                        Card.PrintCard(drawnCard);
                        Player.AddToHand(currentPlayer, drawnCard);
                        //Console.WriteLine("\nHand:");
                        //Player.PrintHand(currentPlayer);
                    }

                    Console.WriteLine("\nCards left for each player:");
                    for (int p = 0; p < N_Players; p++)
                    {
                        Console.Write($"\t{Players[p].name}: {Card.GetLen(Players[p].Hand)}");
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("\nHand:");
                    Player.PrintHand(currentPlayer);

                    //Show available cards and let user pick
                    while (!chosecard)
                    {
                        Available = Card.GetUsableCards(currentPlayer.Hand, Middle[middlelength - 1]);
                        Console.WriteLine("\nAvailable cards:");
                        Card.PrintDeck(Available);

                        Availablelength = Card.GetLen(Available);
                        Console.WriteLine("Enter the index for the card in hand to use: ");
                        chosencardindex = Int32.Parse(Console.ReadLine());

                        if ((chosencardindex <= Availablelength) & (chosencardindex > 0))
                        {
                            chosenCard = Available[chosencardindex - 1];
                            // Use the card
                            Middle[middlelength] = Card.CreateCard(chosenCard.Val, chosenCard.Color, chosenCard.Type);
                            // Get the card out of the hand
                            Player.UseCard(currentPlayer.Hand, Card.Index(currentPlayer.Hand, chosenCard));
                            chosecard = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid index, index should be in range 1-{Availablelength}");
                        }

                    }
                    chosecard = false;

                    // Check if the player won
                    if (Card.GetLen(currentPlayer.Hand) == 0)
                    {
                        running = false;
                        winnername = currentPlayer.name;
                    }
                    else 
                    {
                        // Check if the played card is an action card
                        if (Middle[middlelength].Type == "ColorActCard")
                        {
                            if (Middle[middlelength].Val == "Reverse")
                            {
                                orderindexer *= -1;
                                if (N_Players == 2)
                                {
                                    turn += orderindexer;
                                }
                            }
                            else if (Middle[middlelength].Val == "Block")
                            {
                                turn += orderindexer;
                            }
                            else
                            {
                                turn += orderindexer;
                                if (turn < 0)
                                {
                                    turn += N_Players;
                                }
                                Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));
                                if (Card.GetLen(Deck) != 0)
                                {
                                    Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));
                                }
                            }
                        }
                        else if (Middle[middlelength].Type == "NoColorActCard")
                        {
                            if (Middle[middlelength].Val == "Draw4")
                            {
                                turn += orderindexer;
                                Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));

                                for (int k = 0; k < 3; k++)
                                {
                                    if (Card.GetLen(Deck) != 0)
                                    {
                                        Player.AddToHand(Players[turn % N_Players], Card.DrawCard(Deck));
                                    }

                                }
                                validcolor = false;
                                while (!validcolor)
                                {
                                    Console.WriteLine("Pick a color: (R,G,B,Y)");
                                    pickedcolorwild = (Console.ReadLine()).ToCharArray()[0];
                                    foreach (char clr in colors) {
                                        if (pickedcolorwild == clr)
                                        {
                                            validcolor = true;
                                        }
                                    }
                                }
                                Middle[middlelength].Color = pickedcolorwild;

                            }
                            else
                            {
                                validcolor = false;
                                while (!validcolor)
                                {
                                    Console.WriteLine("Pick a color: (R,G,B,Y)");
                                    pickedcolorwild = (Console.ReadLine()).ToCharArray()[0];
                                    foreach (char clr in colors)
                                    {
                                        if (pickedcolorwild == clr)
                                        {
                                            validcolor = true;
                                        }
                                    }
                                }
                                Middle[middlelength].Color = pickedcolorwild;
                            }
                        }

                        turn += orderindexer;
                    }
                }


                // Quit or restart the game
                Console.WriteLine($"\nGame ended. {winnername} is the winner!");
                Console.WriteLine($"\nPress enter to quit or type anything to play again");

                if (Console.ReadLine() == "")
                {
                    quit = true;
                }

            }

        }
    }
}
