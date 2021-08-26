using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

public interface IGambleGame
{
    string GetName { get; }
    string GetDescription { get; }
    void MainLoop();
    Dictionary<Key, Action> Actions { get; }
    void InitializeActions();
    void GameStart();
}

class Gambling
{
    public static void SetClipboard(string toSet)
    {
        try
        {
            Clipboard.SetText(toSet);
        }
        catch (Exception e)
        {
            Console.WriteLine("Caught: " + e.Message);
        }
    }

    public static string FormatMoney(int money)
    {
        return string.Format(new CultureInfo("en-US"), "{0:C0}", money);
    }

    public static string FormatMoney(string money)
    {
        return string.Format(new CultureInfo("en-US"), "{0:C0}", money);
    }

    [STAThread]
    static void Main(string[] args)
    {
        invalidpath:;
        if (Directory.Exists(ConCommand.cfgPath))
        {
            Console.WriteLine("Using default gmod path");
        }
        else
        {
            if (File.Exists("gamblepath.txt"))
            {
                string path = File.ReadAllText("gamblepath.txt");

                if (Directory.Exists(path))
                {
                    ConCommand.cfgPath = path;
                    Console.WriteLine("Path successfully loaded! (If the path is incorrect, please delete gamblepath.txt in the root of this folder)");
                }
                else
                {
                    Console.WriteLine("Invalid path found... please enter again");
                    File.Delete("gamblepath.txt");
                    goto invalidpath;
                }
            }
            else
            {
                Console.WriteLine(@"note: The default path is C:\Program Files (x86)\Steam\steamapps\common\Garry's Mod\garrysmod\cfg");
                Console.Write("Enter gmod cfg path: ");
                File.WriteAllText("gamblepath.txt", Console.ReadLine());
                goto invalidpath;
            }
        }

        Console.WriteLine("WARNING, READ BELOW");
        Console.WriteLine("Make sure you type 'bind b \"exec gamblecmd\"' into the gmod console if this is your first time using this script");

        List<IGambleGame> games = new List<IGambleGame>()
        {
            new Blackjack(),
            new Dice(),
            new DoubleOrNothing(),
            new Roulette(),
            new ScratchCards(),
            new MysteryBox(),
            new VideoPoker(),
            new Craps(),
            new Baccarat(),
            new SixShooter(),
            new ThreeCardPoker(),
            new InstantInvestments(),
            new HiLo()
        };

        gameSelect:;

        IGambleGame selectedGame = null;

        for (int i = 0; i < games.Count; i++)
        {
            Console.Write(i + ": " + games[i].GetName);

            if (i != games.Count - 1)
            {
                Console.Write(", ");
            }
        }

        Console.WriteLine();

        int selected = -1;
        while (selectedGame == null)
        {
            Console.Write("Enter Game ID: ");

            if (int.TryParse(Console.ReadLine(), out selected))
            {
                if (selected >= 0 && selected < games.Count)
                {
                    selectedGame = games[selected];

                    Console.WriteLine("Selected: " + selectedGame.GetName);
                    Console.WriteLine(selectedGame.GetDescription);

                    break;
                }
            }

            Console.WriteLine("Try again...");
        }

        selectedGame.InitializeActions();

        Dictionary<Key, bool> keyPressed = new Dictionary<Key, bool>();

        foreach (KeyValuePair<Key, Action> action in selectedGame.Actions)
        {
            keyPressed[action.Key] = false;
        }

        selectedGame.GameStart();

        while (true)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                foreach (KeyValuePair<Key, Action> action in selectedGame.Actions)
                {
                    if (Keyboard.IsKeyDown(action.Key))
                    {
                        if (!keyPressed[action.Key])
                        {
                            keyPressed[action.Key] = true;

                            action.Value();
                        }
                    }
                    else
                    {
                        keyPressed[action.Key] = false;
                    }
                }

                if (Keyboard.IsKeyDown(Key.Delete))
                {
                    Console.Clear();
                    goto gameSelect;
                }
            }
            selectedGame.MainLoop();
        }
    }
}
