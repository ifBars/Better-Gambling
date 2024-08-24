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
        InitializeConfigurationPath();

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

        while (true)
        {
            IGambleGame selectedGame = SelectGame(games);
            selectedGame.InitializeActions();

            Dictionary<Key, bool> keyPressed = selectedGame.Actions.Keys.ToDictionary(key => key, key => false);

            selectedGame.GameStart();
            GameLoop(selectedGame, keyPressed);
        }
    }

    private static void InitializeConfigurationPath()
    {
        const string defaultPath = @"C:\Program Files (x86)\Steam\steamapps\common\GarrysMod\garrysmod\cfg";
        string path = string.Empty;

        while (true)
        {
            if (Directory.Exists(ConCommand.cfgPath))
            {
                Console.WriteLine("Using default gmod path");
                break;
            }
            else if (File.Exists("gamblepath.txt"))
            {
                path = File.ReadAllText("gamblepath.txt");

                if (Directory.Exists(path))
                {
                    ConCommand.cfgPath = path;
                    Console.WriteLine("Path successfully loaded! (If the path is incorrect, please delete gamblepath.txt in the root of this folder)");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid path found... please enter again");
                    File.Delete("gamblepath.txt");
                }
            }
            else
            {
                Console.WriteLine($"Note: The default path is {defaultPath}");
                Console.Write("Enter gmod cfg path: ");
                File.WriteAllText("gamblepath.txt", Console.ReadLine());
            }
        }
    }

    private static IGambleGame SelectGame(List<IGambleGame> games)
    {
        while (true)
        {
            Console.WriteLine("Available Games:");
            for (int i = 0; i < games.Count; i++)
            {
                Console.Write($"{i}: {games[i].GetName}");
                if (i != games.Count - 1) Console.Write(", ");
            }

            Console.WriteLine();
            Console.Write("Enter Game ID: ");

            if (int.TryParse(Console.ReadLine(), out int selected) && selected >= 0 && selected < games.Count)
            {
                IGambleGame selectedGame = games[selected];
                Console.WriteLine($"Selected: {selectedGame.GetName}");
                Console.WriteLine(selectedGame.GetDescription);
                return selectedGame;
            }

            Console.WriteLine("Invalid input. Try again...");
        }
    }

    private static void GameLoop(IGambleGame selectedGame, Dictionary<Key, bool> keyPressed)
    {
        while (true)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                foreach (var action in selectedGame.Actions)
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
                    break;
                }
            }

            selectedGame.MainLoop();
            Thread.Sleep(10);
        }
    }
}
