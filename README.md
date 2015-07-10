# Windows Iot Led Driver

Windows Iot LED driver is a Windows runtime component that is designed to abstract the complexity of running a set of LEDS on Windows Iot. It also provides advance LED control features such as animations.

### Main Goals

*	Windows Runtime Component – Allows developers to use this component with any UAP supported language. (C++, C#, JavaScript)
*	Extensible – There are many ways to drive LEDs on a Windows IOT device, this component is designed to be fully extensible so anyone can write a controller for any kind of LED device. 
*	East to Use – The main goal of this component is that the component is easy to use from a consumer standpoint. See the example below to see just how easy it is.
*	Make a NuGet - Hopefully coming soon!

###Example

```
// Create the Controller, this controller requries a latch and blackout pin.
m_controller = new TLC5947Controller(6,5);

// Enable Animation
m_controller.ToggleAnimation(true);

// Create the LED array
m_leds = new AnimatedLed[5];

// Create the LEDs
m_leds[0] = new AnimatedLed(LedType.RBG, true);
m_leds[1] = new AnimatedLed(LedType.RBG, true);
m_leds[2] = new AnimatedLed(LedType.RBG, true);
m_leds[3] = new AnimatedLed(LedType.RBG, true);
m_leds[4] = new AnimatedLed(LedType.RBG, true);

// Associate the LEDs
m_controller.AssoicateLed(0, m_leds[0].GetLed());
m_controller.AssoicateLed(3, m_leds[1].GetLed());
m_controller.AssoicateLed(6, m_leds[2].GetLed());
m_controller.AssoicateLed(9, m_leds[3].GetLed());
m_controller.AssoicateLed(12, m_leds[4].GetLed());

// Aniamte one of them!
// Anaimte to color (255,152,25,255) in one second with a linear animation.
m_leds[0].Animate(255,152,25,255, new TimeSpan(0,0,1), AnimationType.Linear);
```

###Current Supports
*	Software PWM (doesn’t work very well at all)
*	Texas Instruments TLC5947 – I highly recommend this chip; AdaFruit makes a wonder easy to use controller with this chip that [can be found here.](https://www.adafruit.com/products/1429)

###Extending the Basic Controller
There are many ways to drive LEDs from a Windows IOT device, hardware PWM, software PWM, SPI, etc. The base controller controls all of the logic of the LEDs and is easily extensible by consumers to wrap whatever device you’re using. By default the lib comes with a basic software PWM as well as a control for the Texas Instruments TLC5947. For examples of how to write controller extension look at the example code in the repo.
