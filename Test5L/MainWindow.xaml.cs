using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Test5L
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            //Initialisation de le fenêtre
            InitializeComponent();

            //on crée un dispatcher (semblade à un thread, mais pour les fenêtres graphiques) 
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1); // on choisi le temps entre chaque appel de la fonction
            timer.Tick += timer_Tick; // on lance la méthode "timer_tick" à chaque tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // on modifie la valeur du label en fonction de la distance de l'utilisateur par rapport à l'écran.
            LabelOriginZ.Content = Math.Round(((App)Application.Current).GetEyetracker().GetEyetrackerOriginZ(),0); 
        }
    }
}
