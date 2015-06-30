using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    // The base call for animated LEDs
    internal class AnimatedLedBase : IAnimationTickListner
    {
        // 
        // Private Vars
        //
        
        // Holds a reference to the backing LED
        Led m_led;

        // Holds the desired values for each of the LED components
        double m_desiredRed = 0.0;
        double m_startRed = 0.0;
        double m_desiredGreen = 0.0;
        double m_startGreen = 0.0;
        double m_desiredBlue = 0.0;
        double m_startBlue = 0.0;
        double m_desiredIntensity = 0.0;
        double m_startIntensity = 0.0;

        // Holds the time for each animation. A time remain of negative indicates the
        // animation is done.
        int m_animationRemainRed = -1;
        uint m_animationLengthRed = 0;
        int m_animationRemainBlue = -1;
        uint m_animationLengthBlue = 0;
        int m_animationRemainGreen = -1;
        uint m_animationLengthGreen = 0;
        int m_animationRemainIntensity = -1;
        uint m_animationLengthIntensity= 0;

        //
        // Constructor 
        //
        public AnimatedLedBase(LedType type)
        {
            m_led = new Led(type);
            InitValues();
            ToggleResigerForAnimationTicks(true);
        }

        public AnimatedLedBase(Led led)
        {
            if(led == null)
            {
                throw new Exception("You must supply a LED object for this constructor!");
            }
            m_led = led;
            InitValues();
            ToggleResigerForAnimationTicks(true);
        }

        // 
        // Public functions
        //
        public Led GetLed()
        {
            return m_led;
        }

        // Called by the consumer when they want to animate the LED
        public void Animate(double red, double green, double blue, double intensity, TimeSpan animationTime, AnimationType type)
        {
            // Bounds check
            if((red < 0 || red > 1.0) && red != AnimatedLed.INGORE_VALUE)
            {
                throw new ArgumentOutOfRangeException("Red must be between 0 and 1 except for -1!");
            }
            if ((green < 0 || green > 1.0) && green != AnimatedLed.INGORE_VALUE)
            {
                throw new ArgumentOutOfRangeException("Green must be between 0 and 1 except for -1!");
            }
            if ((blue < 0 || blue > 1.0) && blue != AnimatedLed.INGORE_VALUE)
            {
                throw new ArgumentOutOfRangeException("Blue must be between 0 and 1 except for -1!");
            }
            if ((intensity < 0 || intensity > 1.0) && intensity != AnimatedLed.INGORE_VALUE)
            {
                throw new ArgumentOutOfRangeException("Intensity must be between 0 and 1 except for -1!");
            }
            if(animationTime.TotalMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("The animation time must be positive!");
            }

            // Update the values that need updating
            if(red != AnimatedLed.INGORE_VALUE)
            {
                m_desiredRed = red;
                m_startRed = m_led.Red;
                m_animationLengthRed = (uint)animationTime.TotalMilliseconds;
                m_animationRemainRed = (int)animationTime.TotalMilliseconds;
            }
            if(green != AnimatedLed.INGORE_VALUE)
            {
                m_desiredGreen = green;
                m_startGreen = m_led.Green;
                m_animationLengthGreen = (uint)animationTime.TotalMilliseconds;
                m_animationRemainGreen = (int)animationTime.TotalMilliseconds;
            }
            if (blue != AnimatedLed.INGORE_VALUE)
            {
                m_desiredBlue = blue;
                m_startBlue = m_led.Blue;
                m_animationLengthBlue = (uint)animationTime.TotalMilliseconds;
                m_animationRemainBlue = (int)animationTime.TotalMilliseconds;
            }
            if (intensity != AnimatedLed.INGORE_VALUE)
            {
                m_desiredIntensity = intensity;
                m_startIntensity = m_led.Intensity;
                m_animationLengthIntensity = (uint)animationTime.TotalMilliseconds;
                m_animationRemainIntensity = (int)animationTime.TotalMilliseconds;
            }
        }


        // Called for each animation tick, the time given is the time from the last tick.
        // Returns if something was changed or not.
        public bool NotifiyAnimationTick(int timeElapsedMs)
        {
            bool hasUpdates = false;

            // Update Red
            double result = AnimateValue(m_led.Red, m_desiredRed, m_startRed, ref m_animationRemainRed, m_animationLengthRed, timeElapsedMs);
            if(result != -1)
            {
                m_led.Red = result;
                hasUpdates = true;
            }

            // Update Green
            result = AnimateValue(m_led.Green, m_desiredGreen, m_startGreen, ref m_animationRemainGreen, m_animationLengthGreen, timeElapsedMs);
            if (result != -1)
            {
                m_led.Green = result;
                hasUpdates = true;
            }

            // Update Blue
            result = AnimateValue(m_led.Blue, m_desiredBlue, m_startBlue, ref m_animationRemainBlue, m_animationLengthBlue, timeElapsedMs);
            if (result != -1)
            {
                m_led.Blue = result;
                hasUpdates = true;
            }

            // Update Intensity
            result = AnimateValue(m_led.Intensity, m_desiredIntensity, m_startIntensity, ref m_animationRemainIntensity, m_animationLengthIntensity, timeElapsedMs);
            if (result != -1)
            {
                m_led.Intensity = result;
                hasUpdates = true;
            }

            return hasUpdates;
        }

        //
        // Private functions
        //

        // Does the animation logic for the give value
        private double AnimateValue(double currentValue, double desiredValue, double startValue, ref int animationRemain, uint animationLength, int timeElapsed)
        {
            double returnValue = -1;

            // Check if the values are equal and if the animation is still running
            if (!AreCloseEnough(desiredValue, currentValue) && animationRemain >= 0)
            {
                // Remove the elapsed time
                animationRemain -= timeElapsed;

                if (animationRemain > 0)
                {
                    // Figure out what the progress is
                    double animationProgress = 1 - ((double)animationRemain / (double)animationLength);

                    // Set the led to the 
                    returnValue = startValue + ((desiredValue - startValue) * animationProgress);
                }
                else
                {
                    // We are done with the animation.
                    returnValue = desiredValue;
                    animationRemain = -1;
                }
            }
            else
            {
                // If we don't need to animate anymore, kill the time.
                animationRemain = -1;
            }

            return returnValue;
        }

        // Checks if two double are close enough
        private bool AreCloseEnough(double a, double b)
        {
            return Math.Abs(a - b) < 0.0001;
        }

        // Inits values in the class
        private void InitValues()
        {
            // Grab the current color values of the LED
            m_desiredBlue = m_led.Blue;
            m_startBlue = m_desiredBlue;
            m_desiredGreen = m_led.Green;
            m_startGreen = m_desiredGreen;
            m_desiredRed = m_led.Red;
            m_startRed = m_desiredRed;
            m_desiredIntensity = m_led.Intensity;
            m_startIntensity = m_desiredIntensity;
        }
        
        // Registers the LED for ticks from the controller
        private void ToggleResigerForAnimationTicks(bool register)
        {
            m_led.GetBase().ToggleResigerForAnimationTicks(register, this);
        }
    }
}
