using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class MysteryBox : IGambleGame
{
    public string GetName => "Mystery Box";

    public string GetDescription => "ALT+I To generate a box";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    int jackpot = -1;
    int rarity = 2500;
    static int current = 0;

    int Generate()
    {
        Random rand = new Random();
        int random = rand.Next(1, rarity);

        current++;

        if(random == 1)
        {
            ConCommand.Say("[Mystery Box #" + current + " You won the Jackpot: " + Gambling.FormatMoney(jackpot));
            return jackpot;
        }
        else
        {
            int value = 0;
            random = rand.Next(1, rarity);

            if(random <= 2)
            {
                value = (int)((double)jackpot * 0.35);
                ConCommand.Say("[Mystery Box #" + current + "] You win: " + Gambling.FormatMoney(value));
                return value;
            }
            else if(random < (rarity / 75))
            {
                value = (int)((double)jackpot * 0.075);
                ConCommand.Say("[Mystery Box #" + current + "] You win: " + Gambling.FormatMoney(value));
                return value;
            }
            else if(random < (rarity / 40))
            {
                value = (int)((double)jackpot * 0.05);
                ConCommand.Say("[Mystery Box #" + current + "] You win: " + Gambling.FormatMoney(value));
                return value;
            }
            else if(random < (rarity / 15))
            {
                value = (int)((double)jackpot * 0.01);
                ConCommand.Say("[Mystery Box #" + current + "] You win: " + Gambling.FormatMoney(value));
                return value;
            }
            else if(random < (rarity / 10))
            {
                value = (int)((double)jackpot * 0.0075);
                ConCommand.Say("[Mystery Box #" + current + "] You win: " + Gambling.FormatMoney(value));
                return value;
            }

            value = (int)((float)jackpot * 0.005);
            ConCommand.Say("[Mystery Box #" + current + "] You win: " + Gambling.FormatMoney(value));
            return value;
        }
    }

    public void GameStart()
    {
        while(jackpot == -1)
        {
            Console.Write("Enter Jackpot: ");
            if(int.TryParse(Console.ReadLine(), out jackpot))
            {
                Console.WriteLine("Jackpot Set To: " + Gambling.FormatMoney(jackpot));
            }
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