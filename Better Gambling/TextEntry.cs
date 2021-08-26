using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

public class TextEntry
{
    public string textEntry = "";

    public TextEntry(IGambleGame game)
    {
        game.Actions[Key.Back] = () => Reset();

        game.Actions[Key.D0] = () => Add("0");
        game.Actions[Key.D1] = () => Add("1");
        game.Actions[Key.D2] = () => Add("2");
        game.Actions[Key.D3] = () => Add("3");
        game.Actions[Key.D4] = () => Add("4");
        game.Actions[Key.D5] = () => Add("5");
        game.Actions[Key.D6] = () => Add("6");
        game.Actions[Key.D7] = () => Add("7");
        game.Actions[Key.D8] = () => Add("8");
        game.Actions[Key.D9] = () => Add("9");
    }

    public int Read()
    {
        int output = -1;

        if (int.TryParse(textEntry, out output))
        {
            Reset();
            return output;
        }
        else
        {
            return -1;
        }
    }

    void Add(string text)
    {
        textEntry += text;
    }

    void Reset()
    {
        textEntry = "";
    }
}