#include "Entity.h"

#include "Transform.h"

namespace resolve::game_entity {
namespace {
utl::vector<transform::component> transforms;
utl::vector<id::generation_type> generations;
utl::deque<entity_id> free_ids;
}  // namespace

// Creates a new game entity with a unique ID.
// Uses existing IDs from free_ids when possible, otherwise generates a new one.
// Ensures entities always have a required transform component.
entity create_game_entity(const entity_info& info) {
  assert(info.transform);  // all entities must have transform component
  if (!info.transform) return entity{};

  entity_id id;

  if (free_ids.size() > id::min_deleted_elements) {
    id = free_ids.front();
    assert(!is_alive(entity{id}));
    free_ids.pop_front();
    id = entity_id{id::new_generation(id)};
    ++generations[id::index(id)];
  } else {
    id = entity_id{(id::id_type)generations.size()};
    generations.push_back(0);

    // resize components
    // NOTE: we dont call resize(), so the number of memory allocations stays
    // low
    transforms.emplace_back();
  }

  const entity new_entity{id};
  const id::id_type index{id::index(id)};

  // create transform component
  assert(!transforms[index].is_valid());
  transforms[index] = transform::create_transform(*info.transform, new_entity);
  if (!transforms[index].is_valid()) return {};

  return new_entity;
};
// Marks a game entity as deleted by adding its ID to free_ids for recycling.
// Asserts that the entity is alive before removal.
void remove_game_entity(entity e) {
  const entity_id id{e.get_id()};
  const id::id_type index{id::index(id)};
  assert(is_alive(e));
  if (is_alive(e)) {
    transform::remove_transform(transforms[index]);
    transforms[index] = {};
    free_ids.push_back(id);
  }
}
// Checks if a game entity is still valid.
// Compares the entity's generation with the stored generation to confirm
// validity.
bool is_alive(entity e) {
  assert(e.is_valid());
  const entity_id id{e.get_id()};
  const id::id_type index{id::index(id)};
  assert(index < generations.size());
  assert(generations[index] == id::generation(id));
  return (generations[index] == id::generation(id) &&
          transforms[index].is_valid());
}

transform::component entity::transform() const {
  assert(is_alive(*this));
  const id::id_type index{id::index(_id)};
  return transforms[index];
}

}  // namespace resolve::game_entity