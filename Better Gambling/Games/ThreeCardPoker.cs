using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class ThreeCardPoker : IGambleGame
{
    class Hand
    {
        public List<Card> cards;

        public enum HandValue
        {
            NoHand,
            Pair,
            Flush,
            Straight,
            ThreeOfAKind,
            StraightFlush
        }

        public static Dictionary<HandValue, int> anteBonusPays = new Dictionary<HandValue, int>()
        {
            [HandValue.Straight] = 1,
            [HandValue.ThreeOfAKind] = 4,
            [HandValue.StraightFlush] = 5
        };

        public static Dictionary<HandValue, int> pairPlusPays = new Dictionary<HandValue, int>()
        {
            [HandValue.Pair] = 1,
            [HandValue.Flush] = 4,
            [HandValue.Straight] = 6,
            [HandValue.ThreeOfAKind] = 30,
            [HandValue.StraightFlush] = 40
        };

        string[] rankOrder = new string[]
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

        public Hand()
        {
            cards = new List<Card>()
            {
                deck.TakeNext(),
                deck.TakeNext(),
                deck.TakeNext()
            };
        }

        public bool ContainsThreeOfAKind()
        {
            return (cards[0].Rank == cards[1].Rank && cards[0].Rank == cards[2].Rank);
        }

        public bool ContainsStraight()
        {
            List<Card> ordered = cards.OrderBy(card => Deck.cardValues[card.Rank]).ToList();

            if (ordered[0].Rank == "A" && ordered[1].Rank == "Q" && ordered[2].Rank == "K") return true;

            return (Deck.cardValues[ordered[0].Rank] + 2 == Deck.cardValues[ordered[2].Rank]);
        }

        public bool ContainsFlush()
        {
            string suit = cards[0].Suit;
            return cards.All(card => card.Suit == suit);
        }

        public bool ContainsPair()
        {
            return (cards[0].Rank == cards[1].Rank || cards[1].Rank == cards[2].Rank || cards[2].Rank == cards[0].Rank);
        }

        public int RunningTotal()
        {
            int total = 0;

            foreach (Card card in cards)
            {
                if (card.Rank == "A")
                {
                    total += (Deck.cardValues["K"] + 1);
                }
                else
                {
                    total += Deck.cardValues[card.Rank];
                }
            }

            return total;
        }

        public HandValue Value()
        {
            bool straight = ContainsStraight() && !ContainsPair();
            bool flush = ContainsFlush();
            if (straight && flush) return HandValue.StraightFlush;

            if (ContainsThreeOfAKind()) return HandValue.ThreeOfAKind;

            if (straight) return HandValue.Straight;

            if (flush) return HandValue.Flush;

            if (ContainsPair()) return HandValue.Pair;

            return HandValue.NoHand;
        }

        public int HighCard()
        {
            List<Card> ordered = cards.OrderBy(card => Deck.cardValues[card.Rank]).ToList();

            int cardValue = Deck.cardValues[ordered[2].Rank];

            if(cardValue == 1 || ordered[0].Rank == "A")
            {
                return Deck.cardValues["K"] + 1;
            }

            return cardValue;
        }

        public string HighCardString()
        {
            if (HighCard() == Deck.cardValues["K"] + 1) return "A";

            List<Card> ordered = cards.OrderBy(card => Deck.cardValues[card.Rank]).ToList();

            return ordered[2].Rank;
        }

        public string GetName()
        {
            switch (Value())
            {
                case HandValue.NoHand:
                    return HighCardString() + " High";
                case HandValue.Pair:
                    return "Pair";
                case HandValue.Flush:
                    return "Flush";
                case HandValue.Straight:
                    return "Straight";
                case HandValue.ThreeOfAKind:
                    return "Three of a Kind";
                case HandValue.StraightFlush:
                    return "Straight Flush";
                default:
                    return "You broke something";
            }
        }

        public static Hand Compare(Hand hand1, Hand hand2)
        {
            HandValue hand1Value = hand1.Value();
            HandValue hand2Value = hand2.Value();

            if (hand1Value > hand2Value)
            {
                return hand1;
            }
            else if (hand1Value < hand2Value)
            {
                return hand2;
            }
            else
            {
                switch (hand1Value)
                {
                    case HandValue.StraightFlush:
                    case HandValue.Straight:
                        {
                            List<Card> hand1Cards = hand1.cards.OrderBy(card => Deck.cardValues[card.Rank]).ToList();
                            List<Card> hand2Cards = hand2.cards.OrderBy(card => Deck.cardValues[card.Rank]).ToList();

                            if (Deck.cardValues[hand1Cards[0].Rank] > Deck.cardValues[hand2Cards[0].Rank])
                            {
                                return hand1;
                            }
                            else if (Deck.cardValues[hand1Cards[0].Rank] < Deck.cardValues[hand2Cards[0].Rank])
                            {
                                return hand2;
                            }
                            else
                            {
                                return null; // tie
                            }
                        }
                    case HandValue.ThreeOfAKind:
                    case HandValue.Flush:
                    case HandValue.NoHand:
                        {
                            Dictionary<string, int> newValues = new Dictionary<string, int>();

                            foreach(KeyValuePair<string, int> val in Deck.cardValues)
                            {
                                newValues[val.Key] = val.Value;
                            }

                            newValues["A"] = newValues["K"]+1;

                            List<Card> hand1Ordered = hand1.cards.OrderBy(card => newValues[card.Rank]).Reverse().ToList();
                            List<Card> hand2Ordered = hand2.cards.OrderBy(card => newValues[card.Rank]).Reverse().ToList();

                            for (int i = 0; i < hand1Ordered.Count; i++)
                            {
                                int hand1High = newValues[hand1Ordered[i].Rank];
                                int hand2High = newValues[hand2Ordered[i].Rank];

                                if (hand1High > hand2High)
                                {
                                    return hand1;
                                }
                                else if (hand1High < hand2High)
                                {
                                    return hand2;
                                }
                            }

                            return null;
                        }
                    case HandValue.Pair:
                        {
                            string hand1Rank = "";
                            string hand1Other = "";
                            if(hand1.cards[0].Rank == hand1.cards[1].Rank)
                            {
                                hand1Rank = hand1.cards[0].Rank;
                                hand1Other = hand1.cards[2].Rank;
                            }
                            else
                            {
                                hand1Rank = hand1.cards[2].Rank;
                                if(hand1.cards[0].Rank == hand1.cards[2].Rank)
                                {
                                    hand1Other = hand1.cards[1].Rank;
                                }
                                else
                                {
                                    hand1Other = hand1.cards[0].Rank;
                                }
                            }

                            string hand2Rank = "";
                            string hand2Other = "";
                            if (hand2.cards[0].Rank == hand2.cards[1].Rank)
                            {
                                hand2Rank = hand2.cards[0].Rank;
                                hand2Other = hand2.cards[2].Rank;
                            }
                            else
                            {
                                hand2Rank = hand2.cards[2].Rank;
                                if (hand2.cards[0].Rank == hand2.cards[2].Rank)
                                {
                                    hand2Other = hand2.cards[1].Rank;
                                }
                                else
                                {
                                    hand2Other = hand2.cards[0].Rank;
                                }
                            }

                            if (hand1Rank == "A" && hand2Rank != "A")
                            {
                                return hand1;
                            }
                            else if(hand2Rank == "A" && hand1Rank != "A")
                            {
                                return hand2;
                            }

                            if(Deck.cardValues[hand1Rank] > Deck.cardValues[hand2Rank])
                            {
                                return hand1;
                            }
                            else if (Deck.cardValues[hand1Rank] < Deck.cardValues[hand2Rank])
                            {
                                return hand2;
                            }

                            if (hand1Other == "A" && hand2Other != "A")
                            {
                                return hand1;
                            }
                            else if (hand2Other == "A" && hand1Other != "A")
                            {
                                return hand2;
                            }

                            if (Deck.cardValues[hand1Other] > Deck.cardValues[hand2Other])
                            {
                                return hand1;
                            }
                            else if (Deck.cardValues[hand1Other] < Deck.cardValues[hand2Other])
                            {
                                return hand2;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    default:
                        return null;
                }
            }
        }
    }

    public string GetName => "Three Card Poker";

    public string GetDescription => "ALT+O to Reset Game, ALT+I to Advance Game, ALT+L to Fold, ALT+(0-9) for Number Entry, ALT+BACKSPACE to Reset number entry";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    static Deck deck = new Deck();

    string textEntry = "";
    int ante = 0;
    int pairPlus = 0;

    enum Phase
    {
        ChooseAnte,
        ChoosePairPlus,
        DisplayPlayerCards,
        PairPlus,
        PlayOrFold,
        PlayerChoice,
        RevealWinner,
    }

    Phase phase = Phase.ChooseAnte;

    Hand playerHand;
    Hand dealerHand;

    int InputNumber()
    {
        int output = -1;

        if (int.TryParse(textEntry, out output))
        {
            ResetTextEntry();
            return output;
        }
        else
        {
            return -1;
        }
    }

    void TextEntry(string text)
    {
        textEntry += text;
    }

    void ResetTextEntry()
    {
        textEntry = "";
    }

    void AdvanceGame()
    {
        switch (phase)
        {
            case Phase.ChooseAnte:
                {
                    int value = InputNumber();

                    if (value > -1)
                    {
                        phase = Phase.ChoosePairPlus;
                        ante = value;

                        if (value == 0)
                        {
                            ConCommand.Say("[Poker] No ante set, please choose pair plus bet");
                        }
                        else
                        {
                            ConCommand.Say("[Poker] Ante set to: " + Gambling.FormatMoney(ante) + ", please choose pair plus bet");
                        }
                    }
                    else
                    {
                        ConCommand.Say("[Poker] Ante set incorrectly, please try again");
                    }
                    break;
                }
            case Phase.ChoosePairPlus:
                {
                    int value = InputNumber();

                    if (value > -1)
                    {
                        phase = Phase.DisplayPlayerCards;
                        pairPlus = value;

                        if (value == 0)
                        {
                            // Can't start a game with no ante or pairplus bet
                            if (ante == 0)
                            {
                                GameStart();
                                return;
                            }

                            ConCommand.Say("[Poker] No pair plus bet set.");
                        }
                        else
                        {
                            ConCommand.Say("[Poker] Pair Plus set to: " + Gambling.FormatMoney(pairPlus));
                        }
                    }
                    else
                    {
                        ConCommand.Say("[Poker] Pair Plus set incorrectly, please try again");
                    }
                    break;
                }
            case Phase.DisplayPlayerCards:
                {
                    string result = "[Poker] Player Cards: ";

                    foreach (Card card in playerHand.cards)
                    {
                        result += "|" + card.GetName + "| ";
                    }

                    if (Hand.anteBonusPays.ContainsKey(playerHand.Value()))
                    {
                        int amount = Hand.anteBonusPays[playerHand.Value()] * ante;

                        result += "Ante Bonus: " + Gambling.FormatMoney(amount);
                    }

                    ConCommand.Say(result);

                    if (ante == 0)
                    {
                        phase = Phase.PairPlus;
                    }
                    else
                    {
                        phase = Phase.PlayOrFold;
                    }

                    break;
                }
            case Phase.PairPlus:
                {
                    switch (playerHand.Value())
                    {
                        case Hand.HandValue.Pair:
                            ConCommand.Say("[Poker] Pair Plus win: " + (Gambling.FormatMoney(pairPlus*2)) + " (" + playerHand.GetName() + ")");
                            break;
                        case Hand.HandValue.Flush:
                            ConCommand.Say("[Poker] Pair Plus win: " + (Gambling.FormatMoney(pairPlus * 4 + pairPlus)) + " (" + playerHand.GetName() + ")");
                            break;
                        case Hand.HandValue.Straight:
                            ConCommand.Say("[Poker] Pair Plus win: " + (Gambling.FormatMoney(pairPlus * 6 + pairPlus)) + " (" + playerHand.GetName() + ")");
                            break;
                        case Hand.HandValue.ThreeOfAKind:
                            ConCommand.Say("[Poker] Pair Plus win: " + (Gambling.FormatMoney(pairPlus * 30 + pairPlus)) + " (" + playerHand.GetName() + ")");
                            break;
                        case Hand.HandValue.StraightFlush:
                            ConCommand.Say("[Poker] Pair Plus win: " + (Gambling.FormatMoney(pairPlus * 40 + pairPlus)) + " (" + playerHand.GetName() + ")");
                            break;
                        default:
                            ConCommand.Say("[Poker] Pair Plus loss: " + playerHand.HighCardString() + " High");
                            break;
                    }
                    break;
                }
            case Phase.PlayOrFold:
                {
                    ConCommand.Say("[Poker] Would you like to Play (" + Gambling.FormatMoney(ante) + ") or Fold?");
                    phase = Phase.PlayerChoice;
                    break;
                }
            case Phase.PlayerChoice:
                {
                    string result = "[Poker] Dealer Cards: ";

                    foreach (Card card in dealerHand.cards)
                    {
                        result += "|" + card.GetName + "| ";
                    }

                    result += "(" + dealerHand.GetName() + ")";

                    ConCommand.Say(result);

                    phase = Phase.RevealWinner;

                    break;
                }
            case Phase.RevealWinner:
                {
                    if (dealerHand.Value() == Hand.HandValue.NoHand)
                    {
                        if (dealerHand.HighCard() < Deck.cardValues["Q"] && dealerHand.HighCard() != 1)
                        {
                            ConCommand.Say("[Poker] Dealer folds: " + Gambling.FormatMoney(ante * 3) + " returned to player.");

                            if (pairPlus > 0)
                            {
                                phase = Phase.PairPlus;
                            }
                            return;
                        }
                    }

                    Hand winner = Hand.Compare(playerHand, dealerHand);

                    if (winner == playerHand)
                    {
                        ConCommand.Say("[Poker] Player Wins: " + playerHand.GetName() + " (" + Gambling.FormatMoney(ante * 4) + ")");
                    }
                    else if (winner == dealerHand)
                    {
                        ConCommand.Say("[Poker] Dealer Wins: " + dealerHand.GetName());
                    }
                    else
                    {
                        ConCommand.Say("[Poker] Push: " + Gambling.FormatMoney(ante * 4) + " returned to player.");
                    }

                    if (pairPlus > 0)
                    {
                        phase = Phase.PairPlus;
                    }

                    break;
                }
            default:
                break;
        }
    }

    void Fold()
    {
        if (phase == Phase.PlayerChoice)
        {
            string result = "[Poker] Player has folded, Dealer Cards: ";

            foreach (Card card in dealerHand.cards)
            {
                result += "|" + card.GetName + "| ";
            }

            result += "(" + dealerHand.GetName() + ")";

            if (pairPlus > 0)
            {
                phase = Phase.PairPlus;
                ConCommand.Say(result + " Pair Plus coming up...");
            }
            else
            {
                ConCommand.Say(result);
            }
        }
    }

    public void GameStart()
    {
        textEntry = "";
        ante = 0;
        pairPlus = 0;
        phase = Phase.ChooseAnte;
        ConCommand.Say("[Poker] Game Start, please choose ante");

        playerHand = new Hand();
        dealerHand = new Hand();
    }

    public void InitializeActions()
    {
        Actions[Key.O] = () => GameStart();

        Actions[Key.I] = () => AdvanceGame();

        Actions[Key.L] = () => Fold();

        Actions[Key.Back] = () => ResetTextEntry();

        Actions[Key.D0] = () => TextEntry("0");
        Actions[Key.D1] = () => TextEntry("1");
        Actions[Key.D2] = () => TextEntry("2");
        Actions[Key.D3] = () => TextEntry("3");
        Actions[Key.D4] = () => TextEntry("4");
        Actions[Key.D5] = () => TextEntry("5");
        Actions[Key.D6] = () => TextEntry("6");
        Actions[Key.D7] = () => TextEntry("7");
        Actions[Key.D8] = () => TextEntry("8");
        Actions[Key.D9] = () => TextEntry("9");
    }

    public void MainLoop()
    {

    }
}
