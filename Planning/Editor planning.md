# Editor Project Architecture Documentation

## Overview
The **Resolve-3D** game engine provides tools for creating and managing 3D game projects with scene and entity control. The project follows an MVVM architecture focused on modular and reusable components.

## Core Components
### 1. **Utilities**
- **Serializer.cs**: Manages serialization and deserialization of game data.
- **UndoRedo.cs**: Manages undo and redo operations across multiple actions, such as entity renaming and enabling/disabling.
- **Logger.cs**: Logs application events, errors, and information.

### 2. **Components**
- **Component.cs**: Base class for components attached to game entities.
- **GameEntity.cs**: Represents entities in the game with components, like `Transform`, and tracks their `IsEnabled` and `Name` properties.
- **Transform.cs**: Component representing position, rotation, and scale of a game entity.
- **MSEntity.cs**: Manages multi-selection operations for game entities, supporting synchronized changes, including renaming and enabling/disabling.

### 3. **GameProject**
- **NewProject.cs**: Handles the logic for creating new game projects.
- **Project.cs**: Represents a game project containing scenes and project-level settings.
- **Scene.cs**: Represents a scene within a project, holding game entities and their interactions.

## UI Components
### 1. **Main Window**
- **MainWindow.xaml**: Primary UI layout, providing access to project management, scene editing, and entity manipulation.
- **Toolbars and Menus**: Quick access for creating projects, adding scenes, managing entities, and performing undo/redo actions.
- **Project Explorer**: Allows navigation between projects and scenes.
- **Properties Panel**: Displays and allows modification of selected entities' properties, including name and enabled status.

## Data Flow
1. **Project Creation**
   - User initiates project creation in `NewProject.cs`.
   - A new `Project` instance is created and loaded into the UI.
   - **Logging**: `Logger.Log` records the project creation event.

2. **Adding Scenes**
   - User adds a scene via the `SceneViewModel`.
   - A new `Scene` instance is created and added to the project.
   - **Logging**: `Logger.Log` records the scene addition event.

3. **Managing Entities (Multi-Selection)**
   - **Renaming**: `MSEntity` allows multi-selection renaming, storing the original and modified names for undo/redo.
   - **Enabling/Disabling**: Multi-selection toggle enables/disables all selected entities simultaneously, with undo/redo support.
   - `GameEntity` and `MSEntity` work together for property synchronization and state management.
   - **Logging**: `Logger.Log` records renaming, enabling, and disabling events.

4. **Undo/Redo Operations**
   - Actions such as renaming or enabling/disabling entities are tracked by `UndoRedo.cs`.
   - `UndoRedoAction` allows reverting to the previous state or reapplying changes.
   - **Logging**: `Logger.Log` records each undo/redo action.

## Interaction Diagram
1. **User** interacts with **MainWindow.xaml**.
2. **MainWindow.xaml** sends commands to **ViewModels** (`MainViewModel`, `ProjectViewModel`, `SceneViewModel`).
3. **ViewModels** update the **Models** (`Project`, `Scene`, `GameEntity`).
4. **Models** manage data and logic.
5. **ViewModels** update the **UI** based on model changes.
6. **Logger** logs relevant events and actions.

## Use Cases
1. **Create Project**
   - User selects "New Project" from the toolbar.
   - `NewProject.cs` initiates project creation.
   - `Project.cs` is loaded into the UI.
   - **Logging**: `Logger.Log` records project creation.

2. **Add Scene**
   - User selects "Add Scene" from the context menu.
   - `Scene.cs` handles scene creation.
   - New scene is added to the project.
   - **Logging**: `Logger.Log` records scene addition.

3. **Add Entity**
   - User adds an entity in a scene.
   - `GameEntity.cs` manages entity creation, adding components.
   - **Logging**: `Logger.Log` records entity creation.

4. **Multi-Selection Rename/Enable-Disable**
   - User selects multiple entities to rename or enable/disable them.
   - `MSEntity` captures the current state for undo and redo.
   - **Undo/Redo**: Allows reversing or reapplying renaming or enabling actions.
   - **Logging**: `Logger.Log` records renaming and enabling/disabling actions.

5. **Undo Action**
   - User performs an action (e.g., renaming).
   - `UndoRedo.cs` captures it for reversal.
   - User clicks "Undo" to reverse the action.
   - `MainViewModel` applies the undo operation.
   - **Logging**: `Logger.Log` records the undo event.
