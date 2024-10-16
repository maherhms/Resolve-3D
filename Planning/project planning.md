# Project Architecture Documentation

## Overview
The **Resolve-3D** game engine project provides tools for creating and managing 3D game projects, including scene and entity management. The project follows an MVVM (Model-View-ViewModel) architecture with a focus on modular and reusable components.

## Core Components

### 1. **Utilities**
   - **Serializer.cs**: Handles serialization and deserialization of game data.
   - **UndoRedo.cs**: Manages undo and redo operations within the application.

### 2. **Components**
   - **Component.cs**: Base class for all components attached to game entities.
   - **GameEntity.cs**: Represents entities in the game, containing multiple components.
   - **Transform.cs**: A component representing position, rotation, and scale of a game entity.

### 3. **GameProject**
   - **NewProject.cs**: Handles the logic for creating new game projects.
   - **Project.cs**: Represents a game project, containing scenes and project-level settings.
   - **Scene.cs**: Represents a scene within a project, containing game entities and their interactions.

## UI Components

### 1. **Main Window**
   - **MainWindow.xaml**: The main UI layout, providing access to project management, scene editing, and entity manipulation.
   - **Toolbars and Menus**: Provide quick access to common actions like creating projects, adding scenes, and managing entities.
   - **Project Explorer**: Allows navigation between projects and scenes.
   - **Properties Panel**: Displays properties of selected entities and allows modification.

## Data Flow

1. **Project Creation**
   - User initiates project creation via `NewProject.cs`.
   - `NewProject.cs` logic creates and initializes a new `Project` instance.
   - The `Project` is then loaded into the UI via the `ProjectViewModel`.

2. **Adding Scenes**
   - User adds a new scene via the `SceneViewModel`.
   - `SceneViewModel` creates a new `Scene` instance through `Scene.cs`.
   - The new scene is added to the current `Project`.

3. **Managing Entities**
   - User interacts with entities via the `SceneViewModel`.
   - `GameEntity.cs` handles creation, updates, and deletion of entities.
   - `Entity` instances are configured with various components like `Transform`.

4. **Undo/Redo Operations**
   - Actions are recorded and managed by `UndoRedo.cs`.
   - `MainViewModel` handles undoing and redoing actions based on user inputs.

## Interaction Diagram

1. **User** interacts with **MainWindow.xaml**.
2. **MainWindow.xaml** sends commands to the **ViewModels** (`MainViewModel`, `ProjectViewModel`, `SceneViewModel`).
3. **ViewModels** update the **Models** (`Project`, `Scene`, `GameEntity`).
4. **Models** manage application data and logic.
5. **ViewModels** update the **UI** based on changes to the models.

## Use Cases

1. **Create Project**
   - User selects "New Project" from the toolbar.
   - `NewProject.cs` handles the project creation process.
   - `Project.cs` is initialized and loaded into the UI.

2. **Add Scene**
   - User selects "Add Scene" from the context menu.
   - `Scene.cs` handles the scene creation process.
   - The new scene is added to the project.

3. **Add Entity**
   - User adds an entity within a scene.
   - `GameEntity.cs` handles the entity creation process.
   - The new entity is configured with components like `Transform`.

4. **Undo Action**
   - User performs an action, such as adding an entity.
   - `UndoRedo.cs` records the action.
   - User clicks "Undo" to reverse the action.
   - `MainViewModel` applies the undo operation.
