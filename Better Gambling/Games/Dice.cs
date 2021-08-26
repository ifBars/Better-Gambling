using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class Dice : IGambleGame
{
    public string GetName => "Dice";

    public string GetDescription => "ALT+I to Roll";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    int diceToRoll = -1;

    public void GameStart()
    {
        while (diceToRoll == -1)
        {
            Console.Write("How many dice do you want to roll: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out diceToRoll))
            {
                Console.WriteLine("You will be rolling " + diceToRoll + " dice!");
            }
        }
    }

    public void InitializeActions()
    {
        Actions[Key.I] = () =>
        {
            Random rand = new Random();
            int total = 0;
            for (int i = 0; i < diceToRoll; i++)
            {
                total += rand.Next(1, 7);
            }

            ConCommand.Say("[Dice] You have rolled " + diceToRoll + " dice: " + total);
        };
    }

    public void MainLoop()
    {

    }
}
