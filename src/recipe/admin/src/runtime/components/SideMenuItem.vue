<template>
  <RouterLink :to="item.route">
    <Button
      v-tooltip="{ value: l(item.title), showDelay: 300 }"
      :text="!selected"
      :icon="item.icon"
      :severity="selected ? 'primary':'secondary'"
      size="large"
      class="!px-6 !py-2"
      :disabled="item.disabled"
    />
  </RouterLink>
</template>
<script setup>
import { computed } from "vue";
import { RouterLink } from "vue-router";
import { Button } from "primevue";
import { useLocalization } from "#imports";

const { localize: l } = useLocalization();

const { item, path } = defineProps({
  item: { type: Object, required: true },
  path: { type: String, required: true }
});

const selected = computed(() => item.route === path || (item.route !== "/" && path.startsWith(item.route)));
</script>
