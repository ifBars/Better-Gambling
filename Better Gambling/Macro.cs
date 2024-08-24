using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

static class Macro
{
    public static void Send(VirtualKeyCode key, bool delay = true)
    {
        InputSimulator sim = new InputSimulator();
        sim.Keyboard.KeyPress(key);

        if(delay)
            Thread.Sleep(600);
    }
}