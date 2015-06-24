using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    class LedControllerBase : ILedChangeListener, IAnimationTickListner
    {
        //
        // Private vars
        //

        // Holds the associated LEDs and their positions
        private Dictionary<int, WeakReference<Led>> m_ledMap;

        // Holds a reference to the derived 
        IControllerStatChangeListener m_stateChangeListener;
        ControlerUpdateType m_updateType;

        // Animation vars
        TheAnimator m_theAnimator;
        Object animationLock = new Object();
        bool m_animationEnabled;
        bool m_alwaysPaint;
        List<WeakReference<IAnimationTickListner>> m_animationListeners;

        // 
        // Constructor
        //
        public LedControllerBase(IControllerStatChangeListener extendedController, ControlerUpdateType updateType)
        {
            m_stateChangeListener = extendedController;
            m_updateType = updateType;
            m_animationEnabled = false;
            m_alwaysPaint = false;
            m_ledMap = new Dictionary<int, WeakReference<Led>>();
            m_animationListeners = new List<WeakReference<IAnimationTickListner>>();
        }

        //
        // Public functions
        //

        // Binds an LED object to a controller.
        public void AssoicateLed(int startingSlot, Led assoicateLed)
        {
            if (assoicateLed == null)
            {
                throw new ArgumentNullException("Led can't be null!");
            }
            if(startingSlot < 0)
            {
                throw new ArgumentOutOfRangeException("The LED slot can't be < 0!");
            }

            // Get how many slots are needed
            int slotsNeeded = assoicateLed.GetBase().SlotsRequired;

            lock (m_ledMap)
            {
                // Make sure there is enough room in the position requested to
                // hold the new LED                
                for (int i = 0; i < slotsNeeded; i++)
                {
                    // Check if something exists at that key
                    if (m_ledMap.ContainsKey(i + startingSlot))
                    {
                        // If it does, see if it is still valid
                        Led tempLed;
                        if (m_ledMap[i + startingSlot].TryGetTarget(out tempLed))
                        {
                            // The LED actually exists, we can't add the LED here
                            throw new ArgumentException("The requested position is already occupied by a LED. Remember RBG LEDs take 3 slots!");
                        }
                        else
                        {
                            // If we get here we have an entry for a LED that is gone. Remove it
                            m_ledMap.Remove(startingSlot + i);
                        }
                    }
                }
            }

            // Inform the listener that slots need to be added. The listener should throw if
            // this fails.
            m_stateChangeListener.NotifiySlotsAdded(startingSlot, slotsNeeded);

            // Now actually add the LEDs to the map.
            lock(m_ledMap)
            {
                for (int i = 0; i < slotsNeeded; i++)
                {
                    m_ledMap.Add(startingSlot + i, new WeakReference<Led>(assoicateLed));
                }
            }           
            
            // Inform the LED of the association
            assoicateLed.GetBase().SetNotificationCallback(startingSlot, this);
        }

        // Breaks the association with a LED and the controller
        public void DissociateLed(Led dissociateLed)
        {
            if(dissociateLed == null)
            {
                throw new ArgumentNullException("The LED can't be null!");
            }

            int startingPosition = dissociateLed.GetBase().FirstSlot;
            int slotsFilled = dissociateLed.GetBase().SlotsRequired;

            // Remove the LED from the map
            lock (m_ledMap)
            {
                for (int i = 0; i < slotsFilled; i++)
                {
                    m_ledMap.Remove(startingPosition + i);
                }
            }

            // Remove the association with the LED
            dissociateLed.GetBase().RemoveNotificationCallback();

            // Tell the listener last, if they fail we still want to do our logic
            m_stateChangeListener.NotifiySlotsRevmoed(startingPosition, slotsFilled);
        }

        public void ToggleAnimation(bool enableAnmation, bool alwaysPaint, int animationRateMilliseconds = 100)
        {
            lock(animationLock)
            {
                m_animationEnabled = enableAnmation;
                m_alwaysPaint = alwaysPaint;

                if (m_animationEnabled)
                {
                    // Make an animator, this will start the thread
                    m_theAnimator = new TheAnimator(this, animationRateMilliseconds);
                }
                else
                {
                    // Kill the animator, this will stop the thread
                    m_theAnimator.Stop();
                    m_theAnimator = null;
                }
            }
        }

        // Returns a list of the values each slot should be set to.
        private List<double> GetAllSlotValues()
        {
            List<double> slotValues = new List<double>();

            lock(m_ledMap)
            {
                int currentUsedSlotCount = m_ledMap.Count;
                int foundUsedSlots = 0;
                int currentSlotPosition = 0;
                
                // Loop through the map until we find all of the LEDs.
                // Note! Some of the slots might be empty, we need to fill those with 0s
                while(foundUsedSlots < currentUsedSlotCount)
                {
                    // Check if the map has a LED for this entry
                    if(m_ledMap.ContainsKey(currentSlotPosition))
                    {
                        // Try to get the LED
                        Led currentLed;
                        if(m_ledMap[currentSlotPosition].TryGetTarget(out currentLed))
                        {
                            // We got an LED!
                            LedType type;
                            double red, green, blue, intensity;
                            currentLed.GetBase().GetLedState(out type, out red, out green, out blue, out intensity);

                            // Add the values to the slots
                            if(type == LedType.SingleColor)
                            {
                                slotValues.Add(intensity);
                                currentSlotPosition++;
                                foundUsedSlots++;
                            }
                            else
                            {
                                slotValues.Add(red * intensity);
                                slotValues.Add(green * intensity);
                                slotValues.Add(blue * intensity);
                                currentSlotPosition += 3;
                                foundUsedSlots += 3;
                            }
                        }
                        else
                        {
                            // The LED is gone now. Remove it and add a 0 to the slotValues
                            m_ledMap.Remove(currentSlotPosition);
                            slotValues.Add(0);
                            currentSlotPosition++;
                        }
                    }
                    else
                    {
                        // No LED exits in that slot, fill it with a 0
                        slotValues.Add(0);
                        currentSlotPosition++;
                    }
                }
            }

            // Return the values!
            return slotValues;
        }

        // Called when an LEDs value changes. The argument passed is the first slot used by the LED,
        // regardless which slot in the LED changed.
        public void NotifiyLedChange(int baseSlot, LedChangeValue changedValue)
        {
            if (!m_ledMap.ContainsKey(baseSlot))
            {
                throw new ArgumentException("Invalid Slot Number!");
            }

            // Figure out how we want to update, if we need to update everything or just a slot
            if(m_updateType == ControlerUpdateType.AllSlots)
            {                
                if(m_animationEnabled)
                {
                    // If the animation thread is running it will take care of pushing updates.
                    return;
                }
                else
                {
                    // We need to tell the listener of the new state
                    m_stateChangeListener.NotifiySlotsStateChanged(GetAllSlotValues());
                }
            }
            else
            {
                // The controller is single slot update based. We need to send the change to the listener
                // Get the LED for the changed slot
                Led changedLed;
                if (m_ledMap[baseSlot].TryGetTarget(out changedLed))
                {
                    LedType type;
                    double red, green, blue, intensity;
                    changedLed.GetBase().GetLedState(out type, out red, out green, out blue, out intensity);

                    if (type == LedType.SingleColor)
                    {
                        m_stateChangeListener.NotifiySlotStateChanged(baseSlot, intensity);
                    }
                    else
                    {
                        // Send the correct update for the color
                        switch(changedValue)
                        {
                            case LedChangeValue.Red:
                                m_stateChangeListener.NotifiySlotStateChanged(baseSlot, red * intensity);
                                break;
                            case LedChangeValue.Green:
                                m_stateChangeListener.NotifiySlotStateChanged(baseSlot + 1, green * intensity);
                                break;
                            case LedChangeValue.Blue:
                                m_stateChangeListener.NotifiySlotStateChanged(baseSlot + 2, blue * intensity);
                                break;
                            case LedChangeValue.All:
                                m_stateChangeListener.NotifiySlotStateChanged(baseSlot, red * intensity);
                                m_stateChangeListener.NotifiySlotStateChanged(baseSlot + 1, green * intensity);
                                m_stateChangeListener.NotifiySlotStateChanged(baseSlot + 2, blue * intensity);
                                break;
                        }            
                    }
                }
                else
                {
                    throw new Exception("The LED is gone!");
                }
            }
        }

        public void ResigerForAnimationCallbacks(IAnimationTickListner listener)
        {
            m_animationListeners.Add(new WeakReference<IAnimationTickListner>(listener));
        }

        // The call back from the animator
        public bool NotifiyAnimationTick(int timeElapsedMs)
        {
            // Loop through all of the registered LED classes. Keep track if there were any updates
            // on this animation tick.
            bool wasUpdate = false;
            foreach (WeakReference<IAnimationTickListner> weakListener in m_animationListeners)
            {
                IAnimationTickListner listener;
                if(weakListener.TryGetTarget(out listener))
                {
                    bool updated = listener.NotifiyAnimationTick(timeElapsedMs);
                    if(!wasUpdate)
                    {
                        wasUpdate = updated;
                    }
                }
            }

            // Check our update type
            if (m_updateType == ControlerUpdateType.AllSlots)
            {
                // if the update type is all slots and we are animating we will ignore
                // updates while animating so we can do the update once at the end.
                // Update the state if something changed or we always should.
                if(wasUpdate || m_alwaysPaint)
                {
                    m_stateChangeListener.NotifiySlotsStateChanged(GetAllSlotValues());
                }
            }
            
            return true;
        }
    }
}
