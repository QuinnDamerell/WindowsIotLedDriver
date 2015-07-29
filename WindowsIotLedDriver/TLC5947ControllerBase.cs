using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace WindowsIotLedDriver
{
    class TLC5947ControllerBase : ILedControllerExtender
    {
        // Line 0 maps to physical pin number 24 on the Rpi2  
        private const Int32 SPI_CHIP_SELECT_LINE = 0;
        //For Raspberry Pi 2, use SPI0
        private const string SPI_CONTROLLER_NAME = "SPI0";
        // Chip slot size
        private const int TCL5947_SLOT_SIZE = 24;
        private const int TCL5947_BITS_PER_SLOT = 12;
        private const int TCL5947_TOTAL_BYTES_PER_DEVICE = 36;

        // A buffer to hold the bits we write.
        byte[] m_bitBuffer = new byte[TCL5947_TOTAL_BYTES_PER_DEVICE];
        byte[] m_outputBuffer = new byte[TCL5947_TOTAL_BYTES_PER_DEVICE];

        /// <summary>
        /// Need to set the LEDS. This is set low to high on every full set of LED commands.
        /// </summary>
        GpioPin m_latchPin;

        /// <summary>
        /// This pin is used to black out the all of the LEDs
        /// </summary>
        GpioPin m_blackoutPin;

        //
        // Public Vars
        //

        public double MasterIntensity
        {
            get
            {
                return m_controller.MasterIntensity;
            }
            set
            {
                m_controller.MasterIntensity = value;
            }
        }

        //
        // Private Vars
        // 
        LedController m_controller;
        SpiDevice m_spiDevice = null;

        //
        // Constructor
        //

        public TLC5947ControllerBase(uint latchPin, uint blackoutPin)
        {
            // Create the controller
            m_controller = new LedController(this, ControlerUpdateType.AllSlots);

            // Open the latch pin
            GpioController controller = GpioController.GetDefault();
            m_latchPin = controller.OpenPin((int)latchPin);
            m_latchPin.SetDriveMode(GpioPinDriveMode.Output);

            // Open the black out pin, set it high and low to reset the device.
            m_blackoutPin = controller.OpenPin((int)blackoutPin);
            m_blackoutPin.SetDriveMode(GpioPinDriveMode.Output);
            m_blackoutPin.Write(GpioPinValue.High);
            m_blackoutPin.Write(GpioPinValue.Low);

            // Create a async task to setup SPI
            new Task(async () =>
            {
                // Create the settings
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                // Max SPI clock frequency, here it is 30MHz
                settings.ClockFrequency = 30000000;     
                settings.Mode = SpiMode.Mode0;
                //  Find the selector string for the SPI bus controller
                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
                // Find the SPI bus controller device with our selector string
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);
                // Create an SpiDevice with our bus controller and SPI settings
                m_spiDevice = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
            }).Start();
        }

        public void NotifiySlotsAdded(int firstSlot, int numberOfSlots)
        {
            // For this controller we don't need to do anything.
        }

        public void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots)
        {
            // For this controller we don't need to do anything.
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
            // Loop through ever slot on the device
            int currentSlot = 0;
            for(int slot  = 0; slot < TCL5947_SLOT_SIZE; slot++)
            {
                // Get the slot value if we have one.
                double value = 0;
                if(slot < newVaules.Count)
                {
                    value = newVaules[slot];
                }

                // Convert the double to a 12 bit value
                Int16 bitValue = (Int16)(value * 4095);

                if(slot % 2 == 0)
                {
                    // This is an even slot, we will fill 8 bits on the first byte, and 4 bits
                    // on the second byte.

                    // Use a mask to remove everything but the lowest 8 bits.
                    byte first = (byte)(bitValue & 0xFF);

                    // Shift right by 8 bits so the 8 lowest bits will fall off and we will have the
                    // 8 highest bits. Due to the calculation above at my only the first 4 lowest bits should
                    // have data.
                    byte overflow = (byte)(bitValue >> 8);

                    m_bitBuffer[currentSlot] = first;
                    currentSlot++;
                    m_bitBuffer[currentSlot] = overflow;
                }
                else
                {
                    // This is an odd slot. We will fill the first 8 byte of the current slot
                    // and all 8 bits of the next byte.

                    // Same as above, mask the number so we only have the lowest 4 bits remaining.
                    byte first = (byte)(bitValue & 0xF);

                    // Same again, sift the bottom 4 bits off the lower bits. Due to the calculations
                    // above there shouldn't be anymore than 8 bits remaining.
                    byte overflow = (byte)(bitValue >> 4);

                    // Capture the current value of the byte, this shouldn't be anymore than 4 bits of data.
                    byte temp = m_bitBuffer[currentSlot];
                    // Now set the first bits
                    m_bitBuffer[currentSlot] = first;
                    // Now shift them into the higher 4 bits
                    m_bitBuffer[currentSlot] = (byte)(m_bitBuffer[currentSlot] << 4);
                    // Now set the lower 4 bits back
                    m_bitBuffer[currentSlot] += temp;

                    // Now set the next byte
                    currentSlot++;
                    m_bitBuffer[currentSlot] = overflow;

                    // Increment one more time so the next data starts on the next byte
                    currentSlot++;
                }
            }

            // We are done, write them to the device.
            WriteToDevice(m_bitBuffer);
        }

        public void WriteToDevice(byte[] bits)
        {
            // The device requires we send the list in reverse order.
            for (int i = 0; i < bits.Length; i++)
            {
                m_outputBuffer[TCL5947_TOTAL_BYTES_PER_DEVICE - 1 - i] = bits[i];
            }

            if (m_spiDevice != null)
            {               
                // Write the entire buffer to the device.
                m_spiDevice.Write(m_outputBuffer);

                // Write the latch to low and high to indicate we have written all of the bits.
                m_latchPin.Write(GpioPinValue.Low);
                m_latchPin.Write(GpioPinValue.High);
                m_latchPin.Write(GpioPinValue.Low);
            }
        }
    }
}
