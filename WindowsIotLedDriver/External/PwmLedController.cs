namespace WindowsIotLedDriver
{
    /// <summary>
    /// A implementation of a basic software PWM controller. This will use a simple thread method to try to create the correct
    /// color in the LEDS. 
    /// Note: this doesn't work very well and isn't 100% complete!
    /// </summary>
    public sealed class PwmLedController
    {
        //
        // Public vars
        //

        // Holds a reference to the base controller class
        PwmLedControllerBase m_base;

        /// <summary>
        /// Creates a new PWM controller
        /// </summary>
        public PwmLedController()
        {
            m_base = new PwmLedControllerBase();
        }

        /// <summary>
        /// Bind a LED to the controller and specifies where it should be bound.
        //  For single color LEDs they will bind to the number given, for multi color
        //  LEDs they will bind to the given number with an additional 2 slots added linearly.
        /// </summary>
        /// <param name="startingPosition">The first slot the LED should be bound to</param>
        /// <param name="assoicateLed">The LED to bind to the controller</param>
        public void AssoicateLed(int startingPosition, Led assoicateLed)
        {
            m_base.AssoicateLed(startingPosition, assoicateLed);
        }

        /// <summary>
        /// Removes an LED from the controller. This will clean up the slots allocated for it.
        /// </summary>
        /// <param name="dissociateLed">The LED that should be removed</param>
        public void DissociateLed(Led dissociateLed)
        {
            m_base.DissociateLed(dissociateLed);
        }

        /// <summary>
        /// Enables animation for the controller. 
        /// This needs to be enabled to use animated pixels. This can also be used a v-sync type device
        /// for controllers that need constant updates.
        /// </summary>
        /// <param name="enableAnmation">Enables or diables animation</param>
        public void ToggleAnimation(bool enableAnmation)
        {
            m_base.ToggleAnimation(enableAnmation);
        }

        /// <summary>
        /// Enables animation for the controller. 
        /// This needs to be enabled to use animated pixels. This can also be used a v-sync type device
        /// for controllers that need constant updates.
        /// </summary>
        /// <param name="enableAnmation">Enables or diables animation</param>
        /// <param name="animationRateMilliseconds">The rate at which the animation thread will run</param>
        public void ToggleAnimation(bool enableAnmation, int animationRateMilliseconds)
        {
            m_base.ToggleAnimation(enableAnmation, animationRateMilliseconds);
        }
    }
}
