using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            BlackJack blackJack = new BlackJack();
            
            blackJack.PlayGame();
        }
        public class Card
        {
            private enum CardRank { Two = 0, Three = 1, Four = 2, Five = 3, Six = 4, Seven = 5, Eight = 6, Nine = 7, Ten = 8, Jack = 9, Queen = 10, King = 11, Ace = 12 }
            private enum CardSuit { Spade = 0, Club = 1, Heart = 2, Diamond = 3 }
            int i;
            public override string ToString()
            {
                return ((CardRank)(i % 13)).ToString() + " " + ((CardSuit)(i / 13)).ToString();
            }
            public int Value
            {
                get
                {
                    int value;
                    if (i % 13 == 12)
                    {
                        value = 11;
                    }
                    else
                    {
                        value = i % 13 + 2;
                        if (value > 10)
                        {
                            value = 10;
                        }
                    }
                    return value;
                }
            }
            public Card(int I)
            {
                i = I;
            }
        }
        public abstract class Seat
        {
            public int AceCount { get; private set; } = 0;
            public List<Card> Cards { get; private set; } = new List<Card>();
            public bool Busted
            { get { return Total > 21; } }
            public bool HasBlackJack
            { get { return (Total == 21 && Cards.Count == 2); } }
            public int Total
            {
                get
                {
                    int total = 0;
                    AceCount = 0;
                    foreach (Card c in Cards)
                    {
                        total += c.Value;
                    }
                    if (total > 21 && AceCount > 0)
                    {
                        for (int i = 0; i < AceCount; i++)
                        {
                            total -= 10;
                            if (total <= 21)
                            {
                                break;
                            }
                        }
                    }
                    return total;
                }
            }
            public abstract bool CanHit { get; }
            public void HitMe(Card C)
            {
                Cards.Add(C);
            }
            public virtual string DisplayCards
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Card c in Cards)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(c.ToString());
                    }
                    string cards = sb.ToString();
                    return $"{cards} {Total}";
                }
            }
        }
        public class Dealer : Seat
        {
            public override bool CanHit
            {
                get
                {
                    if (AceCount > 0)
                    {
                        return Total <= 18;
                    }
                    else
                    {
                        return Total <= 17;
                    }
                }
            }
            public override string DisplayCards
            {
                get
                {
                    if (Cards.Count == 2)
                    {
                        return $"{Cards[0].ToString()} {Cards[0].Value}";
                    }
                    else
                    {
                        return base.DisplayCards;
                    }
                }
            }
        }
        public class Player : Seat
        {
            public override bool CanHit
            {
                get
                {
                    return Total < 21;
                }
            }

            public int Wins { get; set; } = 0;
            public int Loss { get; set; } = 0;
            public string WinLoss
            {
                get { return $"Win {Wins} Loss {Loss}"; }
            }
        }
        public class BlackJack
        {
            Random rand = new Random();
            private List<Card> Deck;
            private int deckPos = -1;
            Dealer dealer = new Dealer();
            Player player = new Player();
            private void Shuffle()
            {
                deckPos = -1;
                int swap;
                Card temp;
                for (int i = Deck.Count - 1; i > 0; i--)
                {
                    swap = rand.Next(i + 1);  
                    if (swap != i)
                    {
                        temp = Deck[i];
                        Deck[i] = Deck[swap];
                        Deck[swap] = temp;
                    }
                }
            }
            public BlackJack()
            {
                Deck = new List<Card>();
                for (int i = 0; i < 52; i++)
                {
                    Card card = new Card(i);
                    Console.WriteLine(card.ToString());
                    Deck.Add(card);
                }
                Console.WriteLine("");
                Shuffle();
            }
            private Card Pop()
            {
                deckPos++;
                return (Deck[deckPos]);
            }
            private Card Peek()
            {
                return (Deck[deckPos]);
            }
            public void PlayGame()
            {
                while (true)
                {
                    Console.WriteLine("C to continue Q to quit");
                    string p = Console.ReadLine();
                    if (p == "C" || p == "c")
                    {
                        PlayHand();
                    }
                    else if (p == "Q" || p == "q")
                    {
                        Console.WriteLine("Bye");
                        break;
                    }
                }
            }
            public void PlayHand()
            {
                if (deckPos > 30)
                {
                    Shuffle();
                }

                dealer.Cards.Clear();
                player.Cards.Clear();

                Console.WriteLine("");
                Console.WriteLine("");

                Card card = Pop();
                dealer.HitMe(card);
                dealer.HitMe(Pop());
                Console.WriteLine("Dealer");
                Console.WriteLine(dealer.DisplayCards);
                if (dealer.HasBlackJack)
                {
                    Console.WriteLine("Player H to hit  S to Stay");
                    player.Loss++;
                }
                else
                {
                    Console.WriteLine("Player H to hit  S to Stay");
                    player.HitMe(Pop());
                    player.HitMe(Pop());
                    Console.WriteLine(player.DisplayCards);
                    string p;
                    while (true)
                    {
                        p = Console.ReadLine();
                        if (p == "H" || p == "h")
                        {
                            if (player.CanHit)
                            {
                                player.HitMe(Pop());
                                Console.WriteLine(player.DisplayCards);
                                if (player.Busted)
                                {
                                    break;
                                }
                            }
                            if (player.HasBlackJack)
                            {
                                break;
                            }
                        }
                        else if (p == "S" || p == "s")
                        {
                            break;
                        }
                    }

                    if (player.Busted)
                    {
                        Console.WriteLine("Player busted out dealer wins");
                        player.Loss++;
                    }
                    else if (player.HasBlackJack)
                    {
                        Console.WriteLine("Blackjack player wins");
                    }
                    else
                    {
                        Console.WriteLine("Dealer");
                        while (dealer.CanHit)
                        {
                            dealer.HitMe(Pop());
                            Console.WriteLine(dealer.DisplayCards);
                            if (dealer.Busted)
                            {
                                break;
                            }
                        }
                        if (dealer.Busted)
                        {
                            Console.WriteLine("Dealer busted out player wins");
                            player.Wins++;
                        }
                        else if (dealer.Total >= player.Total)
                        {
                            Console.WriteLine("Dealer wins");
                            player.Loss++;
                        }
                        else
                        {
                            Console.WriteLine("Player wins");
                            player.Wins++;
                        }
                    }
                }
                Console.WriteLine(player.WinLoss);
            }
        }
    }
}