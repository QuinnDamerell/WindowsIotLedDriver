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


namespace LedDriverSample
{
    public sealed partial class SimpleSample : Page, IVisualLedListener
    {
        bool isSetup = false;
        VisualController m_controller;
        Led m_led1;
        Led m_led2;
        Led m_led3;
        Led m_led4;
        Led m_led5;

        public SimpleSample()
        {
            this.InitializeComponent();
            this.Loaded += SimpleSample_Loaded;
        }

        private void SimpleSample_Loaded(object sender, RoutedEventArgs e)
        {
            m_controller = new VisualController();
            m_controller.AddVisualListener(this);
            m_led1 = new Led(LedType.RBG);
            m_led2 = new Led(LedType.RBG);
            m_led3 = new Led(LedType.RBG);
            m_led4 = new Led(LedType.RBG);
            m_led5 = new Led(LedType.RBG);

            m_controller.AssoicateLed(m_led1);
            m_controller.AssoicateLed(m_led2);
            m_controller.AssoicateLed(m_led3);
            m_controller.AssoicateLed(m_led4);
            m_controller.AssoicateLed(m_led5);

            // Match the slider default
            m_led1.Intensity = 1.0;
            m_led2.Intensity = 1.0;
            m_led3.Intensity = 1.0;
            m_led4.Intensity = 1.0;
            m_led5.Intensity = 1.0;

            isSetup = true;
        }

        public void UpdateVisualLed(int LedNumber, byte red, byte green, byte blue)
        {
            switch(LedNumber)
            {
                case 1:
                    {
                        VisualLed1.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                        break;
                    }
                case 2:
                    {
                        VisualLed2.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                        break;
                    }
                case 3:
                    {
                        VisualLed3.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                        break;
                    }
                case 4:
                    {
                        VisualLed4.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                        break;
                    }
                case 5:
                    {
                        VisualLed5.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The led isn't known!");
                    }
            }
        }

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
                    m_led3.Green = slider.Value / 100;
                    break;
                case "Led2Blue":
                    m_led3.Blue = slider.Value / 100;
                    break;
                case "Led2Intensity":
                    m_led3.Intensity = slider.Value / 100;
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
    }
}
