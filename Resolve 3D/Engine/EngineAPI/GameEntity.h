#pragma once

#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"

namespace resolve::game_entity {

DEFINE_TYPE_ID(entity_id);

class entity {
 public:
  constexpr explicit entity(entity_id id) : _id{id} {}
  constexpr entity() : _id{id::invalid_id} {}
  constexpr entity_id get_id() const { return _id; }
  constexpr bool is_valid() const { return id::is_valid(_id); }

  transform::component transform() const;

 private:
  entity_id _id;
};
}  // namespace resolve::game_entity
