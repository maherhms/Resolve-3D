# Resolve-3D Engine Architecture Documentation

## Overview
The **Resolve-3D Engine** is a game engine in C++ focused on efficient entity management and modular component handling, including ID management, entity lifecycle, and transform functionality.

### Key Modules

#### 1. **ID Management (`id` namespace)**
   - Provides unique identification for entities using a bitwise structure.
   - **ID Components**:
     - **`id_type`**: Alias for `u32`, used to represent entity IDs.
     - **`generation_bits`**: Defines bit count for generation tracking, allowing each ID to have versioning.
     - **`index_bits`**: Specifies bits allocated for entity indexing.
   - **Masks and Generations**:
     - Masks, `generation_mask` and `index_mask`, extract specific sections (index and generation) from IDs, ensuring entity versioning is efficient.
   - **Utilities**:
     - `is_valid()`: Checks if an ID is valid.
     - `index()` and `generation()`: Extract the index and generation parts.
     - `new_generation()`: Generates the next version of an entity’s ID, helping with recycling IDs.

#### 2. **Entity Management (`game_entity` namespace)**
   - Manages the lifecycle of entities, which represent objects within the game.
   - **Entity Creation (`create_game_entity`)**:
     - Takes `entity_info` (metadata for initializing an entity).
     - Uses a pool of IDs (from `id::free_ids`) to reuse entity IDs after deletion.
   - **Entity Removal (`remove_game_entity`)**:
     - Flags an entity’s ID as available for future reuse, ensuring efficiency in memory.
   - **Entity Validation (`is_alive`)**:
     - Checks if an entity ID points to an active entity by verifying that the generation of the ID matches the stored generation.

#### 3. **Transform Component (WIP)**
   - While not fully implemented, this component will store position, rotation, and scale data for each entity.
   - It will serve as a foundational component that all game entities must have.

## Code Structure

### Header Files
   - **`CommonHeaders.h`**: Contains definitions and macros common to all modules.
   - **`Entity.h`**: Primary header for entity creation, removal, and validation.

### Compilation Macros
   - **STL Containers**: Uses STL containers (`std::vector` and `std::deque`) for managing entity data efficiently. Future versions may replace these with custom containers for performance gains.

## Planned Features

1. **Custom Memory Allocation**: Replace STL containers with custom implementations for memory efficiency.
2. **Enhanced Components System**: Allow additional components like physics and rendering to attach to entities.
3. **Transform Integration**: Finalize the transform component to ensure spatial management.

---

This architecture forms the base of the **Resolve-3D Engine** and will expand with additional features like scene management, custom components, and an event-driven system.

