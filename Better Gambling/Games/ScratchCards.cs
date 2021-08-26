using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class ScratchCards : IGambleGame
{
    public string GetName => "Scratch Cards";

    public string GetDescription => "ALT+I to generate";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    int jackpot = -1;
    Dictionary<int, int> possibilities = new Dictionary<int, int>();

    static int currentID = 1;

    int Generate()
    {
        string result = "[Scratch Card #" + currentID + "] | ";
        currentID++;

        int[] payout = new int[3];

        Random rand = new Random();

        for(int i = 0; i < payout.Length; i++)
        {
            int highest = 0;
            int lowest = jackpot;
            foreach(KeyValuePair<int, int> prize in possibilities)
            {
                int value = prize.Key;
                int chance = prize.Value;
                int random = rand.Next(0, chance);

                if(lowest > value)
                {
                    lowest = value;
                }

                if(random == 0 && value > highest)
                {
                    highest = value;
                }
            }

            // If no prize was won, automatically switch to lowest
            if (highest == 0)
            {
                highest = lowest;
            }

            payout[i] = highest;

            result += Gambling.FormatMoney((int)(payout[i]/1000.0f) * 1000) + " | ";
        }

        ConCommand.Say(result);

        if(payout[0] == payout[1] && payout[1] == payout[2])
        {
            return payout[0];
        }
        else
        {
            return 0;
        }
    }

    public void GameStart()
    {
        while (jackpot == -1)
        {
            Console.Write("Enter Jackpot: $");

            if (int.TryParse(Console.ReadLine(), out jackpot))
            {
                Console.WriteLine("Jackpot Set: " + Gambling.FormatMoney(jackpot));
                Console.WriteLine("Cards Should Be Sold For " + Gambling.FormatMoney(jackpot/100) + " each.");
            }
        }

        int totalPossibilities = 15;
        for (int i = 1; i <= totalPossibilities; i++)
        {
            possibilities[(int)((1.0f/totalPossibilities)*i * jackpot)] = (int)Math.Pow(i, 1.8f);
        }
    }

    public void InitializeActions()
    {
        Actions[Key.I] = () => Generate();
    }

    public void MainLoop()
    {
        
    }
}
