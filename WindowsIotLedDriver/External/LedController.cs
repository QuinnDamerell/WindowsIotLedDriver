using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    // Defines how the controller can take updates. If the controller needs to know all of the 
    // slots state for each update, it should opt for AllSlots. If the controller can update each
    // slot individually it should opt for SingleSlot
    public enum ControlerUpdateType
    {
        AllSlots,
        SingleSlot
    }

    public sealed class LedController 
    {
        // Holds a reference to the base controller class
        LedControllerBase m_base;

        // The constucrtor for the LED controller. The state change interface is used to tell the
        // implmenter when slots have been added or removed, and when new update are aviable.
        // The controller update type indicates how the controller wants to take update, see above.
        public LedController(IControllerStatChangeListener stateListener, ControlerUpdateType updateType)
        {
            m_base = new LedControllerBase(stateListener, updateType);
        }

        // Bind a LED to the controller and specifies where it should be bound.
        // For single color leds they will bind to the number given, for multi color
        // LEDs they will bind to the given number with an additional 2 slots.
        public void AssoicateLed(int startingPosition, Led assoicateLed)
        {
            m_base.AssoicateLed(startingPosition, assoicateLed);
        }

        // Removes the binging of the LED to the controller
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
        /// <param name="alwaysPaint">Forces the controller to always produce a new frame if the controller update mode is AllSlots</param>
        public void ToggleAnimation(bool enableAnmation, bool alwaysPaint)
        {
            m_base.ToggleAnimation(enableAnmation, alwaysPaint);
        }

        /// <summary>
        /// Enables animation for the controller. 
        /// This needs to be enabled to use animated pixels. This can also be used a v-sync type device
        /// for controllers that need constant updates.
        /// </summary>
        /// <param name="enableAnmation">Enables or diables animation</param>
        /// <param name="alwaysPaint">Forces the controller to always produce a new frame if the controller update mode is AllSlots</param>
        /// <param name="animationRateMilliseconds">The rate at which the animation thread will run</param>
        public void ToggleAnimation(bool enableAnmation, bool alwaysPaint, int animationRateMilliseconds)
        {
            m_base.ToggleAnimation(enableAnmation, alwaysPaint, animationRateMilliseconds);
        }
    }
}
