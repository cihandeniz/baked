<template>
  <UiSpec
    title="Side Menu"
    :variants="variants"
    :vertical="true"
  />
</template>
<script setup>
import giveMe from "~/utils/giveMe";

const variants = [
  {
    name: "Base",
    descriptor: giveMe.aSideMenu({
      menu: [
        giveMe.aSideMenuItem({ route: "/menu", icon: "pi pi-heart", title: "Spec.Title" })
      ],
      footer: giveMe.anExpected({ testId: "footer", value: "FT" })
    })
  },
  {
    name: "Highlight",
    descriptor: giveMe.aSideMenu({
      menu: [
        giveMe.aSideMenuItem({ route: "/selected", icon: "pi pi-heart" }),
        giveMe.aSideMenuItem({ route: "/not-selected", icon: "pi pi-wave-pulse" })
      ],
      data: {
        path: "/selected"
      }
    })
  },
  {
    name: "Custom Logo",
    descriptor: giveMe.aSideMenu({
      logo: "e5c4p3.png"
    })
  },
  {
    name: "Disabled Item",
    descriptor: giveMe.aSideMenu({
      menu: [
        giveMe.aSideMenuItem({ disabled: true })
      ]
    })
  }
];
</script>
