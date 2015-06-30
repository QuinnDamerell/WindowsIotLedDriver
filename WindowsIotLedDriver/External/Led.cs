

namespace WindowsIotLedDriver
{
    /// <summary>
    /// Defines they type of LED. 
    /// </summary>
    public enum LedType
    {
        SingleColor,
        RBG
    }

    /// <summary>
    /// Defines the basic LED class. This class is the root of any LED and is used to
    /// change the basic color and intensity of the LED
    /// </summary>
    public sealed class Led
    {
        //
        // Public getters and setters
        //

        /// <summary>
        /// Gets or sets the red value for the LED.
        /// </summary>
        public double Red
        {
            get { return m_base.Red; }
            set { m_base.Red = value; }
        }
        /// <summary>
        /// Gets or sets the green value for the LED.
        /// </summary>
        public double Green
        {
            get { return m_base.Green; }
            set { m_base.Green = value; }
        }
        /// <summary>
        /// Gets or set the blue value for the LED.
        /// </summary>
        public double Blue
        {
            get { return m_base.Blue; }
            set { m_base.Blue = value; }
        }
        /// <summary>
        /// Gets or sets the intensity value for the LED
        /// </summary>
        public double Intensity
        {
            get { return m_base.Intensity; }
            set { m_base.Intensity = value; }
        }

        /// <summary>
        /// Gets the type of the LED
        /// </summary>
        public LedType Type
        {
            get { return m_base.Type; }
        }

        //
        // Private vars
        //

        // Holds a reference to the base LED class that backs this external class.
        private LedBase m_base;
        
        //
        // Constructor
        //      

        /// <summary>
        /// Creates a new LED object given the LED type
        /// </summary>
        /// <param name="type">The type of LED that should be created</param>
        public Led(LedType type)
        {
            m_base = new LedBase(type);
        }

        // Returns a reference to the base class
        internal LedBase GetBase()
        {
            return m_base;
        }
    }
}
