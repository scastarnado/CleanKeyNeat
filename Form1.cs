using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace CleanKeyNeat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Import necessary functions from user32.dll
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static IntPtr hookId = IntPtr.Zero;

        // Function to disable the keyboard by setting up a low-level keyboard hook
        static void DisableKeyboard()
        {
            hookId = SetHook(KeyboardHookProc);
        }

        // Function to enable the keyboard by unhooking the low-level keyboard hook
        static void EnableKeyboard()
        {
            UnhookWindowsHookEx(hookId);
        }

        // Function to set up the low-level keyboard hook
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                // Set up and return the low-level keyboard hook
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        // Function to handle the low-level keyboard hook events
        private static IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                // Block the keypress by not calling the next hook in the chain
                return (IntPtr)1;
            }

            // Continue with the next hook in the chain
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        // Event handler for the button to disable the keyboard
        private void button1_Click(object sender, EventArgs e)
        {
            DisableKeyboard();
        }

        // Event handler for the button to enable the keyboard
        private void button2_Click(object sender, EventArgs e)
        {
            EnableKeyboard();
        }
    }
}
