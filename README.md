# SimpleAutoclose
Simple Auto Close Door Script for Space Engineers
Thomas LaFreniere aka laftho
v1.0 - May 29, 2016

This script is will simply auto close doors on your grid after
300ms if it has the [autoclose] tag in the name.

Setup the Programmable Block with an Timer Block loop on Trigger Now.

Default arguments:
Key: [autoclose]
Delay: 300ms

Run script with optional arguments:
key,delay

E.g.

argument: [fancyDoor],200

This will find all doors tagged [fancyDoor] and auto close after 200ms.

If you wish to close any doors, omit the key in the arguments and simply
provide a delay integer.

E.g.

argument: 150

This will auto close all doors after 150ms.

Note it takes 100ms for the door animation to go from closed to open. Unless
you want impossible to open doors, set this value above 100ms.
