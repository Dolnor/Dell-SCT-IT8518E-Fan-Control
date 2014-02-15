###IT8518E-SCT-FC

Windows Fan Control solution for Dell SecureCore Tiano firmware based laptops with ITE IT8518E embedded controller (EC).

This software is a fork of https://github.com/Gabriel-LG/Acer7551GFanControl with knowledge from https://github.com/Dolnor/DELL-Tiano-IT8518E-Debug 
and algorithms from https://github.com/Dolnor/Vostro-3450-Fan-Override applied to it. The Acer class has been renamed to better reflect the purpose of this software.
Original copyright and license notice have been retained.

## Usage 

For this program to work install TVic Port first. The installer can be found in the binaries folder.

The program sits in tray upon launch and outputs the maximum temperature from 3 sensor readings into tray icon. 
On AMD Switchable Graphics and nVidia Optimus based laptops the program will output CPU, GPU and PCH temperatures. 
On IGD (Intel) laptops GPU temperature will not be shown as there's no EC register for that.

The visual representation of fan speed is shown as a progress bar that is being filled in the range from 0 to 100%, 100% being 5300 rpm 
(typical max speed of SUNON brand fans used by Dell according to ePSA).

Hovering over the tray icon will bring up the name of currently active profile, sensor reading as well as current fan speed.

Right clicking the icon brings up profiles defined in config.xml as well as default automatic mode option, choosing which will reset the fan to default behaviour.
Upon exiting the application the automatic mode is being restored too! But not upon soft restarting the laptop! The fan speed bar will colorize depending on the active fan mode. If bios has control over fan the bar will be green, otherwise it will be white.


## Configuration

The configuration in handled through use of config.xml which you can edit according to your desired fan behaviour. 
When running on battery neither of these setting will have an effect. There are 6 fields total that you can alter. You can have as many profiles as you want too...

  		<profile default="true">
    		    <name>Audible</name>
    	   	    <interval>1000</interval>
    		    <safetemp>52</safetemp>
		    <triptemp>62</triptemp>
	  	    <steadyspeed>3000</steadyspeed>
  		</profile>

- default="true" property for given profile will enable it at program start. If there are not profiles marked as default automatic control will be used.

- name property as implied, is meant to specify a profile name that will appear in the list when you right click the tray icon.

- interval should be in range from 1sec to 2sec (1000-2000ms), this property defines the time interval in between EC data checks.

- safetemp is treated as the lowest average temperature from 16 iterations to initiate manual control. If temps is higher than safe_temp automatic control will be retained.

- triptemp is the highest temperature the manual control mode can be kept. If average temperature surpasses the trip temperature automatic control mode will be restored for system to cool down.

- steadyspeed is the desired rpm value that the fan should stick at when safe_temp threshold is met. On Vostro 3450 setting speed below 3000 won't stick, the fan will immediately drop rpm to 0 if you go past 2600 mark.
If you set this to 0 a passive cooling profile will be initiated, meaning the fan won't run at all times, but will only enable when average temp has reached a trip point.

If you need the program to start with a delay (because the port driver takes some time to initialize) you can specify the -delay XX in the shortcut that you put in your Startup folder. Here XX is the number of seconds to delay the start of the application.
