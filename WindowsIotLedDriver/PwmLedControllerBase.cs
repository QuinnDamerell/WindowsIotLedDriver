using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WindowsIotLedDriver
{
    internal class PwmLedControllerBase : ILedControllerExtender, IAnimationTickListner
    {
        //
        // Private Vars
        //
        LedController m_controller;
        GpioController m_gpioController;
        Dictionary<int, GpioPin> m_pinMap;
        IReadOnlyList<double> m_currentValuesList;
        TheAnimator m_animator;
        int m_tickCouunt = 0;

        public PwmLedControllerBase()
        {
            // Try to get the GPIO
            m_gpioController = GpioController.GetDefault();
            if(m_gpioController == null)
            {
                throw new Exception("This device doesn't have a GPIO!");
            }

            // Init the vars
            m_pinMap = new Dictionary<int, GpioPin>();
            m_currentValuesList = new List<double>();

            // Create the controller
            m_controller = new LedController(this, ControlerUpdateType.AllSlots);

            // Create an animator to control the ticks
            m_animator = new TheAnimator(this, 0);
        }

        public void NotifiySlotsAdded(int firstSlot, int numberOfSlots)
        {
            // For each new pin, try to open it and then add it to the list
            for(int i = 0; i < numberOfSlots; i++)
            {
                GpioPin pin;
                GpioOpenStatus status;
                if(m_gpioController.TryOpenPin(firstSlot + i, GpioSharingMode.Exclusive, out pin, out status))
                {
                    // Get got the pin! Add it to our list.
                    pin.SetDriveMode(GpioPinDriveMode.Output);
                    pin.Write(GpioPinValue.High);
                    m_pinMap.Add(firstSlot + i, pin);
                }
                else
                {
                    throw new Exception("The pin is not available! Reason: " + status.ToString());
                }
            }
        }

        public void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots)
        {
            // #todo: Finished PWM remove logic
        }

        public void AssoicateLed(int startingPosition, Led assoicateLed)
        {
            m_controller.AssociateLed(startingPosition, assoicateLed);
        }

        public void DissociateLed(Led dissociateLed)
        {
            m_controller.DissociateLed(dissociateLed);
        }

        public void ToggleAnimation(bool enableAnmation)
        {
            m_controller.ToggleAnimation(enableAnmation, false);
        }

        public void ToggleAnimation(bool enableAnmation, int animationRateMilliseconds)
        {
            m_controller.ToggleAnimation(enableAnmation, false, animationRateMilliseconds);
        }

        public void NotifiySlotStateChanged(int Slot, double newValue)
        {
            // We are using AllSlot update mode, so this won't be called
        }

        public void NotifiySlotsStateChanged(IReadOnlyList<double> newVaules)
        {
            // Capture the new values
            m_currentValuesList = newVaules;
        }

        public bool NotifiyAnimationTick(int timeElapsedMs)
        {
            // Capture the current list of values for this tick
            IReadOnlyList<double> pinValuesThisTick = m_currentValuesList;

            // #todo: calculate cycle per tick            

            // Loop through each value and set it.
            for(int i = 0; i < pinValuesThisTick.Count; i++)
            {
                // Try to get a pin for this number
                GpioPin pin;
                if (m_pinMap.TryGetValue(i, out pin))
                {
                    int ticksPostive =  (int)(pinValuesThisTick[i] * 100);
                    bool setThisTick = false;

                    if (m_tickCouunt < ticksPostive)
                    {
                        setThisTick = true;
                    }

                    // Write the value!
                    pin.Write(setThisTick ? GpioPinValue.High : GpioPinValue.Low);
                }
            }

            m_tickCouunt++;
            if(m_tickCouunt > 100)
            {
                m_tickCouunt = 0;
            }

            return true;
        }
    }
}
