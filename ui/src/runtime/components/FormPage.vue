<template>
  <div class="flex flex-col gap-8">
    <PageTitle :schema="title">
      <template
        v-if="sections.length > 0"
        #actions
      >
        <Button
          :schema="submit"
          :ready
          @submit="onSubmit"
        />
      </template>
    </PageTitle>
    <div class="flex justify-center">
      <Contents class="gap-6">
        <div
          v-for="section in sections"
          :key="section.key"
          class="w-full col-span-2 grid gap-4"
        >
          <div
            v-if="sections.length > 1"
            class="
              pb-2 border-b-2
              border-zinc-100 dark:border-zinc-900
            "
          >
            <span
              class="
                text-md font-semibold
                text-zinc-800 dark:text-zinc-400
              "
            >
              {{ l(section.label) }}
            </span>
          </div>
          <div
            class="
              grid grid-cols-2 grid-flow-col
              gap-4 items-start
              max-md:flex max-md:flex-col
            "
            :style="{ 'grid-template-rows': `repeat(${calculateRowCount(section)}, auto)` }"
          >
            <div
              v-for="inputGroup in section.inputGroups"
              :key="inputGroup.key"
              :class="{ 'col-span-2': inputGroup.wide }"
              class="w-full"
            >
              <div
                class="flex gap-2 max-md:flex-col"
                :class="{ 'reset-min-w': inputGroup.inputs.length > 1 }"
              >
                <Inputs
                  :inputs="inputGroup.inputs"
                  input-class="w-full"
                  @ready="(value) => onReady(`${section.key}_${inputGroup.key}`, value)"
                  @changed="onChanged"
                />
              </div>
            </div>
          </div>
        </div>
      </Contents>
    </div>
  </div>
</template>
<script setup>
import { computed, ref } from "vue";
import { useLocalization } from "#imports";
import { Button, Contents, Inputs, PageTitle } from "#components";

const { localize: l } = useLocalization();

const { schema } = defineProps({
  schema: { type: null, required: true }
});
const emit = defineEmits(["submit"]);

const { title, submit, sections } = schema;

const formData = ref({});
const readyData = ref({});
const ready = computed(() => Object.values(readyData.value).every(v => v));

function calculateRowCount(section) {
  const cellCount = section.inputGroups.reduce((sum, group) => sum + !!group.wide + 1, 0);

  return Math.ceil(cellCount / 2);
}

function onReady(key, value) {
  Object.assign(readyData.value, { [key]: value });
}

function onChanged({ values }) {
  Object.assign(formData.value, values);
}

function onSubmit() {
  if(!ready.value) { return; }

  emit("submit", formData.value);
}
</script>
<style>
.b-component--FormPage {
  .reset-min-w * {
    @apply min-w-0;
  }
}
</style>