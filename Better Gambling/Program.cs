using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            Console.WriteLine("Error setting clipboard: " + e.Message);
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
    static async Task Main(string[] args)
    {
        string cfgPath = await LoadOrPromptForPathAsync();
        ShowInitialInstructions();

        List<IGambleGame> games = InitializeGames();

        while (true)
        {
            IGambleGame selectedGame = SelectGame(games);
            selectedGame.InitializeActions();
            Dictionary<Key, bool> keyPressed = InitializeKeyPressStates(selectedGame);

            selectedGame.GameStart();
            await RunGameLoopAsync(selectedGame, keyPressed);
        }
    }

    private static async Task<string> LoadOrPromptForPathAsync()
    {
        const string defaultPath = @"C:\Program Files (x86)\Steam\steamapps\common\GarrysMod\garrysmod\cfg";
        string path = ConCommand.cfgPath;

        if (!Directory.Exists(path))
        {
            if (File.Exists("gamblepath.txt"))
            {
                path = File.ReadAllText("gamblepath.txt");
            }
            else
            {
                Console.WriteLine($"Note: The default path is {defaultPath}");
                Console.Write("Enter gmod cfg path: ");
                path = Console.ReadLine();
                File.WriteAllText("gamblepath.txt", path);
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine("Invalid path. Please try again.");
                return await LoadOrPromptForPathAsync();
            }

            ConCommand.cfgPath = path;
            Console.WriteLine("Path successfully loaded! (If the path is incorrect, please delete gamblepath.txt in the root of this folder)");
        }
        else
        {
            Console.WriteLine("Using default gmod path");
        }

        return path;
    }

    private static void ShowInitialInstructions()
    {
        Console.WriteLine("WARNING, READ BELOW");
        Console.WriteLine("Make sure you type 'bind b \"exec gamblecmd\"' into the gmod console if this is your first time using this script");
    }

    private static List<IGambleGame> InitializeGames()
    {
        return new List<IGambleGame>
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
    }

    private static IGambleGame SelectGame(List<IGambleGame> games)
    {
        while (true)
        {
            Console.WriteLine("Select a game by entering its ID:");
            for (int i = 0; i < games.Count; i++)
            {
                Console.WriteLine($"{i}: {games[i].GetName}");
            }

            if (int.TryParse(Console.ReadLine(), out int selected) && selected >= 0 && selected < games.Count)
            {
                IGambleGame selectedGame = games[selected];
                Console.WriteLine($"Selected: {selectedGame.GetName}");
                Console.WriteLine(selectedGame.GetDescription);
                return selectedGame;
            }
            else
            {
                Console.WriteLine("Invalid selection. Try again.");
            }
        }
    }

    private static Dictionary<Key, bool> InitializeKeyPressStates(IGambleGame selectedGame)
    {
        var keyPressed = new Dictionary<Key, bool>();
        foreach (var action in selectedGame.Actions)
        {
            keyPressed[action.Key] = false;
        }
        return keyPressed;
    }

    private static async Task RunGameLoopAsync(IGambleGame selectedGame, Dictionary<Key, bool> keyPressed)
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
            await Task.Delay(10);
        }
    }
}
