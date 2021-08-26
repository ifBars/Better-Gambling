using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

static class ConCommand
{
    public static string cfgPath = @"C:\Program Files (x86)\Steam\steamapps\common\GarrysMod\garrysmod\cfg";

    public static void Run(string cmd, bool delay = true)
    {
        File.WriteAllText(cfgPath + "\\gamblecmd.cfg", cmd);
        Macro.Send(VirtualKeyCode.VK_B, delay);
    }

    public static void Say(string cmd, bool delay = true)
    {
        Run("say " + cmd, delay);
    }
}