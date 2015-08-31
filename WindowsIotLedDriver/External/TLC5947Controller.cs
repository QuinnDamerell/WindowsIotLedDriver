using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    /// <summary>
    /// This is an implementation of a controller for the TI TLC5947 chip. This may work with other SPI controllers,
    /// but I am not sure at this time.
    /// </summary>
    public sealed class TLC5947Controller
    {
        // Holds a reference to the base controller class
        TLC5947ControllerBase m_base;

        /// <summary>
        /// Creates a new SPI controller
        /// </summary>
        public TLC5947Controller(uint latchPin, uint blackoutPin)
        {
            m_base = new TLC5947ControllerBase(latchPin, blackoutPin);
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

        /// <summary>
        /// Animates the master intensity for the controller.
        /// </summary>
        /// <param name="intensity">The new intensity</param>
        /// <param name="animationTime">How long the animation should take</param>
        /// <param name="type">The type of animation</param>
        public void AnimateMasterIntensity(double intensity, TimeSpan animationTime)
        {
            m_base.AnimateMasterIntensity(intensity, animationTime);
        }

        /// <summary>
        /// Returns the current intensity.
        /// </summary>
        /// <returns></returns>
        public double GetMasterIntensity()
        {
            return m_base.GetMasterIntensity();
        }

        /// <summary>
        /// Returns if the master intensity is currently animating.
        /// </summary>
        /// <returns></returns>
        public bool IsMasterIntensityAnimating()
        {
            return m_base.IsMasterIntensityAnimating();
        }
    }
}
