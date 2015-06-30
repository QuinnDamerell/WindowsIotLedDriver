using System;

namespace WindowsIotLedDriver
{
    /// <summary>
    /// Defines the animation type.
    /// </summary>
    public enum AnimationType
    {
        Linear,
    }

    /// <summary>
    /// This class is used to wrap an LED object to enable simple animation.
    /// Note animation must be enabled on the controller for this to work properly.
    /// </summary>
    public sealed class AnimatedLed
    {
        /// <summary>
        /// Returns the type of the LED
        /// </summary>
        public LedType Type
        {
            get { return m_base.GetLed().Type; }
        }

        /// <summary>
        /// This is the magic value that can be used to indicate you don't want to change
        /// a given field when calling animate.
        /// </summary>
        internal static double INGORE_VALUE = -1;

        //
        // Private Vars
        //
        AnimatedLedBase m_base;

        /// <summary>
        /// Constructs an animated led without an led class. The LED class will be
        /// created internally for the animated led.
        /// </summary>
        /// <param name="type">The type of LED that should be created</param>
        /// <param name="boolThatMeansNothingForWinRt">Means nothing, only needed to make WinRT happy</param>
        public AnimatedLed(LedType type, bool boolThatMeansNothingForWinRt)
        {
            m_base = new AnimatedLedBase(type);
        }

        /// <summary>
        /// Creates a new animated LED given an existing LED class
        /// </summary>
        /// <param name="led">An instance of the LED class.</param>
        public AnimatedLed(Led led)
        {
            m_base = new AnimatedLedBase(led);
        }

        /// <summary>
        /// Returns the backing LED class for this animated LED.
        /// </summary>
        /// <returns>The backing LED class</returns>
        public Led GetLed()
        {
            return m_base.GetLed();
        }

        /// <summary>
        /// Called to animate the LED to a new color. The consumer defines the new values the LED should be set at, the
        /// amount of time the animation should take, and the type of animation they want. Any value that is set to -1
        /// will be ignored and will remain at it's current value / animation. The animation will start instantly, and
        /// will start from the color the LED is when the new animation is started.
        /// 
        /// This call will replace any current animation running the on component specified.
        /// </summary>
        /// <param name="red">The value of red at the end of the animation</param>
        /// <param name="green">The value of green at the end of the animation</param>
        /// <param name="blue">The value of blue at the end of the animation</param>
        /// <param name="intensity">The value of intensity at the end of the animation</param>
        /// <param name="animationTime">The ammount of time the animation should run for.</param>
        /// <param name="type">The type of animation that should be ran.</param>
        public void Animate(double red, double green, double blue, double intensity, TimeSpan animationTime, AnimationType type)
        {
            m_base.Animate(red, green, blue, intensity, animationTime, type);
        }

        internal AnimatedLedBase GetBase()
        {
            return m_base;
        }
    }
}
