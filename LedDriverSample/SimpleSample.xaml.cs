using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsIotLedDriver;
using Windows.UI.Core;

// Screw you Winrt
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LedDriverSample
{
    public sealed partial class SimpleSample : Page, IVisualLedListener
    {
        // Holds a reference to the controller. In this case the controller is one that reports back to the UI
        // for testing.
        VisualLedController m_controller;

        // Holds each LED.
        Led m_led1;
        Led m_led2;
        Led m_led3;
        Led m_led4;
        Led m_led5;

        // Holds the animated LEDs
        AnimatedLed m_animatedLed1;
        AnimatedLed m_animatedLed2;
        AnimatedLed m_animatedLed3;
        AnimatedLed m_animatedLed4;
        AnimatedLed m_animatedLed5;

        // Misc vars
        CoreDispatcher m_dispatcher;
        bool isSetup = false;

        // Animation timer vars
        enum VisualAnimationType
        {
            None,
            Random,
            Clock
        }
        DispatcherTimer m_timer;
        VisualAnimationType m_animationType = VisualAnimationType.None;


        public SimpleSample()
        {
            this.InitializeComponent();
            this.Loaded += SimpleSample_Loaded;
            m_dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;          
        }

        private void SimpleSample_Loaded(object sender, RoutedEventArgs e)
        {
            // First we need to create the controller. This is the object that is responsible for controlling the
            // output of the system. The visual controller is an extension of the controller that feeds data back to 
            // a UI. Two controller are provided in the SDK, a software PWM controller, and a SPI bus controller.
            m_controller = new VisualLedController();

            // This is only needed for the VisualController
            m_controller.AddVisualListener(this);

            // Now create the LEDs. All we need to do is tell the system if they are multi color or single colored.
            m_led1 = new Led(LedType.RBG);
            m_led2 = new Led(LedType.RBG);
            m_led3 = new Led(LedType.RBG);
            m_led4 = new Led(LedType.RBG);
            m_led5 = new Led(LedType.RBG);

            // Next wrap the LEDs in an animated LED object so we can make them animated nicely.
            m_animatedLed1 = new AnimatedLed(m_led1);
            m_animatedLed2 = new AnimatedLed(m_led2);
            m_animatedLed3 = new AnimatedLed(m_led3);
            m_animatedLed4 = new AnimatedLed(m_led4);
            m_animatedLed5 = new AnimatedLed(m_led5);

            // Last, associate the LEDs with the controller. This actually binds the LED objects to a given controller.
            // The only thing we must specify here is what slot the LED is in. What a slot means is defined by whatever
            // controller you're using, for example in PWM a slot is the same as a pin; for SPI the slot is the same as the output
            // For RGB LEDs the slot passed is for the first pin (red), blue and green are assumed to be slot + 1 and slot + 2.
            m_controller.AssociateLed(0, m_led1);
            m_controller.AssociateLed(3, m_led2);
            m_controller.AssociateLed(6, m_led3);
            m_controller.AssociateLed(9, m_led4);
            m_controller.AssociateLed(12, m_led5);

            // Now set the intensity on the LEDs to match the UI.
            m_led1.Intensity = 1.0;
            m_led2.Intensity = 1.0;
            m_led3.Intensity = 1.0;
            m_led4.Intensity = 1.0;
            m_led5.Intensity = 1.0;

            // Setup the animation thread.
            m_timer = new DispatcherTimer();
            m_timer.Tick += AnimationTimer_Tick;
            m_timer.Interval = new TimeSpan(0, 0, 1);
            m_timer.Start();

            // Done.
            isSetup = true;
        }

        // This function is a call back from the Visual controller when one LED is updated. 
        public void UpdateVisualLed(int LedNumber, byte red, byte green, byte blue)
        {
            m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    switch (LedNumber)
                    {
                        case 1:
                            {
                                VisualLed1.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                                Led1Red.Value = (red/255.0) * 100;
                                Led1Green.Value = (green / 255.0) * 100;
                                Led1Blue.Value = (blue / 255.0) * 100;
                                break;
                            }
                        case 2:
                            {
                                VisualLed2.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                                Led2Red.Value = (red / 255.0) * 100;
                                Led2Green.Value = (green / 255.0) * 100;
                                Led2Blue.Value = (blue / 255.0) * 100;
                                break;
                            }
                        case 3:
                            {
                                VisualLed3.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                                Led3Red.Value = (red / 255.0) * 100;
                                Led3Green.Value = (green / 255.0) * 100;
                                Led3Blue.Value = (blue / 255.0) * 100;
                                break;
                            }
                        case 4:
                            {
                                VisualLed4.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                                Led4Red.Value = (red / 255.0) * 100;
                                Led4Green.Value = (green / 255.0) * 100;
                                Led4Blue.Value = (blue / 255.0) * 100;
                                break;
                            }
                        case 5:
                            {
                                VisualLed5.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                                Led5Red.Value = (red / 255.0) * 100;
                                Led5Green.Value = (green / 255.0) * 100;
                                Led5Blue.Value = (blue / 255.0) * 100;
                                break;
                            }
                        default:
                            {
                                throw new ArgumentException("The led isn't known!");
                            }
                    }
                });           
        }

        // This function is a callback from the Visual controller when all of the LEDs are updated.
        public void UpdateVisualLed(int singleSlot, byte value)
        {
            m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (singleSlot)
                {
                    case 0:
                        VisualLed1.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, value, ((SolidColorBrush)(VisualLed1.Fill)).Color.G, ((SolidColorBrush)(VisualLed1.Fill)).Color.B));
                        break;
                    case 1:
                        VisualLed1.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed1.Fill)).Color.R, value, ((SolidColorBrush)(VisualLed1.Fill)).Color.B));
                        break;
                    case 2:
                        VisualLed1.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed1.Fill)).Color.R, ((SolidColorBrush)(VisualLed1.Fill)).Color.G, value));
                        break;

                    case 3:
                        VisualLed2.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, value, ((SolidColorBrush)(VisualLed2.Fill)).Color.G, ((SolidColorBrush)(VisualLed2.Fill)).Color.B));
                        break;
                    case 4:
                        VisualLed2.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed2.Fill)).Color.R, value, ((SolidColorBrush)(VisualLed2.Fill)).Color.B));
                        break;
                    case 5:
                        VisualLed2.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed2.Fill)).Color.R, ((SolidColorBrush)(VisualLed2.Fill)).Color.G, value));
                        break;

                    case 6:
                        VisualLed3.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, value, ((SolidColorBrush)(VisualLed3.Fill)).Color.G, ((SolidColorBrush)(VisualLed3.Fill)).Color.B));
                        break;
                    case 7:
                        VisualLed3.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed3.Fill)).Color.R, value, ((SolidColorBrush)(VisualLed3.Fill)).Color.B));
                        break;
                    case 8:
                        VisualLed3.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed3.Fill)).Color.R, ((SolidColorBrush)(VisualLed3.Fill)).Color.G, value));
                        break;

                    case 9:
                        VisualLed4.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, value, ((SolidColorBrush)(VisualLed4.Fill)).Color.G, ((SolidColorBrush)(VisualLed4.Fill)).Color.B));
                        break;
                    case 10:
                        VisualLed4.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed4.Fill)).Color.R, value, ((SolidColorBrush)(VisualLed4.Fill)).Color.B));
                        break;
                    case 11:
                        VisualLed4.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed4.Fill)).Color.R, ((SolidColorBrush)(VisualLed4.Fill)).Color.G, value));
                        break;

                    case 12:
                        VisualLed5.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, value, ((SolidColorBrush)(VisualLed5.Fill)).Color.G, ((SolidColorBrush)(VisualLed5.Fill)).Color.B));
                        break;
                    case 13:
                        VisualLed5.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed5.Fill)).Color.R, value, ((SolidColorBrush)(VisualLed5.Fill)).Color.B));
                        break;
                    case 14:
                        VisualLed5.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, ((SolidColorBrush)(VisualLed5.Fill)).Color.R, ((SolidColorBrush)(VisualLed5.Fill)).Color.G, value));
                        break;

                }
            });
        }

        // This is a callback from the UI when a slider has changed.
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if(!isSetup)
            {
                // Prevent any fires when we are not ready
                return;
            }

            Slider slider = (Slider)sender;
            switch (slider.Name)
            {
                case "Led1Red":
                    m_led1.Red = slider.Value / 100;
                    break;
                case "Led1Green":
                    m_led1.Green = slider.Value / 100;
                    break;
                case "Led1Blue":
                    m_led1.Blue = slider.Value / 100;
                    break;
                case "Led1Intensity":
                    m_led1.Intensity = slider.Value / 100;
                    break;
                case "Led2Red":
                    m_led2.Red = slider.Value / 100;
                    break;
                case "Led2Green":
                    m_led2.Green = slider.Value / 100;
                    break;
                case "Led2Blue":
                    m_led2.Blue = slider.Value / 100;
                    break;
                case "Led2Intensity":
                    m_led2.Intensity = slider.Value / 100;
                    break;
                case "Led3Red":
                    m_led3.Red = slider.Value / 100;
                    break;
                case "Led3Green":
                    m_led3.Green = slider.Value / 100;
                    break;
                case "Led3Blue":
                    m_led3.Blue = slider.Value / 100;
                    break;
                case "Led3Intensity":
                    m_led3.Intensity = slider.Value / 100;
                    break;
                case "Led4Red":
                    m_led4.Red = slider.Value / 100;
                    break;
                case "Led4Green":
                    m_led4.Green = slider.Value / 100;
                    break;
                case "Led4Blue":
                    m_led4.Blue = slider.Value / 100;
                    break;
                case "Led4Intensity":
                    m_led4.Intensity = slider.Value / 100;
                    break;
                case "Led5Red":
                    m_led5.Red = slider.Value / 100;
                    break;
                case "Led5Green":
                    m_led5.Green = slider.Value / 100;
                    break;
                case "Led5Blue":
                    m_led5.Blue = slider.Value / 100;
                    break;
                case "Led5Intensity":
                    m_led5.Intensity = slider.Value / 100;
                    break;
                default:                    
                    break;                    
            }
        }

        // This is a callback from the UI when one of the animation buttons has been clicked.
        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Random rand = new Random();
            TimeSpan time = new TimeSpan(0, 0, 0, (int)(rand.Next(1, 5)));

            switch (button.Name)
            {
                case "Animate1":
                    m_animatedLed1.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                    break;
                case "Animate2":
                    m_animatedLed2.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                    break;
                case "Animate3":
                    m_animatedLed3.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                    break;
                case "Animate4":
                    m_animatedLed4.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                    break;
                case "Animate5":
                    m_animatedLed5.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                    break;
                case "StartRandom":
                    m_animationType = VisualAnimationType.Random;
                    break;
                case "StartClock":
                    m_animationType = VisualAnimationType.Clock;
                    break;
                case "StopAnimations":
                    m_animationType = VisualAnimationType.None;
                    break;                   
            }
        }

        // This is a timer callback function when the animation timer has fired.
        private void AnimationTimer_Tick(object sender, object e)
        {
            switch(m_animationType)
            {
                case VisualAnimationType.None:
                    // Tick slower
                    m_timer.Interval = new TimeSpan(0, 0, 1);
                    break;
                case VisualAnimationType.Random:
                    // Set the timer to tick faster
                    m_timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

                    // Randomly pick some values.
                    Random rand = new Random();
                    TimeSpan time = new TimeSpan(0,0,rand.Next(2,5));
                    switch(rand.Next(0,5))
                    {
                        case 0:
                            m_animatedLed1.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                            break;
                        case 1:
                            m_animatedLed2.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                            break;
                        case 2:
                            m_animatedLed3.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                            break;
                        case 3:
                            m_animatedLed4.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                            break;
                        case 4:
                            m_animatedLed5.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, time, AnimationType.Linear);
                            break;
                    }
                    
                    break;
                case VisualAnimationType.Clock:
                    // Tick faster
                    m_timer.Interval = new TimeSpan(0, 0, 0, 0, 50);

                    // Set the colors for the current time.
                    DateTime now = DateTime.Now;
                    double days = (now.DayOfYear / 365.0);
                    int hour = now.Hour > 12 ? now.Hour - 12 : now.Hour;
                    double hours = (hour / 12.0);
                    double minutes = (now.Minute / 60.0);
                    double seconds = (now.Second / 60.0);
                    double mills = (now.Millisecond / 1000.0);
                    TimeSpan animationTimer = new TimeSpan(0, 0, 1);

                    // Set the colors
                    m_animatedLed1.Animate(days, days, days, -1.0, animationTimer, AnimationType.Linear);
                    m_animatedLed2.Animate(hours, hours, hours, -1.0, animationTimer, AnimationType.Linear);
                    m_animatedLed3.Animate(minutes, minutes, minutes, -1.0, animationTimer, AnimationType.Linear);
                    m_animatedLed4.Animate(seconds, seconds, seconds, -1.0, animationTimer, AnimationType.Linear);
                    m_animatedLed5.Animate(mills, mills, mills, -1.0, new TimeSpan(0,0,0,0,50), AnimationType.Linear);
                    break;
            }
        }
    }
}
