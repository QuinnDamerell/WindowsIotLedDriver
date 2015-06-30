

namespace WindowsIotLedDriver
{
    /// <summary>
    /// Defines how the controller wants to take slot updates. If the controller needs to know all of the 
    /// slots state for any update, it should opt for AllSlots. If the controller can update each
    /// slot individually, it should opt for SingleSlot.
    /// </summary>
    public enum ControlerUpdateType
    {
        AllSlots,
        SingleSlot
    }

    /// <summary>
    /// Defines the base class for any LED controller. This class should never be used by something that
    /// isn't extending the LED controller.
    /// </summary>
    public sealed class LedController 
    {
        //
        // Public vars
        //

        /// <summary>
        /// Sets a master intensity for the controller.
        /// </summary>
        public double MasterIntensity
        {
            get
            {
                return m_base.MasterIntensity;
            }
            set
            {
                m_base.MasterIntensity = value;
            }
        }

        //
        // Private vars
        //

        // Holds a reference to the base controller class
        LedControllerBase m_base;

        //
        // Constructor
        //

        /// <summary>
        /// The constructor for the LED controller. The state change interface is used to tell the
        //  implementer when slots have been added or removed and when new slot update are available.
        //  The controller update type indicates how the controller wants to take update, see above.
        /// </summary>
        /// <param name="stateListener">The callback for controller events</param>
        /// <param name="updateType">Defines how the extended controller can take updates</param>
        public LedController(ILedControllerExtender stateListener, ControlerUpdateType updateType)
        {
            m_base = new LedControllerBase(stateListener, updateType);
        }


        /// <summary>
        /// Bind a LED to the controller and specifies where it should be bound.
        //  For single color LEDs they will bind to the number given, for multi color
        //  LEDs they will bind to the given number with an additional 2 slots added linearly.
        /// </summary>
        /// <param name="startingPosition">The first slot the LED should be bound to</param>
        /// <param name="assoicateLed">The LED to bind to the controller</param>
        public void AssociateLed(int startingPosition, Led assoicateLed)
        {
            m_base.AssociateLed(startingPosition, assoicateLed);
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
