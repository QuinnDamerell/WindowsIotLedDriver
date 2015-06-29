﻿using System;
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

        AnimatedLed m_animatedLed1;

        CoreDispatcher m_dispatcher;

        public SimpleSample()
        {
            this.InitializeComponent();
            this.Loaded += SimpleSample_Loaded;
            m_dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
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

            m_animatedLed1 = new AnimatedLed(m_led1, true);

            m_controller.AssoicateLed(0, m_led1);
            m_controller.AssoicateLed(3, m_led2);
            m_controller.AssoicateLed(6, m_led3);
            //m_controller.AssoicateLed(9, m_led4);
            m_controller.AssoicateLed(12, m_led5);

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
            m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    switch (LedNumber)
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
                });           
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            m_animatedLed1.Animate(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), -1, new TimeSpan(0, 0, 0, 1), AnimationType.Linear);   
        }
    }
}
