using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PGBox.IoTCore.Sensor.Distance;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HCSR04sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int TRIG_GPIO = 18;
        private const int ECHO_GPIO = 12;

        private DispatcherTimer timer;

        HCSR04 distanceSensor = new HCSR04();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int result = distanceSensor.Init(18, 12);
            if (result != 0)
                return;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Timer_Tick(object sender, object e)
        {
            distanceText.Text = String.Format("Distance : {0:f} cm", distanceSensor.GetDistance());
        }
    }
}
