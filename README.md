# FANUC MR Teleoperation — Unity

Unity project developed as part of the Master's dissertation:

**DEVELOPMENT, INTEGRATION, AND COMPARATIVE EVALUATION OF MIXED REALITY AND CONVENTIONAL ROBOTIC TELEOPERATION SYSTEMS: AN INTEGRATED FRAMEWORK FOR PERFORMANCE, WORKLOAD, USABILITY, AND USER EXPERIENCE**

**Author:** Matheus Nicolás Hernandez

---

## Overview

This repository contains the complete Unity project developed for the Mixed Reality robotic teleoperation system presented in the Master's dissertation.

The system provides a Mixed Reality interface for the teleoperation of a FANUC LR Mate 200iD/7L industrial robot using the Meta Quest 3 headset. The Unity environment is integrated with ROS and MoveIt for robot communication, motion planning, trajectory execution, gripper control, and sensor data exchange.

The Unity application operates as the master system of the teleoperation architecture, allowing the user to interact with the robotic system through an immersive Mixed Reality interface.

---

## Main Features

The Unity project includes:

- Mixed Reality teleoperation using Meta Quest 3
- FANUC LR Mate 200iD/7L digital model
- Unity–ROS communication
- End-effector pose control
- Robot motion planning and trajectory execution commands
- Planned trajectory visualization
- Automatic planning and execution mode
- Collision detection and visual feedback
- Workspace limit detection and feedback
- Haptic feedback
- Audio feedback
- Robotic gripper control
- Distance sensor visualization
- Real-time camera streaming
- Interactive Mixed Reality user interface

---

## System Architecture

The teleoperation system follows a master–slave architecture.

### Master System

The master system consists of:

- Windows computer
- Unity
- Meta Quest 3
- Meta Quest Link
- Mixed Reality teleoperation interface

The user interacts with the robot through the Mixed Reality environment. The target pose of the robot end effector is defined in Unity and transmitted to the ROS environment.

### Slave System

The slave system consists of:

- ROS
- MoveIt
- ROS-Industrial
- FANUC robot communication packages
- Motion planning and trajectory execution nodes
- Gripper control
- Sensor integration

The slave system receives commands from Unity, performs motion planning, communicates with the physical robot, and returns robot states, planned trajectories, and sensor data to the Unity environment.

---

## Requirements

The project was developed and tested using:

- Windows 11
- Unity 2022.3.45f1 LTS
- Meta Quest 3
- Meta Quest Link
- ROS-TCP-Connector
- OpenXR
- XR Interaction Toolkit

A running ROS environment is required for communication with the physical robot, motion planning, trajectory execution, gripper control, and sensor integration.

---

## Repository Contents

This repository contains the files required to open and reproduce the Unity project, including:

- C# scripts
- Unity scenes
- Interactive objects
- 3D models
- Robot models
- Materials
- Mixed Reality interface components
- ROS communication components
- Project settings
- Package configuration files

The main Unity project directories are:

Assets/
Packages/
ProjectSettings/
Library/
Temp/
Logs/
obj/
UserSettings/
.vs/

## Installation

1. Clone this repository.

2. Install Unity Hub.

3. Install Unity 2022.3.45f1 LTS.

4. Open Unity Hub.

5. Select **Add project from disk**.

6. Select the cloned repository directory.

7. Open the project using Unity 2022.3.45f1 LTS.

8. Allow Unity to import the project files and restore the required packages.

9. Connect the Meta Quest 3 headset to the computer using Meta Quest Link.

10. Start the corresponding ROS environment.

11. Configure the ROS connection parameters according to the network configuration of the system.

12. Open the main teleoperation scene.

13. Run the Unity application.

---

## Unity–ROS Communication

Communication between Unity and ROS is implemented using the Unity Robotics ROS-TCP-Connector.

The Unity application exchanges information with the ROS environment for:

- Target end-effector pose transmission
- Robot joint state reception
- Motion planning requests
- Trajectory execution requests
- Planned trajectory visualization
- Automatic planning and execution
- Gripper control
- Distance sensor data reception

The communication architecture enables the Mixed Reality interface to operate as the master system while ROS manages robot communication, motion planning, trajectory execution, and sensor integration.

---

## Robot Platform

The robotic platform used in this project is the FANUC LR Mate 200iD/7L industrial robot.

The Unity environment contains a digital representation of the robotic manipulator used for:

- Robot state visualization
- User interaction
- Target pose definition
- Planned trajectory visualization
- Collision feedback
- Workspace feedback

The physical robot is controlled through the corresponding ROS environment.

---

## Mixed Reality Interface

The Mixed Reality interface was developed for the Meta Quest 3 headset.

The interface allows the user to interact with the robotic system while maintaining visual awareness of the physical environment.

The developed interaction system includes:

- End-effector target manipulation
- Robot visualization
- Planned motion visualization
- Collision warnings
- Workspace limit warnings
- Haptic feedback
- Audio feedback
- Robot visibility controls
- Automatic and manual operation modes
- Gripper interaction
- Sensor visualization

---

## ROS Repository

The corresponding ROS implementation is available in the companion repository:

**FANUC MR Teleoperation — ROS**

https://github.com/matheusnicolashernandez/FANUC_MR_TELEOPERATION_ROS

The ROS repository contains the FANUC robot configuration packages, gripper control scripts, auxiliary nodes developed in Python, Arduino code, and the Docker environment required to reproduce the ROS system.

---

## Research Context

This project was developed as part of a Master's research project investigating the development, integration, and comparative evaluation of Mixed Reality and conventional robotic teleoperation systems.

The research evaluates the teleoperation systems using an integrated framework that considers:

- Performance
- Workload
- Usability
- User experience

The proposed Mixed Reality teleoperation system was experimentally compared with a conventional robotic teleoperation method.

---

## Citation

If you use this project or any part of the developed teleoperation system in academic work, please cite the corresponding Master's dissertation:

> M. Hernandez, *Development, Integration, and Comparative Evaluation of Mixed Reality and Conventional Robotic Teleoperation Systems: An Integrated Framework for Performance, Workload, Usability, and User Experience*. Master's dissertation, forthcoming.

The complete bibliographic reference and official repository link will be added after the dissertation is formally published.

---

