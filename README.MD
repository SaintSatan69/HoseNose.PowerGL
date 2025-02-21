# HoseNose.PowerGL

#### A picture captured during the development of the engine
![alt text](https://github.com/SaintSatan69/HoseNose.PowerGL/blob/Dev-Branch/GitHubAssets/PowerShellOpenGL.png "The Engine Rendering a few Powershells During Testing")

***
# Warning
PowerGL is VERY alpha, so very buggy and prone to quirks. Im a sysadmin by day so I can only work on this after work when I get time to so development is slow.

***
# About
### What is PowerGL
PowerGL is a PowerShell module/C# program that allows someone to use powershell to leverage technology like OpenGL to render 3D enviroments with powershell acting as a controller. It is 10% pure powershell and 90% C#, this is because powershell doesn't work with the Silk.Net libraries to make the window. Powershell is used as the control plane of the rendering engine (C# side) so that the engine itself can remain slim to do what it does better.
### Why?
why not? I see and use powershell for practicle applications (new users, Departing users) in the day job, and one day I wondered what Non-practicle use for powershell I could make and stumbled on someone who did a little similar but at that point it was several years old PowerShell OpenGL.

### Prerequisites
1. Currently requires Powershell 7-preview to be installed. Used While Testing and it messed with the powershell module variable of PS 7.5.

2.  May only work (for now until the engine config.json is ever dreamed up) if placed into the Power7-preview module folder as there is a hardcoded path in the engine for provided assets, that the engine will fail back to as defaults if something manages to slip through the cracks. its only pointed to ps 7 preview for reason in 1.

### How to use
1. First in the module folder there is a script called rendergoldps.ps1 Run this since its a built out script I used while testing to make sure everything is working.

2. If that script works either open the script up and attempt to read it or keep reading.

3. First make a shape counter, this will help keep track of things and can be used to incrent the shape number for the engine.

4. Second make an Array using @() this is where the shapes will be loaded. (if someone wants to make it better go for it)

5. use```PS> New-PowerGLShape -shapenum <ShapeCounter> ... -PosZ 1.0``` to make your desired shape make sure to look at all the parameters available.

6. Take the output of the New-PowerGLShape and += into the array and increment the counter.

7. After you all the shapes you would like Use the following functions
```PS> Initialize-PowerGL -phase 1``` and ```PS> Initialize-PowerGL -phase 2 -shapesarray <your array>``` these functions prepares the ground work that the engine needs to get your shapes into it.

8. After getting the ground work we can now use the ```PS> Start-PowerGL``` function the parameter -programflag is used to enable certain features and can be ignored, if "DEV_BUGLAND" (exp features) is left in the script the program will await the visual studio debugger to attach into the process so be warned.

9. If you want to post launch of the engine move shapes use the correct program flag and the following ```PS> $Pipe = Initialize-PowerGL -phase 3``` This function builds a named pipe server to talk between powershell and the engine.

10.  With the pipe you can use ```PS> $pipe.WriteDirective((Move-PowerGLShape -scale <if SCALE is the property> -X <for anything other then scale, applies to -Y, -Z> -Y -Z -shapenum <shape number to be chaged> -property <Property to be moved [TRANSFORM,ROTATE,SCALE]>))```

11.  When a directive is written to the pipe it will return the length in bytes < * 2 per char> that it has written, you can usually ignore it if you would like. You can also call the move-powerGLshape to see the string it writes into the pipe if you would like to build your own function to build them.