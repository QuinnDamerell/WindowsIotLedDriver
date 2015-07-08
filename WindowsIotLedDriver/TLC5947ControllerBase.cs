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

        // A buffer to hold the bits we write.
        byte[] m_bitBuffer = new byte[36];
        byte[] m_rbitBuffer = new byte[36];
        GpioPin pin;

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

        public TLC5947ControllerBase(int chipSelectPin)
        {
            // Create the controller
            m_controller = new LedController(this, ControlerUpdateType.AllSlots); // #todo set this when known.

            // Latch
            GpioController controller = GpioController.GetDefault();
            pin = controller.OpenPin(6);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            // Blackout
            GpioPin pin2 = controller.OpenPin(5);
            pin2.SetDriveMode(GpioPinDriveMode.Output);
            pin2.Write(GpioPinValue.High);
            pin2.Write(GpioPinValue.Low);

            //for (int i = 0; i < 50; i++)
            //{
            //    GpioPin pin;
            //    GpioOpenStatus statu;
            //    if (controller.TryOpenPin(i, GpioSharingMode.Exclusive, out pin, out statu))
            //    {
            //        pin.Write(GpioPinValue.Low);
            //        pin.Write(GpioPinValue.High);
            //        //pin.Write(GpioPinValue.Low);
            //    }
            //}



            // Create a async task to setup SPI
            new Task(async () =>
            {
                // Create the settings
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                // Max SPI clock frequency, here it is 30MHz
                settings.ClockFrequency = 30000000;               
   

                settings.Mode = SpiMode.Mode0;                                  /* The display expects an idle-high clock polarity, we use Mode3    
                                                                                 * to set the clock polarity and phase to: CPOL = 1, CPHA = 1         
                                                                                 */

                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);       /* Find the selector string for the SPI bus controller          */
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);         /* Find the SPI bus controller device with our selector string  */
                m_spiDevice = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);  /* Create an SpiDevice with our bus controller and SPI settings */
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
            m_controller.ToggleAnimation(enableAnmation, false); // #todo set alwayspaint when we know the correct value
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
                Int16 bitValue = (Int16)(value * 4096);

                if(slot % 2 == 0)
                {
                    // This is an even slot, we will fill 8 bits on the first byte, and 4 bits
                    // on the second byte.

                    // Shift all of the bits left by 8 to drop the 8 bits major bits, and then back
                    // so we only have the 8 lowest bits remaining.
                    byte first = (byte)(((Int16)(bitValue << 8)) >> 8);

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

                    // Same as above, sift the right 12 so we will only have 4 bits remaining,
                    // then shit them back in place.

                    byte first = (byte)(((Int16)(bitValue << 12)) >> 12);
                    byte firste = (byte)((bitValue << 12) );

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

        int skip = 0;
        int writebyte = 0;
        public void WriteToDevice(byte[] bits)
        {
            //for(int i = 0; i < 36; i++)
            //{
            //    byte result = 0;
            //    byte val = bits[i];
            //    int counter = 8;
            //    while (counter-- < 0)
            //    {
            //        result <<= 1;
            //        result |= (byte)(val & 1);
            //        val = (byte)(val >> 1);
            //    }

            //    m_rbitBuffer[36 - i] = result;
            //}




            for (int i = 0; i < bits.Length; i++)
            {
                m_rbitBuffer[35 - i] = bits[i];

                //bits[i] = 0;
            }

            //writebyte += 5;
            //if (writebyte > 4096)
            //    writebyte = 0;

            //if (writebyte < 15)
            //{
            //    bits[1] = (byte)((writebyte & 255) << 4);
            //}
            //else
            //{
            //    bits[1] = (byte)((writebyte & 255) << 4);
            //    bits[0] = (byte)((writebyte & 0xFF0) >> 4);
            //}

            // bits[1] = (byte)((writebyte & 255) << 4);

            //bits[0] = (byte)writebyte;

            //pin.Write(GpioPinValue.High);

            //System.Diagnostics.Debug.WriteLine("byte 1:"+bits[1] +" byte 0:"+bits[0]);


            if (m_spiDevice != null)
            {
                pin.Write(GpioPinValue.Low);
                // Write the entire buffer to the device.
                m_spiDevice.Write(m_rbitBuffer);
                pin.Write(GpioPinValue.High);

            }
        }
    }
}
