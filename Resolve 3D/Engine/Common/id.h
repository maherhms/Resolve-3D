#pragma once

#include "CommonHeaders.h"

namespace resolve::id {

// Type alias for IDs, using a 32-bit unsigned integer.
using id_type = u32;

namespace internal {
// Constants defining bit allocation for ID structure.
constexpr u32 generation_bits{10};
constexpr u32 index_bits{sizeof(id_type) * 8 - generation_bits};
// Masks to isolate index and generation bits.
constexpr id_type index_mask{(id_type{1} << index_bits) - 1};
constexpr id_type generation_mask{(id_type{1} << generation_bits) - 1};
}  // namespace internal
// Special values and minimum deleted elements threshold.
constexpr id_type invalid_id{id_type(-1)};
constexpr u32 min_deleted_elements{1024};

// Selects appropriate type for storing generation bits.
using generation_type = std::conditional_t<
    internal::generation_bits <= 16,
    std::conditional_t<internal::generation_bits <= 8, u8, u16>, u32>;
static_assert(sizeof(generation_type) * 8 >= internal::generation_bits);
static_assert((sizeof(id_type) - sizeof(generation_type)) > 0);

// ID validity check.
constexpr bool is_valid(id_type id) { return id != invalid_id; }

// Extracts the index part of an ID.
constexpr id_type index(id_type id) {
  id_type index{id & internal::index_mask};
  assert(index != internal::index_mask);
  return index;
}

// Extracts the generation part of an ID.
constexpr id_type generation(id_type id) {
  return (id >> internal::index_bits) & internal::generation_mask;
}

constexpr id_type new_generation(id_type id) {
  const id_type generation{id::generation(id) + 1};
  assert(generation < (((u64)1 << internal::generation_bits) - 1));
  return index(id) | (generation << internal::index_bits);
}

#if _DEBUG
namespace internal {
// Base struct for ID types in debug mode.
struct id_base {
  constexpr explicit id_base(id_type id) : _id{id} {}
  constexpr operator id_type() const { return _id; }

 private:
  id_type _id;
};
}  // namespace internal

// Defines a typed ID struct for debugging purposes.
#define DEFINE_TYPE_ID(name)                                 \
  struct name final : id::internal::id_base {                \
    constexpr explicit name(id::id_type id) : id_base{id} {} \
    constexpr name() : id_base{0} {}                         \
  };
#else
// Simplified ID alias for release mode.
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif
}  // namespace resolve::id
