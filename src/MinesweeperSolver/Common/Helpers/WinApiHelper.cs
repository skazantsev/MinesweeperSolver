using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MinesweeperSolver.Common.Enums;
using MinesweeperSolver.Common.WinApiConstants;

namespace MinesweeperSolver.Common.Helpers
{
    public static class WinApiHelper
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public static void MouseClick(IntPtr hWnd, MouseButton button, int x, int y)
        {
            var lParam = new IntPtr(y * 0x10000 + x);
            IntPtr wParam;
            switch (button)
            {
                case MouseButton.Left:
                    wParam = (IntPtr)MouseKeys.MK_LBUTTON;
                    PostMessage(hWnd, WindowMessages.WM_LBUTTONDOWN, wParam, lParam);
                    PostMessage(hWnd, WindowMessages.WM_LBUTTONUP, wParam, lParam);
                    break;
                case MouseButton.Right:
                    wParam = (IntPtr)MouseKeys.MK_RBUTTON;
                    PostMessage(hWnd, WindowMessages.WM_RBUTTONDOWN, wParam, lParam);
                    PostMessage(hWnd, WindowMessages.WM_RBUTTONUP, wParam, lParam);
                    break;
                default:
                    throw new ArgumentException("Wrong argument for MouseButton!");
            }
        }

        public static Color GetColor(IntPtr hWnd, int x, int y)
        {
            IntPtr hdc = GetDC(hWnd);
            var pixel = GetPixel(hdc, x, y);
            ReleaseDC(hWnd, hdc);
            return Color.FromArgb((int)(pixel & 0x000000FF),
                    (int)(pixel & 0x0000FF00) >> 8,
                    (int)(pixel & 0x00FF0000) >> 16);
        }
    }
}
