NavigationSimulator
===================
This is the main application in this project. This application can act as a simulator and as a controller to a real-world mobile robot.
The goal is to calculate the motion commands for the robot which take it to the goal position on a path that avoids the static and moving obstacles.

Hardware requirements
---------------------
To use it with a real-world robot, a serial link to the robot must be established. Moreover, a webcam is needed, that is pointed at the floor, 
so it can see the robot and its environment from above. The robot and the goal position must be marked with special markers for the MarkerFinderLib to work.

Simulation mode
---------------
The application can be used as a simulator, without any hardware reqirements. This is the default setting. 
The start position and orientation of the robot, and the position and orientation of the goal position can be adjusted by dragging the object icons in the environment window.
Obstacles can be added by pressing the right mouse button. The size of the obstacles can be adjusted as well.

The training of the MLP is started by pressing the *Start Training* button.

The simulation is started by pressing the *Start Sim* button.

NOTE: the webcam overlay is on by default (Show Camera Image).

