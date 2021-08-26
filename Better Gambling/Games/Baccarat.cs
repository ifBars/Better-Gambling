using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class Baccarat : IGambleGame
{
    public string GetName => "Baccarat";

    public string GetDescription => "ALT+O to Reset, ALT+I to Advance Game";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    List<Card> playerHand;
    List<Card> bankerHand;

    bool playerStood = false;
    bool bankerStood = false;

    Deck deck = new Deck();

    Dictionary<string, int> cardValues = new Dictionary<string, int>()
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
        ["10"] = 0,
        ["J"] = 0,
        ["Q"] = 0,
        ["K"] = 0
    };

    int GetTotal(List<Card> hand)
    {
        int total = 0;
        foreach (Card card in hand)
        {
            total += cardValues[card.Rank];
        }

        return total % 10;
    }

    public void GameStart()
    {
        ConCommand.Say("[Baccarat] Game Started");

        playerHand = new List<Card>();
        bankerHand = new List<Card>();
        playerStood = false;
        bankerStood = false;
    }

    void AdvanceGame()
    {
        if (playerHand.Count == 0)
        {
            playerHand.Add(deck.TakeNext());
            playerHand.Add(deck.TakeNext());
            ConCommand.Say("[Baccarat] Player Hand |" + playerHand[0].GetName + "|" + " |" + playerHand[1].GetName + "| (Total: " + GetTotal(playerHand) + ")");
        }
        else if (bankerHand.Count == 0)
        {
            bankerHand.Add(deck.TakeNext());
            bankerHand.Add(deck.TakeNext());
            ConCommand.Say("[Baccarat] Banker Hand |" + bankerHand[0].GetName + "|" + " |" + bankerHand[1].GetName + "| (Total: " + GetTotal(bankerHand) + ")");
        }
        else
        {
            int playerTotal = GetTotal(playerHand);
            int bankerTotal = GetTotal(bankerHand);
            if (playerHand.Count == 2 && bankerHand.Count == 2 && !playerStood)
            {
                if (playerTotal == 8 || playerTotal == 9 || bankerTotal == 8 || bankerTotal == 9)
                {
                    if (playerTotal > bankerTotal)
                    {
                        ConCommand.Say("[Baccarat] (PLAYER WIN) Player's Natural " + playerTotal + " beats Banker's " + bankerTotal);
                    }
                    else if (playerTotal < bankerTotal)
                    {
                        ConCommand.Say("[Baccarat] (BANKER WIN) Banker's Natural " + bankerTotal + " beats Player's " + playerTotal);
                    }
                    else
                    {
                        ConCommand.Say("[Baccarat] (TIE) Player's Natural" + playerTotal + " is matched by the Banker");
                    }
                }
                else
                {
                    if (playerTotal <= 5)
                    {
                        Card newCard = deck.TakeNext();
                        playerHand.Add(newCard);
                        ConCommand.Say("[Baccarat] Player draws a |" + newCard.GetName + "| (Total: " + GetTotal(playerHand) + ")");
                    }
                    else
                    {
                        playerStood = true;
                        ConCommand.Say("[Baccarat] Player stands at " + playerTotal);
                    }
                }
            }
            else
            {
                if (playerStood)
                {
                    if (bankerHand.Count == 2 && bankerStood == false)
                    {
                        if (bankerTotal <= 5)
                        {
                            bankerStood = true;
                            Card newCard = deck.TakeNext();
                            bankerHand.Add(newCard);
                            ConCommand.Say("[Baccarat] Banker draws a |" + newCard.GetName + "| (Total: " + GetTotal(bankerHand) + ")");
                        }
                        else
                        {
                            bankerStood = true;
                            ConCommand.Say("[Baccarat] Banker stands at " + bankerTotal);
                        }
                    }
                    else
                    {
                        if (bankerStood)
                        {
                            if (playerTotal > bankerTotal)
                            {
                                ConCommand.Say("[Baccarat] (PLAYER WIN) Player's " + playerTotal + " beats Banker's " + bankerTotal);
                            }
                            else if (playerTotal < bankerTotal)
                            {
                                ConCommand.Say("[Baccarat] (BANKER WIN) Banker's " + bankerTotal + " beats Player's " + playerTotal);
                            }
                            else
                            {
                                ConCommand.Say("[Baccarat] (TIE) Player's " + playerTotal + " is matched by the Banker");
                            }
                        }
                    }
                }
                else
                {
                    if (playerHand.Count == 2)
                    {
                        Card newCard = deck.TakeNext();
                        playerHand.Add(newCard);
                        ConCommand.Say("[Baccarat] Player draws a " + newCard.GetName + " (Total: " + GetTotal(playerHand) + ")");
                    }
                    else if (bankerStood)
                    {
                        if (playerTotal > bankerTotal)
                        {
                            ConCommand.Say("[Baccarat] (PLAYER WIN) Player's " + playerTotal + " beats Banker's " + bankerTotal);
                        }
                        else if (playerTotal < bankerTotal)
                        {
                            ConCommand.Say("[Baccarat] (BANKER WIN) Banker's " + bankerTotal + " beats Player's " + playerTotal);
                        }
                        else
                        {
                            ConCommand.Say("[Baccarat] (TIE) Player's " + playerTotal + " is matched by the Banker");
                        }
                    }
                    else
                    {
                        int third = cardValues[playerHand[2].Rank];
                        bankerStood = true;

                        if(third == 2 || third == 3)
                        {
                            if(bankerTotal <= 4)
                            {
                                Card newCard = deck.TakeNext();
                                bankerHand.Add(newCard);
                                ConCommand.Say("[Baccarat] Banker draws a |" + newCard.GetName + "| (Total: " + GetTotal(bankerHand) + ")");
                            }
                            else
                            {
                                ConCommand.Say("[Baccarat] Banker stands at " + bankerTotal);
                            }
                        }
                        else if(third == 4 || third == 5)
                        {
                            if (bankerTotal <= 5)
                            {
                                Card newCard = deck.TakeNext();
                                bankerHand.Add(newCard);
                                ConCommand.Say("[Baccarat] Banker draws a |" + newCard.GetName + "| (Total: " + GetTotal(bankerHand) + ")");
                            }
                            else
                            {
                                ConCommand.Say("[Baccarat] Banker stands at " + bankerTotal);
                            }
                        }
                        else if(third == 6 || third == 7)
                        {
                            if (bankerTotal <= 6)
                            {
                                Card newCard = deck.TakeNext();
                                bankerHand.Add(newCard);
                                ConCommand.Say("[Baccarat] Banker draws a |" + newCard.GetName + "| (Total: " + GetTotal(bankerHand) + ")");
                            }
                            else
                            {
                                ConCommand.Say("[Baccarat] Banker stands at " + bankerTotal);
                            }
                        }
                        else if(third == 8)
                        {
                            if (bankerTotal <= 2)
                            {
                                Card newCard = deck.TakeNext();
                                bankerHand.Add(newCard);
                                ConCommand.Say("[Baccarat] Banker draws a |" + newCard.GetName + "| (Total: " + GetTotal(bankerHand) + ")");
                            }
                            else
                            {
                                ConCommand.Say("[Baccarat] Banker stands at " + bankerTotal);
                            }
                        }
                        else
                        {
                            if (bankerTotal <= 3)
                            {
                                Card newCard = deck.TakeNext();
                                bankerHand.Add(newCard);
                                ConCommand.Say("[Baccarat] Banker draws a |" + newCard.GetName + "| (Total: " + GetTotal(bankerHand) + ")");
                            }
                            else
                            {
                                ConCommand.Say("[Baccarat] Banker stands at " + bankerTotal);
                            }
                        }
                    }
                }
            }
        }
    }

    public void InitializeActions()
    {
        Actions[Key.O] = () => GameStart();

        Actions[Key.I] = () => AdvanceGame();

    }

    public void MainLoop()
    {

    }
}