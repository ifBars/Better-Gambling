using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class TexasHoldem : IGambleGame
{
    public string GetName => "Texas Hold'em";

    public string GetDescription => "ALT+I to Advance/Check/Call, ALT+O to Reset, ALT+L to Fold, ALT+K to Raise, ALT+(0-9) for Number Entry, ALT+BACKSPACE to Reset Number Entry";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    string textEntry = "";

    public void GameStart()
    {
        
    }

    void AdvanceGame()
    {

    }

    void Fold()
    {

    }

    void Raise()
    {

    }

    int InputNumber()
    {
         if (int.TryParse(textEntry, out int output))
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

    public void InitializeActions()
    {
        Actions[Key.O] = () => GameStart();

        Actions[Key.I] = () => AdvanceGame();

        Actions[Key.L] = () => Fold();

        Actions[Key.K] = () => Raise();

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