using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class HiLo : IGambleGame
{
    public TextEntry text;

    public string GetName => "Draw Hi-Lo";

    public string GetDescription => "ALT+O to reset, ALT+Up for Hi, ALT+Down for Lo, ALT+I to Advance";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    public enum Direction { None, Hi, Lo, GameOver }

    public Direction direction = Direction.None;

    public static Deck deck = new Deck();

    public static Card current;

    public static float currentMultiplier = 1.0f;
    public static int currentBet = 0;

    Dictionary<string, Dictionary<Direction, float>> payTable = new Dictionary<string, Dictionary<Direction, float>>() {
        {"A", new Dictionary<Direction, float>{ { Direction.Lo, 0.0f }, { Direction.Hi, 1.0f} } },
        {"2", new Dictionary<Direction, float>{ { Direction.Lo, 12.0f }, { Direction.Hi, 1.1f} } },
        {"3", new Dictionary<Direction, float>{ { Direction.Lo, 5.0f }, { Direction.Hi, 1.2f} } },
        {"4", new Dictionary<Direction, float>{ { Direction.Lo, 4.0f }, { Direction.Hi, 1.3f} } },
        {"5", new Dictionary<Direction, float>{ { Direction.Lo, 3.0f }, { Direction.Hi, 1.4f} } },
        {"6", new Dictionary<Direction, float>{ { Direction.Lo, 2.0f }, { Direction.Hi, 1.5f} } },
        {"7", new Dictionary<Direction, float>{ { Direction.Lo, 1.8f }, { Direction.Hi, 1.8f} } },
        {"8", new Dictionary<Direction, float>{ { Direction.Lo, 1.5f }, { Direction.Hi, 2.0f} } },
        {"9", new Dictionary<Direction, float>{ { Direction.Lo, 1.4f }, { Direction.Hi, 3.0f} } },
        {"10", new Dictionary<Direction, float>{ { Direction.Lo, 1.3f }, { Direction.Hi, 4.0f} } },
        {"J", new Dictionary<Direction, float>{ { Direction.Lo, 1.2f }, { Direction.Hi, 5.0f} } },
        {"Q", new Dictionary<Direction, float>{ { Direction.Lo, 1.1f }, { Direction.Hi, 12.0f} } },
        {"K", new Dictionary<Direction, float>{ { Direction.Lo, 1.0f }, { Direction.Hi, 0.0f} } },
    };

    public void GameStart()
    {
        direction = Direction.None;

        current = deck.TakeNext();
        currentMultiplier = 1.0f;

        currentBet = text.Read();

        ConCommand.Say("[Hi-Lo] Game Started, Player Card: " + current.GetName + " (Bet: " + Gambling.FormatMoney(currentBet) + ")");
    }

    public bool? CheckHi(Card currentCard, Card nextCard)
    {
        if(Deck.cardValues[nextCard.Rank] > Deck.cardValues[currentCard.Rank])
        {
            return true;
        }
        else if(Deck.cardValues[nextCard.Rank] < Deck.cardValues[currentCard.Rank])
        {
            return false;
        }
        else
        {
            return null;
        }
    }

    public bool? CheckLo(Card currentCard, Card nextCard)
    {
        if (Deck.cardValues[nextCard.Rank] > Deck.cardValues[currentCard.Rank])
        {
            return false;
        }
        else if (Deck.cardValues[nextCard.Rank] < Deck.cardValues[currentCard.Rank])
        {
            return true;
        }
        else
        {
            return null;
        }
    }

    void Advance()
    {
        if(current.Rank == "A" || current.Rank == "K")
        {
            current = deck.TakeNext();
            ConCommand.Say("[Hi-Lo] Decision obvious, New Card: " + current.GetName);
        }
        else
        {
            if(direction == Direction.None)
            {
                ConCommand.Say("[Hi-Lo] Please choose direction (Hi or Lo)");
            }
            else if(direction == Direction.Hi)
            {
                Card next = deck.TakeNext();
                switch(CheckHi(current, next))
                {
                    case true:
                        currentMultiplier *= payTable[current.Rank][Direction.Hi];
                        currentMultiplier = (float)Math.Round(currentMultiplier, 2);
                        current = next;
                        ConCommand.Say("[Hi-Lo] Card Reveal: " + next.GetName + "(Win) (" + currentMultiplier + "x=" + Gambling.FormatMoney((int)(currentBet * currentMultiplier)) + ")");
                        direction = Direction.None;
                        break;
                    case false:
                        ConCommand.Say("[Hi-Lo] Card Reveal: " + next.GetName + " (Lose) (" + currentMultiplier + "x=" + Gambling.FormatMoney((int)(currentBet * currentMultiplier)) + ")");
                        direction = Direction.GameOver;
                        break;
                    default:
                        current = next;
                        ConCommand.Say("[Hi-Lo] Card Reveal: " + next.GetName + " (Push) (" + currentMultiplier + "x=" + Gambling.FormatMoney((int)(currentBet * currentMultiplier)) + ")");
                        direction = Direction.None;
                        break;
                }
            }
            else if(direction == Direction.Lo)
            {
                Card next = deck.TakeNext();
                switch (CheckLo(current, next))
                {
                    case true:
                        currentMultiplier *= payTable[current.Rank][Direction.Lo];
                        currentMultiplier = (float)Math.Round(currentMultiplier, 2);
                        current = next;
                        ConCommand.Say("[Hi-Lo] Card Reveal: " + next.GetName + "(Win) (" + currentMultiplier + "x=" + Gambling.FormatMoney((int)(currentBet * currentMultiplier)) + ")");
                        direction = Direction.None;
                        break;
                    case false:
                        ConCommand.Say("[Hi-Lo] Card Reveal: " + next.GetName + " (Lose) (" + currentMultiplier + "x=" + Gambling.FormatMoney((int)(currentBet * currentMultiplier)) + ")");
                        direction = Direction.GameOver;
                        break;
                    default:
                        current = next;
                        ConCommand.Say("[Hi-Lo] Card Reveal: " + next.GetName + " (Push) (" + currentMultiplier + "x=" + Gambling.FormatMoney((int)(currentBet * currentMultiplier)) + ")");
                        direction = Direction.None;
                        break;
                }
            }
        }
    }

    void Hi()
    {
        if(direction == Direction.None || direction == Direction.Lo)
        {
            direction = Direction.Hi;
            ConCommand.Say("[Hi-Lo] Player has chosen Hi!");
        }
    }

    void Lo()
    {
        if (direction == Direction.None || direction == Direction.Hi)
        {
            direction = Direction.Lo;
            ConCommand.Say("[Hi-Lo] Player has chosen Lo!");
        }
    }

    public void InitializeActions()
    {
        text = new TextEntry(this);

        Actions[Key.I] = Advance;

        Actions[Key.Up] = Hi;

        Actions[Key.Down] = Lo;

        Actions[Key.O] = GameStart;
    }

    public void MainLoop()
    {
        
    }
}