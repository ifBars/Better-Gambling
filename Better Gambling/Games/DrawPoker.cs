using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

class DrawPoker : IGambleGame
{
    public string GetName => "5 Card Draw Poker";

    public string GetDescription => "WIP";

    public Dictionary<Key, Action> Actions { get; private set; } = new Dictionary<Key, Action>();

    int totalPlayers = 0;

    void AdvanceGame()
    {

    }

    public void GameStart()
    {
        ConCommand.Say("[Poker] Game Started, enter number of players");
    }

    public void InitializeActions()
    {
        Actions[Key.O] = () => GameStart();

        Actions[Key.I] = () => AdvanceGame();
    }

    public void MainLoop()
    {
        
    }
}