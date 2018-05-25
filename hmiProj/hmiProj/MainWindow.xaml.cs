using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using CircularGauge;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace hmiProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int gear = 0;
        static double fuel = 60;
        static double speed = 0;
        static double spalanie = 0;
        static double przebieg = 10000.00;
        static private int wsp_x = 0;

        ObservableDataSource<Point> source1 = null;
        ObservableDataSource<Point> source2 = null;
        ObservableDataSource<Point> source3 = null;

        public MainWindow()
        {
            
            InitializeComponent();
            fuelGauge.CurrentValue = fuel;
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(1000);
            timer.Start();
            timer.Enabled = true;
            timer.Elapsed += TimerElapsedEvent;
            timer.Elapsed += TempSilnikaEvent;
            timer.Elapsed += PrzebiegEvent;          
        }
            

        private void TimerElapsedEvent(object sender, EventArgs e)
        {
            //zegar
            bool uiAccess = label.Dispatcher.CheckAccess();

            if (uiAccess)
                label.Content = DateTime.Now.TimeOfDay;
            else
                label.Dispatcher.Invoke(() => { label.Content = DateTime.Now.ToLongTimeString(); });


            // wskaznik paliwa

            bool uiAccessFuel = fuelGauge.Dispatcher.CheckAccess();
            if (uiAccess)
            {
                fuel = fuel - (spalanie / 50); //duza wartosc aby pokazac zmiane w stanie paliwa
                fuelGauge.CurrentValue = fuel;

            }
            else
            {
                fuel = fuel - (spalanie / 50); //duza wartosc aby pokazac zmiane w stanie paliwa
                fuelGauge.Dispatcher.Invoke(() => { fuelGauge.CurrentValue = fuel; });
            }
        }

        static int tempSilnika = 30;
        private void TempSilnikaEvent(object sender, EventArgs e)
        {
            if (speed > 0)
            {
                if (tempSilnika < 89)
                    tempSilnika = tempSilnika + 1;
                else if (tempSilnika > 89)
                {
                    if (speed > 140)
                    {
                        tempSilnika = tempSilnika + 1;
                        if (tempSilnika > 111)
                            tempSilnika--;
                    }
                    else
                        tempSilnika = tempSilnika - 1;
                }
                else if (tempSilnika == 89)
                {
                    if (speed > 140)
                        tempSilnika = tempSilnika + 1;
                }
            }
            else if (tempSilnika > 30)
                tempSilnika -= 1;

            bool uiAccess = engineTemp.Dispatcher.CheckAccess();
            if (uiAccess)
                engineTemp.Content = "Temp. :" + tempSilnika.ToString() + "°C";
            else
                label.Dispatcher.Invoke(() => { engineTemp.Content = "Temp. :" + tempSilnika.ToString() + "°C"; });
        }     

        private void PrzebiegEvent(object sender, EventArgs e)
        {
            
            if (speed > 0)
            {
                przebieg = przebieg + (speed/3600); //przebieg w realnym tempie
            }

            int wyswietl = Convert.ToInt32(przebieg);

            bool uiAccess = przebiegWindow.Dispatcher.CheckAccess();
            if (uiAccess)
                przebiegWindow.Content =  wyswietl.ToString() + "km";
            else
                przebiegWindow.Dispatcher.Invoke(() => { przebiegWindow.Content = wyswietl.ToString() + "km"; });
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Chur.AddLineGraph(speed, 2, null);
            Speed.CurrentValue = slider.Value;
            speed = slider.Value;
            if (Speed.CurrentValue > 60 && Speed.CurrentValue < 120)
                Speed.GaugeBackgroundColor = Colors.Green;
            else if (Speed.CurrentValue >= 120)
                Speed.GaugeBackgroundColor = Colors.Red;
            else
                Speed.GaugeBackgroundColor = Colors.Blue;

            double value = slider.Value;
            value = value / 3.0;
//------------------------------------ biegi
            switch (gear)
            {
                case 0:
                    gear++;
                    break;

                case 1:
                    value = value * 6.5;
                    myGauge1.CurrentValue = value;
                    if (myGauge1.CurrentValue > 32)
                        gear++;
                    
                    break;

                case 2:
                    value = value * 3;
                    myGauge1.CurrentValue = value;
                    if (myGauge1.CurrentValue > 32)
                        gear++;
                    else if (myGauge1.CurrentValue < 15)
                        gear--;

                    break;

                case 3:
                    value = value * 2.2;
                    myGauge1.CurrentValue = value;
                    if (myGauge1.CurrentValue > 32)
                        gear++;
                    else if (myGauge1.CurrentValue < 15)
                        gear--;

                    break;

                case 4:
                    value = value * 1.6;
                    myGauge1.CurrentValue = value;
                    if (myGauge1.CurrentValue > 32)
                        gear++;
                    else if (myGauge1.CurrentValue < 15)
                        gear--;
                    break;

                case 5:
                    value = value * 1.0;
                    myGauge1.CurrentValue = value;
                    if (myGauge1.CurrentValue < 15)
                        gear--;
                    break;
            }
            //--------------------koniec biegi
            //spalanie
            if (gear == 0 || myGauge1.CurrentValue == 0)
            {
                fuelConsume.Content = "0.00 l/100km";
                spalanie = 0.0;
            }
            else
            {
                double fuelValue = (myGauge1.CurrentValue / 10) + 3.0;
                fuelValue = fuelValue * 100;
                int tmp = Convert.ToInt32(fuelValue);
                fuelValue = Convert.ToDouble(tmp);
                fuelValue = fuelValue / 100.00;
                spalanie = fuelValue;
                fuelConsume.Content = fuelValue.ToString() + " l/100km";
            }
            //koniec spalanie
        }

        private void espControl_Checked(object sender, RoutedEventArgs e)
        {
            espImage.Opacity = 1;
        }

        private void espControl_Unchecked(object sender, RoutedEventArgs e)
        {
            espImage.Opacity = 0.2;
        }

        private void accumulatorControl_Checked(object sender, RoutedEventArgs e)
        {
            batteryImage.Opacity = 1;
        }

        private void accumulatorControl_Unchecked(object sender, RoutedEventArgs e)
        {
            batteryImage.Opacity = 0.2;
        }

        private void oilControl_Checked(object sender, RoutedEventArgs e)
        {
            oilImage.Opacity = 1;
        }

        private void oilControl_Unchecked(object sender, RoutedEventArgs e)
        {
            oilImage.Opacity = 0.2;
        }

        private void absControl_Checked(object sender, RoutedEventArgs e)
        {
            imageAbs.Opacity = 1;
        }

        private void absControl_Unchecked(object sender, RoutedEventArgs e)
        {
            imageAbs.Opacity = 0.2;
        }

        private void Wykres_Loaded(object sender, RoutedEventArgs e)
        {
            source1 = new ObservableDataSource<Point>();
            source1.SetXYMapping(p => p);
            Chart.AddLineGraph(source1, 1, "Speed"); ;

            source2 = new ObservableDataSource<Point>();
            source2.SetXYMapping(p => p);
            Chart.AddLineGraph(source2, 1, "Fuel"); ;

            source3 = new ObservableDataSource<Point>();
            source3.SetXYMapping(p => p);
            Chart.AddLineGraph(source3, 1, "Temp."); ;   

            Thread simThread = new Thread(new ThreadStart(Simulation));
            simThread.IsBackground = true;
            simThread.Start();
        }

        private void Simulation()
        {
            while (true)
            {
                if (speed > 0)
                {
                    while (true)
                    {
                        double x = Convert.ToDouble(wsp_x); 
                        double y1 = speed;
                        double y2 = spalanie;
                        double y3 = tempSilnika;
                        Point p1 = new Point(x, y1);
                        Point p2 = new Point(x, y2);
                        Point p3 = new Point(x, y3);
                        source1.AppendAsync(Dispatcher, p1);
                        source2.AppendAsync(Dispatcher, p2);
                        source3.AppendAsync(Dispatcher, p3);
                        wsp_x++;
                        Thread.Sleep(100);
                    }
                }
            }
        }

        //private void ListBox1_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //RecognizeSpeech();
        //}

        ////static SpeechRecognitionEngine _recognizer = null;
        //SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        //void RecognizeSpeech()
        //{
        //    SpeechRecognizer recognizer = new SpeechRecognizer();

        //    GrammarBuilder grammarBuilder = new GrammarBuilder();
        //    Choices commandChoices = new Choices("weight", "color", "size");
        //    grammarBuilder.Append(commandChoices);

        //    recognizer.LoadGrammar(new Grammar(grammarBuilder));
        //    //_recognizer.LoadGrammar(new DictationGrammar());
        //   // _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("activate")));
        //   // _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("deactivate")));
        //    recognizer.SpeechRecognized += _recognizeSpeech_SpeechRecognized;
        //    recognizer.SpeechRecognitionRejected += _recognizeSpeech_SpeechRecognitionRejected;
        //   // _recognizer.SetInputToDefaultAudioDevice();
        //    //_recognizer.RecognizeAsync(RecognizeMode.Multiple);
        //}
        //void
        //_recognizeSpeech_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    if
        //    (e.Result.Text == "activate")
        //    {
        //        this
        //        .listBox1.Items.Add(">SpeechRecognitionEngine: AUTOPILOT START!");
        //        speechSynthesizer.Speak("Initiating automatic driving mode");
        //    }
        //    else if (e.Result.Text == "deactivate")
        //    {
        //        this.listBox1.Items.Add(">SpeechRecognitionEngine: AUTOPILOT STOP!");
        //        speechSynthesizer.Speak("Deactivating automatic driving mode");
        //    }
        //    else
        //    {
        //        this.listBox1.Items.Add(">SpeechRecognitionEngine: "+ e.Result.Text);
        //    }
        //}

        //void _recognizeSpeech_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        //{
        //    this.listBox1.Items.Add(">SpeechRecognitionEngine: Unrecognized command...");
        //}
    }
}
