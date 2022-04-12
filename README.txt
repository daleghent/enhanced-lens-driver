## Enhanced Astromechanics Canon Lens Driver

Website: https://daleghent.com/astromechanics-aperture-control

This ASCOM driver controls the Canon EF lens focuser by Astromechanics. It operates the focuser in much the same way as the official ASCOM driver from Astromechanics, but offser addtional functionality that can be leveraged by astro-imaging applications.

The Enhanced Astromechanics Canon Lens Driver implements four ASCOM device actions:

1. GetApertureIndex - returns a stringified index number of currently active aperture
2. GetFocalRatioList - returns a colon-separated string of focal ratios that are available for the configured lens model (example: "f/2.8:f/3.5:f/4.0:...")
3. GetLensModel - returns the name of the lens that is currently selected
4. SetAperture - takes an action paramter of a stringified integer aperture index number to set the lens to

These ASCOM device actions can be used by astro-imaging apps to get aperture-related information from the focuser and set the focal ratio of the lens while it is inoperation. This can remove the need for the user to manually reconfigure the driver for a different focal ratio if mulltiple focal ratios are desired over the course of an imaging session. These device actions are advertised via the SupportedActions driver property.

An example of a system that uses these features is the Astromechanics Aperture Control plugin for NINA 2.0. This plugin introduces interactive user controls and instructions for controlling the focal ratio of the lens directly from with the Advanced Sequencer. The Astromechanics Aperture Control plugin depends on this Enhanced driver to operate.