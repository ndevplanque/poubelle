# Smart Trash Cans Project

## Overview
The Smart Trash Cans project is an initiative by the Mayor of Gotham City to modernize and improve the city's waste management system, addressing a significant pain point for its residents. The project focuses on developing an AR (Augmented Reality) app for smartphones that enhances the waste collection process through advanced visualization and IoT integration.

## Features

### AR App Functionalities
1. **Garbage Truck Path Overlay**:
   - The app displays the path that garbage trucks will follow to collect waste. This path is static and does not change dynamically.

2. **Trashcan Highlighting**:
   - Trashcans are highlighted using 3D models overlaid on the AR map.
   - The app uses image and/or object tracking to accurately place the 3D models.
   - The real-time status of each trashcan (e.g., full or empty) is displayed in AR.

3. **Garbage Truck Simulation**:
   - A 3D model representing the garbage truck is included in the AR app.
   - When triggered by an Arduino signal, the truck model moves along its predetermined path to simulate the collection process.

## Final Defense Requirements
To successfully complete the project, the following elements must be demonstrated during a video conference:

1. **IoT Installation**:
   - Show the setup and integration of IoT devices that communicate with the AR app and provide real-time data on the trashcans.

2. **AR Display**:
   - Demonstrate the AR app's functionalities using a smartphone, showcasing the garbage truck path, trashcan highlighting, and truck simulation.

3. **Demo Preparation**:
   - Ensure that the setup works reliably under various lighting conditions to avoid any issues during the live demonstration.

## Setup and Testing
- **IoT Devices**: Install and configure IoT sensors on the trashcans to monitor their status.
- **AR App**: Develop the AR app with functionalities to track trashcans, display the truck path, and simulate the truck movement.
- **Arduino Integration**: Set up the Arduino to signal the AR app, initiating the garbage truck simulation.
- **Testing**: Thoroughly test the entire setup in different lighting conditions to ensure a smooth and successful live demo.

This project leverages AR and IoT technologies to provide a modern solution for waste management in Gotham City, aiming to enhance the efficiency and user experience of the city's waste collection system.

-----

### TrashBin Script

**Purpose:** 
Handles the state and behavior of a trash bin, including checking its fill state via an API, updating its material, and sending requests to change its state.

**Key Features:**
- Checks and updates the bin's state from an API periodically.
- Changes the visual material based on whether the bin is full or empty.
- Can send requests to mark the bin as empty or full.
- Invokes an event when the bin's state changes.

### TruckController Script

**Purpose:** 
Controls the movement of a truck between checkpoints, detects nearby trash bins, and stops to empty them if they are full.

**Key Features:**
- Moves the truck towards the closest checkpoint.
- Detects full trash bins within a specified radius and stops to empty them.
- Manages checkpoints, deactivating them temporarily after they are reached.
- Resumes movement after emptying a bin.

### ImageTracker Script

**Purpose:** 
Tracks AR images and spawns AR prefabs at detected positions, including initializing and managing the truck.

**Key Features:**
- Spawns AR prefabs at the positions of tracked images.
- Detects the floor plane to position AR objects correctly.
- Initializes the truck at the detected floor height and updates its checkpoints.
- Updates the positions of AR objects based on tracked image changes.