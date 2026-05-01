<template>
  <div class="flex flex-col gap-8">
    <PageTitle :schema="title">
      <template
        v-if="inputs.length > 0"
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
          v-for="section in sectionsWithGroups"
          :key="section.key"
          class="w-full col-span-2 grid gap-4"
        >
          <div
            v-if="sectionsWithGroups.length > 1"
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
              grid grid-flow-col
              gap-4 items-start
              max-md:flex max-md:flex-col
            "
            :class="{ 'grid-cols-2': !singleColumn }"
            :style="{ 'grid-template-rows': `repeat(${section.rowCount}, auto)` }"
          >
            <div
              v-for="sGroup in section.groups"
              :key="sGroup.name"
              :class="{ 'col-span-2': !singleColumn && sGroup.wide }"
              class="w-full"
            >
              <div
                class="flex gap-2 max-md:flex-col"
                :class="{ 'narrow': sGroup.inputs.length > 1 }"
              >
                <Inputs
                  :inputs="sGroup.inputs"
                  input-class="w-full"
                  @ready="(value) => onReady(`${section.key}_${sGroup.name}`, value)"
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

const { title, submit, inputs, sections, groups, wide, singleColumn } = schema;

const formData = ref({});
const readyData = ref({});
const ready = computed(() => Object.values(readyData.value).every(v => v));

const group = {};
for(const groupName in groups) {
  for(const inputName of groups[groupName]) {
    group[inputName] = groupName;
  }
}

const inputsByName = {};
for(let i = 0; i < inputs.length; i++) {
  const input = inputs[i];
  inputsByName[input.name] = { input, i };
}

const sectionsWithGroups = [];
for(const section of sections) {
  const sectionWithGroups = {
    key: section.key,
    label: section.label,
    groups: [],
    groupsByName: {}
  };

  const sortedInputNames = [...section.inputs].sort((l, r) => inputsByName[l].i - inputsByName[r].i);
  for(const name of sortedInputNames) {
    const input = inputsByName[name].input;
    const inputGroupName = group[name] || name;
    let inputGroup = sectionWithGroups.groupsByName[inputGroupName];
    if(!inputGroup) {
      inputGroup = {
        name: inputGroupName,
        inputs: [],
        wide: wide.includes(inputGroupName)
      };

      sectionWithGroups.groups.push(inputGroup);
      sectionWithGroups.groupsByName[inputGroupName] = inputGroup;
    }

    inputGroup.inputs.push(input);
  }

  const cellCount = sectionWithGroups.groups.reduce((sum, group) => sum + group.wide + 1, 0);
  sectionWithGroups.rowCount = singleColumn ? cellCount : Math.ceil(cellCount / 2);
  sectionsWithGroups.push(sectionWithGroups);
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
  .narrow * {
    @apply min-w-0;
  }
}
</style>