using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    // Defines what kind of LED this is.
    public enum LedType
    {
        SingleColor,
        RBG
    }

    // Defines a the public LED class.
    public sealed class Led
    {
        //
        // Public getters and setters
        //
        public double Red
        {
            get { return m_base.Red; }
            set { m_base.Red = value; }
        }
        public double Green
        {
            get { return m_base.Green; }
            set { m_base.Green = value; }
        }
        public double Blue
        {
            get { return m_base.Blue; }
            set { m_base.Blue = value; }
        }
        public double Intensity
        {
            get { return m_base.Intensity; }
            set { m_base.Intensity = value; }
        }

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
