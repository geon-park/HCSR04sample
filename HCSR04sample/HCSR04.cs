using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace PGBox.IoTCore.Sensor.Distance
{
    class HCSR04
    {
        private GpioController gpio;
        private GpioPin trigPin;
        private GpioPin echoPin;

        private bool initialized = false;
        public int TimeoutMilliseconds { get; set; }

        public HCSR04()
        {
        }

        public int Init(int trigPinNum, int echoPinNum, int timeoutMilliseconds = 20)
        {
            gpio = GpioController.GetDefault();
            if (null == gpio)
            {
                return 1;
            }

            trigPin = gpio.OpenPin(trigPinNum);
            echoPin = gpio.OpenPin(echoPinNum);

            TimeoutMilliseconds = timeoutMilliseconds;

            trigPin.SetDriveMode(GpioPinDriveMode.Output);
            echoPin.SetDriveMode(GpioPinDriveMode.Input);

            initialized = true;

            return 0;
        }
    
        public double GetDistance(int timeoutMilliseconds = 20)
        {
            if (false == initialized)
                return 0.0;

            TimeoutMilliseconds = timeoutMilliseconds;

            trigPin.Write(GpioPinValue.Low);
            Task.Delay(TimeSpan.FromMilliseconds(1)).Wait(); // 1millisecond

            trigPin.Write(GpioPinValue.High);
            Task.Delay(TimeSpan.FromMilliseconds(0.01)).Wait(); // 10microseconds
            trigPin.Write(GpioPinValue.Low);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < TimeoutMilliseconds && GpioPinValue.Low == echoPin.Read()) ;

            if (sw.ElapsedMilliseconds >= TimeoutMilliseconds)
                return 0.0;

            sw.Restart();

            while (sw.ElapsedMilliseconds < TimeoutMilliseconds && GpioPinValue.High == echoPin.Read()) ;
            sw.Stop();

            if (sw.ElapsedMilliseconds >= TimeoutMilliseconds)
                return 0.0;

            // the utralsonic speed is 34300cm/s -> 34.3cm/ms. And divide by 2 becuase it is round-trip time.
            return sw.Elapsed.TotalMilliseconds * 34.3 / 2.0;
        }
    }
}
