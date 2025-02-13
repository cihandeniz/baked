import { defineContentConfig, defineCollection, z } from "@nuxt/content";

export default defineContentConfig({
  collections: {
    content: defineCollection({
      type: "page",
      source: "**/*.md"
    }),
    notFound: defineCollection({
      type: "page",
      source: "**/not-found.md"
    }),
    sections: defineCollection({
      type: "data",
      source: "**/*.md",
      schema: z.object({
        path: z.string(),
        title: z.string(),
        pages: z.array(z.string()),
        position: z.number(),
        sort: z.object(
          {
            by: z.string(),
            order: z.string(),
            version: z.boolean()
          }
        )
      })
    }),
    menus: defineCollection({
      type: "data",
      source: {
        include: "**/**/index.md"
      },
      schema: z.object({
        path: z.string(),
        title: z.string(),
        position: z.number(),
        sections: z.array(z.string())
      })
    }),
    menuOrder: defineCollection({
      type: "data",
      source: {
        include: "index.md"
      },
      schema: z.object({
        sections: z.array(z.string())
      })
    })
  }
});
