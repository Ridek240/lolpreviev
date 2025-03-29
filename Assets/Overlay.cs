using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class OverlayManager : MonoBehaviour
{
    private const int HWND_TOPMOST = -1;
    private const int HWND_NOTOPMOST = -2;
    private const int SWP_NOMOVE = 0x0002;
    private const int SWP_NOSIZE = 0x0001;
    private const int SWP_SHOWWINDOW = 0x0040;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    private void Start()
    {
        IntPtr windowHandle = GetActiveWindow(); // Pobiera uchwyt okna Unity
        SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
    }
}