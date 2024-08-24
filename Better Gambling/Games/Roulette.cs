using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class Roulette : IGambleGame
{
    public string GetName => "Roulette";

    public string GetDescription => "ALT+I to spin";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    public void GameStart()
    {
        
    }

    private Random rand = new Random();

    public void InitializeActions()
    {
        Actions[Key.I] = () =>
        {
            int result = rand.Next(0, 37);
            string colour = "green";

            if(result > 0)
            {
                colour = result % 2 == 0 ? "red" : "black";
            }

            ConCommand.Say("[Roulette] Result: " + result + " (" + colour + ")");
        };
    }

    public void MainLoop()
    {
        
    }
}