# Virtual Underwater Window Project

Welcome to the **Virtual Underwater Window** repository, where I explore the integration of real-time face tracking with immersive 3D environments using Python and Unity. This project represents a fusion of computer vision and interactive graphics, creating an illusion of looking through a virtual window into an underwater world. It is a culmination of both individual effort and collaborative insights, reflecting a journey into advanced programming techniques and multimedia design.

---

## Repository Overview

This repository is structured to provide a comprehensive view of the project's development, including source code, documentation, and analytical reports. The primary focus is on how face tracking data from a webcam can control camera movement within a Unity scene to enhance the perception of depth and immersion.

### Directories:

- **Python Face Tracking**: Contains the Python script utilizing MediaPipe for real-time face tracking and sending positional data to Unity via UDP sockets.
- **Unity Scripts**: Includes C# scripts for Unity that handle camera movement, fish animations, and data reception from the Python script.
- **Documentation**: Features the `GUI_Ocean.pdf` report, which provides an in-depth explanation of the project's objectives, methodologies, and technical implementation.

---

## Project Summary

### Virtual Underwater Window

**Objective**: To create an interactive underwater scene in Unity that responds to the user's head movements, providing a realistic 3D experience on a 2D screen.

**Key Components**:

- **Face Tracking with Python**:
  - Utilizes OpenCV and MediaPipe to capture and process real-time facial landmark data from a webcam.
  - Normalizes and smooths the positional data to calculate deltas relative to the initial head position.
  - Sends the processed data to Unity using UDP sockets for immediate interaction.

- **Unity Underwater Scene**:
  - Implements a dynamic underwater environment with swimming fish and environmental effects.
  - Uses the received face tracking data to adjust the camera's position, simulating a window into the underwater world that changes perspective based on user movement.
  - Animates fish using scripts to create natural and varied movement patterns within defined boundaries.

**Scripts Included**:

- **Python Script**:
  - `facetracking.py`: Captures webcam input, processes face tracking, and communicates with Unity.

- **Unity C# Scripts**:
  - `CameraController.cs`: Adjusts the camera based on face tracking data.
  - `EyeTrackingReceiver.cs`: Receives and parses data from the Python script.
  - `FishMovement.cs`: Controls fish behavior and movement within the scene.

---

## Documentation and Analysis

The project is thoroughly documented in the `GUI_Ocean.pdf` report, which includes:

- **Introduction**: Overview of the project's goals and significance in the field of interactive media.
- **Technical Implementation**:
  - Detailed explanation of the face tracking process using MediaPipe.
  - Communication protocol between Python and Unity.
  - Description of Unity scene setup and scripting logic.
- **Challenges and Solutions**: Discussion of any obstacles encountered during development and the strategies employed to overcome them.
- **Conclusion**: Reflections on the project's outcomes and potential future enhancements.

---

## Demonstration

Experience the Virtual Underwater Window in action:

- **Video Demonstration**: [Click here to watch the project in action](https://www.youtube.com/watch?v=sAhsR06ssLA).
