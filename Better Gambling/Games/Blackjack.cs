using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class Blackjack : IGambleGame
{
    public string GetName => "Blackjack";

    public string GetDescription => "ALT+O to Reset, ALT+I to Hit, ALT+L to Stand";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    Deck deck = new Deck();

    List<Card> playerHand;
    List<Card> dealerHand;
    bool playerStood;

    public int GetTotal(List<Card> hand)
    {
        int aces = 0;
        int total = 0;
        foreach (Card card in hand)
        {
            if (card.Rank == "A")
            {
                aces++;
            }
            else if (card.Rank == "J" || card.Rank == "Q" || card.Rank == "K")
            {
                total += 10;
            }
            else
            {
                total += int.Parse(card.Rank);
            }
        }

        for (int i = 0; i < aces; i++)
        {
            if (total + 11 > 21)
            {
                total++;
            }
            else
            {
                total += 11;
            }
        }

        return total;
    }

    public void GameStart()
    {
        ConCommand.Say("[Blackjack] Game Started");
        playerHand = new List<Card>();
        dealerHand = new List<Card>();
        playerStood = false;
    }

    public void Stand()
    {
        if (!playerStood)
        {
            playerStood = true;
            ConCommand.Say("[Blackjack] Player stood at " + GetTotal(playerHand));
        }
    }

    public void Hit()
    {
        if (playerStood)
        {
            Card newCard = deck.TakeNext();

            dealerHand.Add(newCard);

            if(GetTotal(dealerHand) > 21)
            {
                if (playerHand.Count == 2 && GetTotal(playerHand) == 21)
                {
                    ConCommand.Say("[Blackjack] (PLAYER BLACKJACK) Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
                }
                else
                {
                    ConCommand.Say("[Blackjack] (DEALER BUST) Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
                }
            }
            else if(GetTotal(dealerHand) >= 17)
            {
                if(GetTotal(dealerHand) > GetTotal(playerHand))
                {
                    ConCommand.Say("[Blackjack] (DEALER WIN) Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
                }
                else if(GetTotal(dealerHand) == GetTotal(playerHand))
                {
                    ConCommand.Say("[Blackjack] (PUSH) Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
                }
                else
                {
                    if (playerHand.Count == 2 && GetTotal(playerHand) == 21)
                    {
                        ConCommand.Say("[Blackjack] (PLAYER BLACKJACK) Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
                    }
                    else
                    {
                        ConCommand.Say("[Blackjack] (PLAYER WIN) Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
                    }
                }
            }
            else
            {
                ConCommand.Say("[Blackjack] Dealer Hit: " + newCard.GetName + " (Total: " + GetTotal(dealerHand) + ") | Player Total: " + GetTotal(playerHand));
            }
        }
        else if(playerHand.Count == 0)
        {
            Card[] newCards = new Card[3]{
                deck.TakeNext(), // Player's first card
                deck.TakeNext(), // Player's second card
                deck.TakeNext() // Dealer's face card
            };

            playerHand.Add(newCards[0]);
            playerHand.Add(newCards[1]);
            dealerHand.Add(newCards[2]);

            ConCommand.Say(
                "[Blackjack] Player Cards: " + newCards[0].GetName + " and " + newCards[1].GetName + "(Total: " + GetTotal(playerHand) + ") | Dealer Card: " + newCards[2].GetName + " (Total: " + GetTotal(dealerHand) + ")"
            );
        }
        else
        {
            Card newCard = deck.TakeNext();

            playerHand.Add(newCard);

            if(GetTotal(playerHand) > 21)
            {
                ConCommand.Say("[Blackjack] (PLAYER BUST) Player Hit: " + newCard.GetName + " (Total: " + GetTotal(playerHand) + ")");
            }
            else
            {
                ConCommand.Say("[Blackjack] Player Hit: " + newCard.GetName + " (Total: " + GetTotal(playerHand) + ")");
            }
        }
    }

    public void InitializeActions()
    {
        Actions[Key.O] = () =>
        {
            GameStart();
        };

        Actions[Key.I] = () =>
        {
            Hit();
        };

        Actions[Key.L] = () =>
        {
            Stand();
        };
    }

    public void MainLoop()
    {
        
    }
}
