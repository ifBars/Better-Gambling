using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Card
{
    public string Rank { get; set; }
    public string Suit { get; set; }
    public string GetName => Rank + Suit;

    public Card(string rank, string suit)
    {
        Rank = rank;
        Suit = suit;
    }
}

class Deck
{
    List<Card> cards = new List<Card>();
    private static Random rand = new Random();

    public static string[] cardTypes = new string[]
    {
        "A",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "J",
        "Q",
        "K"
    };

    // This isn't the value of a card in any particular game, but rather their general ranking order
    public static Dictionary<string, int> cardValues = new Dictionary<string, int>()
    {
        ["A"] = 1,
        ["2"] = 2,
        ["3"] = 3,
        ["4"] = 4,
        ["5"] = 5,
        ["6"] = 6,
        ["7"] = 7,
        ["8"] = 8,
        ["9"] = 9,
        ["10"] = 10,
        ["J"] = 11,
        ["Q"] = 12,
        ["K"] = 13,
    };

    public static string[] suits = new string[]
    {
        "♠",
        "♥",
        "♦",
        "♣"
    };

    public void Generate()
    {
        cards = new List<Card>();

        for(int i = 0; i < cardTypes.Length; i++)
        {
            for (int j = 0; j < suits.Length; j++)
            {
                cards.Add(new Card(cardTypes[i], suits[j]));
            }
        }
    }

    public void Shuffle()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            int swap = rand.Next(0, cards.Count);

            Card placeholder = cards[swap];

            cards[swap] = cards[i];
            cards[i] = placeholder;
        }
    }

    public Deck()
    {
        Generate();
        Shuffle();
    }

    public Card TakeNext()
    {
        if(cards.Count > 0)
        {
            Card toTake = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return toTake;
        }
        else
        {
            Generate();
            Shuffle();
            return TakeNext();
        }
    }
}
