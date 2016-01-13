RobotNavigation
===============
This is a research project for robotic navigation with neural networks written in C# and C. The goal is to navigate a mobile robot between static and moving obstacles by performing the motion planning with an MLP.

The solution contains several projects, including two C libraries, one very lightweight [MLP implementation](https://github.com/hunsteve/RobotNavigation/tree/master/MLPDll), and the [image processing library](https://github.com/hunsteve/RobotNavigation/tree/master/MarkerFinderLib) that is used for the localization. See the specific readme files in the project directories.

Motion planning
---------------
The motion controlling MLP is trained online with an extended backpropagation through time algorithm (BPTT), which uses potential fields for obstacle avoidance. The paths of the moving obstacles are predicted with other MLPs. 

Read more here: [István Engedy, Gábor Horváth: *Artificial Neural Network based Mobile Robot Navigation*](http://home.mit.bme.hu/~engedy/docs/WISP_2009_NN_RobotNav.pdf)

Localization
------------
The localization is performed with a camera- and image processing-based global indoor localization system for the robot and all other objects in its surrounding environment. The system is able to locate different objects marked with simple marker shapes, and any other objects in the working area. The method is also able to keep track of moving objects, and predict their future positions. Fourier transformation is used to determine the main localization parameters of the marker shapes. 

Read more here: [István Engedy, Gábor Horváth: *Global, camera-based localization and prediction of future positions in mobile robot navigation*](http://home.mit.bme.hu/~engedy/docs/SpringerBook_2010_CameraLocalization.pdf)

Installation and build
----------------------
1. Clone from git
2. Open the solution with Visual Studio 2008 or later
3. Build the solution (release, x86)
4. Select the project NavigationSimulator as startup project, and run it

Videos
------
1. https://www.youtube.com/watch?v=mY1dQg8ODJk
2. https://www.youtube.com/watch?v=cIKDyRJl37U
3. https://www.youtube.com/watch?v=1-YcDvLpwv4
4. https://www.youtube.com/watch?v=SfX7dxyd4Mo
