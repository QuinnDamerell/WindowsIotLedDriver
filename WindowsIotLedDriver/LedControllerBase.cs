using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    internal class LedControllerBase : ILedChangeListener, IAnimationTickListner
    {
        //
        // Private vars
        //
        double m_masterIntensity = 0;
        double m_desiredMasterIntensity = 0.0;
        double m_startMasterIntensity = 0.0;
        int m_animationRemainIntensity = -1;
        uint m_animationLengthIntensity = 0;

        // Holds the associated LEDs and their positions
        private Dictionary<int, WeakReference<Led>> m_ledMap;

        // Holds a reference to the extender
        ILedControllerExtender m_controllerExtener;
        ControlerUpdateType m_updateType;

        // Animation vars
        TheAnimator m_theAnimator;
        Object animationLock = new Object();
        bool m_animationEnabled;
        bool m_alwaysPaint;
        bool m_isDirty;
        List<WeakReference<IAnimationTickListner>> m_animationListeners;

        // 
        // Constructor
        //
        public LedControllerBase(ILedControllerExtender extendedController, ControlerUpdateType updateType)
        {
            if(extendedController == null)
            {
                throw new Exception("A controller extender is required!");
            }
            
            m_controllerExtener = extendedController;
            m_updateType = updateType;
            m_animationEnabled = false;
            m_alwaysPaint = false;
            m_isDirty = false;
            m_ledMap = new Dictionary<int, WeakReference<Led>>();
            m_animationListeners = new List<WeakReference<IAnimationTickListner>>();
            m_masterIntensity = 1.0;
        }

        //
        // Public functions
        //

        // Binds an LED object to a controller.
        public void AssociateLed(int startingSlot, Led assoicateLed)
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
            m_controllerExtener.NotifiySlotsAdded(startingSlot, slotsNeeded);

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
            m_controllerExtener.NotifiySlotsRevmoed(startingPosition, slotsFilled);
        }

        public void ToggleAnimation(bool enableAnmation, bool alwaysPaint, int animationRateMilliseconds = 16)
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
                                slotValues.Add(intensity * m_masterIntensity);
                                currentSlotPosition++;
                                foundUsedSlots++;
                            }
                            else
                            {
                                slotValues.Add(red * intensity * m_masterIntensity);
                                slotValues.Add(green * intensity * m_masterIntensity);
                                slotValues.Add(blue * intensity * m_masterIntensity);
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
                    m_isDirty = true;
                    return;
                }
                else
                {
                    // We need to tell the listener of the new state
                    m_controllerExtener.NotifiySlotsStateChanged(GetAllSlotValues());
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
                        m_controllerExtener.NotifiySlotStateChanged(baseSlot, intensity * m_masterIntensity);
                    }
                    else
                    {
                        // Send the correct update for the color
                        switch(changedValue)
                        {
                            case LedChangeValue.Red:
                                m_controllerExtener.NotifiySlotStateChanged(baseSlot, red * intensity * m_masterIntensity);
                                break;
                            case LedChangeValue.Green:
                                m_controllerExtener.NotifiySlotStateChanged(baseSlot + 1, green * intensity * m_masterIntensity);
                                break;
                            case LedChangeValue.Blue:
                                m_controllerExtener.NotifiySlotStateChanged(baseSlot + 2, blue * intensity * m_masterIntensity);
                                break;
                            case LedChangeValue.All:
                                m_controllerExtener.NotifiySlotStateChanged(baseSlot, red * intensity * m_masterIntensity);
                                m_controllerExtener.NotifiySlotStateChanged(baseSlot + 1, green * intensity * m_masterIntensity);
                                m_controllerExtener.NotifiySlotStateChanged(baseSlot + 2, blue * intensity * m_masterIntensity);
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

            // Animate the intensity
            wasUpdate = AnimateIntensity(timeElapsedMs);

            // Check our update type, if we updated the animation 
            if (m_updateType == ControlerUpdateType.AllSlots)
            {
                // if the update type is all slots and we are animating we will ignore
                // updates while animating so we can do the update once at the end.
                // Update the state if something changed or we always should.
                if(wasUpdate || m_alwaysPaint || m_isDirty)
                {
                    m_isDirty = false;
                    m_controllerExtener.NotifiySlotsStateChanged(GetAllSlotValues());
                }
            }
            else
            {
                // #todo handle intensity updates for per slot modes.
            }
            
            return true;
        }

        public void AnimateMasterIntensity(double intensity, TimeSpan animationTime)
        {
            if (intensity < 0 || intensity > 1.0)
            {
                throw new ArgumentOutOfRangeException("The intensity must be between 0.0 and 1.0");
            }

            m_desiredMasterIntensity = intensity;
            m_startMasterIntensity = m_masterIntensity;
            m_animationLengthIntensity = (uint)animationTime.TotalMilliseconds;
            m_animationRemainIntensity = (int)animationTime.TotalMilliseconds;
        }

        private bool AnimateIntensity(int timeElapsedMs)
        {
            // Update Intensity
            // Check if the values are equal and if the animation is still running
            bool didWork = false;
            if (!AreCloseEnough(m_desiredMasterIntensity, m_masterIntensity) && m_animationRemainIntensity >= 0)
            {
                // Remove the elapsed time
                m_animationRemainIntensity -= timeElapsedMs;

                if (m_animationRemainIntensity > 0)
                {
                    // Figure out what the progress is
                    double animationProgress = 1 - ((double)m_animationRemainIntensity / (double)m_animationLengthIntensity);

                    // Set the led to the 
                    m_masterIntensity = m_startMasterIntensity + ((m_desiredMasterIntensity - m_startMasterIntensity) * animationProgress);
                }
                else
                {
                    // We are done with the animation.
                    m_masterIntensity = m_desiredMasterIntensity;
                    m_animationRemainIntensity = -1;
                }

                didWork = true;
            }
            else
            {
                // If we don't need to animate anymore, kill the time.
                m_animationRemainIntensity = -1;
            }         

            return didWork;
        }

        // Checks if two double are close enough
        private bool AreCloseEnough(double a, double b)
        {
            return Math.Abs(a - b) < 0.0001;
        }

        public double GetMasterIntensity()
        {
            return m_masterIntensity;
        }

        public bool IsMasterIntensityAnimating()
        {
            return m_animationRemainIntensity != -1;
        }
    }
}
