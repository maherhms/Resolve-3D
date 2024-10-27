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

entity create_game_entity(const entity_info& info);
void remove_game_entity(entity e);
bool is_alive(entity e);
}  // namespace game_entity
}  // namespace resolve