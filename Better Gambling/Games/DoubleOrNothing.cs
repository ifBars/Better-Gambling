using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class DoubleOrNothing : IGambleGame
{
    public string GetName => "Double or Nothing";

    public string GetDescription => "ALT+O to Reset, ALT+I to Double";

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

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    string textEntry = "";
    int total = -1;
    bool isBust = false;
    int[] chanceForBust = { 55, 100 }; // 55 in 100 chance of going bust

    public void GameStart()
    {
        
    }

    public void Reset()
    {
        isBust = false;
        total = InputNumber();
        if (total > 0)
        {
            ConCommand.Say("[Double or Nothing] Game Started: " + Gambling.FormatMoney(total));
        }
        else
        {
            ConCommand.Say("[Double or Nothing] Invalid number entered.");
        }
    }

    public void Double()
    {
        if (!isBust)
        {
            Random rand = new Random();

            if(rand.Next(1, chanceForBust[1] + 1) <= chanceForBust[0])
            {
                isBust = true;
                ConCommand.Say("[Double or Nothing] (BUST): " + Gambling.FormatMoney(total) + "!");
            }
            else
            {
                total *= 2;
                ConCommand.Say("[Double or Nothing] (DOUBLE): " + Gambling.FormatMoney(total) + "!");
            }
        }
    }

    public void InitializeActions()
    {
        Actions[Key.O] = () => Reset();
        Actions[Key.I] = () => Double();

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