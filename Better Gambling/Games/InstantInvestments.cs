using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class InstantInvestments : IGambleGame
{
    public string GetName => "Instant Investments";

    public string GetDescription => "ALT+(0-9) for number entry, ALT+BACKSPACE to reset number entry, ALT+I to generate, ALT+O to change deviation";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    int deviation = 50;
    string textEntry = "";

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

    void SetDeviation()
    {
        int input = InputNumber();
        if (input > 0)
        {
            deviation = input;
            ConCommand.Say("Deviation set to " + deviation + "%");
        }
    }

    public void GameStart()
    {

    }

    public void InitializeActions()
    {
        Actions[Key.I] = () =>
        {
            int currentAmount = InputNumber();

            if (currentAmount <= -1)
            {
                ConCommand.Say("Incorrect number entered");
                return;
            }

            Random rand = new Random();

            int multiplier = 1;
            string sign = "+";

            if (rand.Next(1, 101) > 45)
            {
                multiplier = -1;
                sign = "-";
            }

            int deviatedBy = rand.Next(0, deviation + 1) * multiplier;

            currentAmount += (int)(currentAmount * ((float)deviatedBy / 100.0f));

            deviatedBy = Math.Abs(deviatedBy);

            ConCommand.Say("[Investment] Investment returned: " + Gambling.FormatMoney(currentAmount) + " (" + sign + deviatedBy + "%)");
        };

        Actions[Key.O] = () => SetDeviation();

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