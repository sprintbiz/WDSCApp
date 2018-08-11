using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WDSCApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Task msg;
        private const int PIN1 = 18;
        private GpioPin pin;
        private GpioPinValue pinValue;
        const string deviceConnectionString = "HostName=home-IoT-hub1.azure-devices.net;DeviceId=Device1;SharedAccessKey=R0br8n7yZ2ii1ry3aD0Nsjczp38ebTS2JD38+upw3vs=";
        static DeviceClient deviceClient = null;
        string CurrentStatus = "";
        Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

        public MainPage()
        {
            this.InitializeComponent();
            Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation eas =
                new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            DeviceBox.Text = eas.FriendlyName;

            ReceiveCloudToDeviceMessageAsync();



        }


        private static void CreateClient()
        {
            if (deviceClient == null)
            {
                // create Azure IoT Hub client from embedded connection string
                deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
            }
        }

        private async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            CreateClient();
            
            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);

                    Debug.WriteLine(messageData);
                    if (messageData == "on")
                    {
                        receiveMessage(messageData);
                        CurrentStatus = messageData;
                        InitGPIO(PIN1);
                    }

                    if (messageData == "off")
                    {
                        receiveMessage(messageData);
                        CurrentStatus = messageData;
                    }

                    if (messageData == "status")
                    {
                        receiveMessage(messageData);
                        
                    }
                    //return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        private async void receiveMessage(string message)
        {
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                MessageBox.Text = "Received message: " + message;
            });
        }

        private void InitGPIO(int GPIOPin)
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pin = gpio.OpenPin(GPIOPin);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized correctly.";

        }

        private void TurnOn(int GPIOPin)
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pin = gpio.OpenPin(GPIOPin);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized correctly.";

        }

    }
}
