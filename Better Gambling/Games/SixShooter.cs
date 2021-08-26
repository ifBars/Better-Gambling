using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class SixShooter : IGambleGame
{
    public string GetName => "Six Shooter";

    public string GetDescription => "ALT+O to Reset, ALT+I to Advance";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    List<int> dealerDice;
    List<int> playerDice;

    bool displayRemaining;

    public void GameStart()
    {
        dealerDice = new List<int>();
        playerDice = new List<int>();
        displayRemaining = false;

        ConCommand.Say("[Six Shooter] Game Started");
    }

    void AdvanceGame()
    {
        Random rand = new Random();

        if (dealerDice.Count == 0)
        {
            if (playerDice.Count > 0)
            {
                switch (playerDice.Count/3)
                {
                    case 1:
                        ConCommand.Say("[Six Shooter] (PLAYER WIN) 12x bet");
                        break;
                    case 2:
                        ConCommand.Say("[Six Shooter] (PLAYER WIN) 2.6x bet");
                        break;
                    case 3:
                        ConCommand.Say("[Six Shooter] (PLAYER WIN) 2x bet");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                string result = "[Six Shooter] Dealer Dice: ";
                for (int i = 0; i < 6; i++)
                {
                    dealerDice.Add(rand.Next(1, 7));

                    result += "" + dealerDice[i];

                    if (i != 5)
                    {
                        result += "-";
                    }
                }
                ConCommand.Say(result);
            }
        }
        else if (displayRemaining)
        {
            displayRemaining = false;

            string result = "";
            for (int i = 0; i < dealerDice.Count; i++)
            {
                result += "" + dealerDice[i];

                if (i != dealerDice.Count - 1)
                {
                    result += "-";
                }
            }
            ConCommand.Say("[Six Shooter] Dealer Dice: " + result);
        }
        else
        {
            if(playerDice.Count == 9)
            {
                string result = "";
                for(int i = 0;i<dealerDice.Count;i++)
                {
                    result += "" + dealerDice[i];

                    if(i != dealerDice.Count - 1)
                    {
                        result += "-";
                    }
                }

                ConCommand.Say("[Six Shooter] (DEALER WIN) Dice remaining: " + result);
            }
            else
            {
                string result = "";
                for (int i = 0; i < 3; i++)
                {
                    int dice = rand.Next(1, 7);

                    result += "" + dice;

                    playerDice.Add(dice);

                    if (dealerDice.Contains(dice))
                    {
                        for (int j = dealerDice.Count - 1; j >= 0; j--)
                        {
                            if (dealerDice[j] == dice)
                            {
                                dealerDice.RemoveAt(j);
                            }
                        }
                    }

                    if(i != 2)
                    {
                        result += "-";
                    }
                }

                displayRemaining = true;

                ConCommand.Say("[Six Shooter] Player rolls: " + result + " (" + (3-(playerDice.Count/3)) + " remaining)");
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
