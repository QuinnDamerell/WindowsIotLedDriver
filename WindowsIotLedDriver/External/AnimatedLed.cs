using System;

namespace WindowsIotLedDriver
{
    public enum AnimationType
    {
        Linear,
    }

    public sealed class AnimatedLed
    {
        public LedType Type
        {
            get { return m_base.GetLed().Type; }
        }

        // Todo fix
        internal const double INGORE_VALUE = -1;

        //
        // Private Vars
        //
        AnimatedLedBase m_base;

        public AnimatedLed(LedType type)
        {
            m_base = new AnimatedLedBase(type);
        }

        public AnimatedLed(Led led, bool boolThatMeansNothingForWinRt)
        {
            m_base = new AnimatedLedBase(led);
        }

        public Led GetLed()
        {
            return m_base.GetLed();
        }

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
