

http://www.copenhagengamecollective.org/projects/unidrone/

Unity + Node.js + AR.drone == UniDrone!


Introduction
-------------------------
UniDrone is an open-source project that couples the AR.drone with Unity. It creates a node.js server that listens to udp messages.

UniMove was created by Patrick Jarnfelt and Tim Garbos of the Copenhagen Game Collective, and used for their Drone concert.
The drone concert uses UniMove to utilize the move controllers and sends these commands to the node.js server. 
The node.js server is using the ar-drone library to control the drone based on the Unity inputs received. 
Our server then relays the the drone's navigation data to an external sound system through OSX. This sound system is then built up to understand and respond to the navData. The sound system is not part of this library.

The project is licensed under the MIT License (MIT), and is still under development.

Installation instructions
-------------------------
 For the server:
 Open the terminal and navigate to the server folder
 run: npm install (this will install the package dependencies)

How to run
-------------------------
 For the server:
 Open the terminal and navigate to the server folder
 run: node drone_server argument1 argument2

Arguments sent to the app
argument1 = Target ip for the drone (the drone must be connected to the same network as the control computer with a manually set ip address)
argument2 = The index of this drone (0 for first drone, 1 for next etc)

This script only controls one drone. Run one more script in another terminal and change the arguments to control more drones.


Requirements
-------------------------

 * Unity 
 * Node.js installed (run node and npm commands in the terminal to test)