//using System;
//using System.Threading;
//using System.Windows;
////using System.Windows.Forms;
//using System.Windows.Controls;
//using System.Windows.Media; 
//using System.Windows.Input;
//using UnityEngine;

//public class SettingsWindow : MonoBehaviour
//{
//    private Thread uiThread;

//    private void Start()
//    {
//        uiThread = new Thread(OpenWpfWindow);
//        uiThread.SetApartmentState(ApartmentState.STA); // Konieczne dla WPF
//        uiThread.Start();
//    }

//    private void OpenWpfWindow()
//    {
//        System.Windows.Application app = new System.Windows.Application();
//        app.Run(new SettingsWpfWindow());
//    }

//    private void OnApplicationQuit()
//    {
//        if (uiThread != null && uiThread.IsAlive)
//        {
//            uiThread.Abort();
//        }
//    }
//}

//public class SettingsWpfWindow : Window
//{
//    public SettingsWpfWindow()
//    {
//        Title = "Ustawienia";
//        Width = 400;
//        Height = 300;

//        Button submitButton = new Button();
//        submitButton.Content = "ZatwierdŸ";
//        submitButton.Click += (sender, args) => MessageBox.Show("Zatwierdzone!");

//        this.Content = submitButton;
//    }
//}