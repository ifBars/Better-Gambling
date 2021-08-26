using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class VideoPoker : IGambleGame
{
    public string GetName => "Video Poker";

    public string GetDescription => "ALT+O Start Game, ALT+I Advance Game, ALT+(1-5) Hold a card";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    List<int> heldCards = new List<int>();
    List<Card> hand = new List<Card>();
    Deck deck = new Deck();
    bool cardsHeld = false;

    bool IsStraight()
    {
        // This is ugly, I know
        Dictionary<string, string> nextCard = new Dictionary<string, string>();
        nextCard["A"] = "2";
        nextCard["2"] = "3";
        nextCard["3"] = "4";
        nextCard["4"] = "5";
        nextCard["5"] = "6";
        nextCard["6"] = "7";
        nextCard["7"] = "8";
        nextCard["8"] = "9";
        nextCard["9"] = "10";
        nextCard["10"] = "J";
        nextCard["J"] = "Q";
        nextCard["Q"] = "K";
        nextCard["K"] = "A";

        List<string> used = new List<string>();

        int totalFound = 0;
        Dictionary<string, int> totalRanks = TotalRanks();

        // lazy hack
        if(totalRanks.ContainsKey("A") && totalRanks.ContainsKey("K") && totalRanks.ContainsKey("2"))
        {
            return false;
        }

        for(int i = 0; i < hand.Count; i++)
        {
            if (used.Contains(hand[i].Rank)) return false;
            if (totalRanks.ContainsKey(nextCard[hand[i].Rank])){
                used.Add(hand[i].Rank);
                totalFound++;
            }
        }

        return totalFound == 4;
    }

    bool IsFlush()
    {
        string suit = null;
        for (int i = 0; i < hand.Count; i++)
        {
            if (suit == null)
            {
                suit = hand[i].Suit;
            }
            else if (suit != hand[i].Suit)
            {
                return false;
            }
        }
        return true;
    }

    Dictionary<string, int> TotalRanks()
    {
        Dictionary<string, int> total = new Dictionary<string, int>();
        for(int i = 0; i < hand.Count; i++)
        {
            if (total.ContainsKey(hand[i].Rank))
            {
                total[hand[i].Rank]++;
            }
            else
            {
                total[hand[i].Rank] = 1;
            }
        }

        return total;
    }

    bool IsRoyal()
    {
        List<string> required = new List<string>() { "10", "J", "Q", "K", "A" };

        for(int i = 0; i < hand.Count; i++)
        {
            for(int j = 0; j < required.Count; j++)
            {
                if(hand[i].Rank == required[j])
                {
                    required.RemoveAt(j);
                    continue;
                }
            }
        }

        return required.Count == 0;
    }

    bool IsJackOrHigher(string rank)
    {
        return (rank == "J" || rank == "Q" || rank == "K" || rank == "A");
    }

    void HoldCard(int cardToHold)
    {
        if (!heldCards.Contains(cardToHold))
        {
            heldCards.Add(cardToHold);
        }
    }

    void AdvanceGame()
    {
        if (hand.Count == 0)
        {
            heldCards = new List<int>();

            string result = "[Video Poker] |";
            for (int i = 0; i < 5; i++)
            {
                Card newCard = deck.TakeNext();
                hand.Add(newCard);

                result += newCard.GetName + "| ";
            }

            result += "Please choose which cards to hold";

            ConCommand.Say(result);
        }
        else if (!cardsHeld)
        {
            cardsHeld = true;
            string result = "[Video Poker] |";

            for (int i = 0; i < 5; i++)
            {
                if (!heldCards.Contains(i))
                {
                    hand[i] = deck.TakeNext();
                }
                result += hand[i].GetName + "| ";
            }

            ConCommand.Say(result);
        }
        else
        {
            if (IsFlush())
            {
                if (IsRoyal())
                {
                    ConCommand.Say("[Video Poker] (PLAYER WIN): Royal Flush 250x");
                    return;
                }
                else if (IsStraight())
                {
                    ConCommand.Say("[Video Poker] (PLAYER WIN): Straight Flush 50x");
                    return;
                }
            }

            Dictionary<string, int> totalRanks = TotalRanks();

            if (totalRanks.ContainsValue(4)) {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Four of a Kind 25x");
                return;
            }
            else if(totalRanks.ContainsValue(3) && totalRanks.ContainsValue(2))
            {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Full House 9x");
                return;
            }

            if (IsFlush())
            {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Flush 6x");
                return;
            }

            if (IsStraight())
            {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Straight 4x");
                return;
            }

            if (totalRanks.ContainsValue(3))
            {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Three of a Kind 3x");
                return;
            }

            int totalPairs = 0;
            bool jacksOrHigher = false;
            foreach(KeyValuePair<string, int> total in totalRanks)
            {
                if(total.Value == 2)
                {
                    if (IsJackOrHigher(total.Key))
                    {
                        jacksOrHigher = true;
                    }
                    totalPairs++;
                }
            }

            if(totalPairs == 2)
            {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Two Pair 2x");
                return;
            }

            if (jacksOrHigher)
            {
                ConCommand.Say("[Video Poker] (PLAYER WIN): Jacks or Higher 1x");
                return;
            }

            ConCommand.Say("[Video Poker] (PLAYER LOSE): No Match");
        }
    }

    public void GameStart()
    {
        heldCards = new List<int>();
        hand = new List<Card>();
        cardsHeld = false;

        ConCommand.Say("[Video Poker] Game Start");
    }

    public void InitializeActions()
    {
        Actions[Key.D1] = () => HoldCard(0);
        Actions[Key.D2] = () => HoldCard(1);
        Actions[Key.D3] = () => HoldCard(2);
        Actions[Key.D4] = () => HoldCard(3);
        Actions[Key.D5] = () => HoldCard(4);

        Actions[Key.O] = () => GameStart();
        Actions[Key.I] = () => AdvanceGame();
    }

    public void MainLoop()
    {

    }
}