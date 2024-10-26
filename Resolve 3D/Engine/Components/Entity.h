#pragma once

#include "ComponentsCommon.h"

namespace resolve {
namespace transform {
struct init_info;
}
namespace game_entity {

#define INIT_INFO(component) \
  namespace {                \
  struct init_info;          \
  }

INIT_INFO(transform);

#undef INIT_INFO

struct entity_info {
  transform::init_info* transform{nullptr};
};

entity_id create_game_entity(const entity_info& info);
void remove_game_entity(entity_id id);
bool is_alive(entity_id id);
}  // namespace game_entity
}  // namespace resolve