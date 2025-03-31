import { expect, test } from "@nuxt/test-utils/playwright";
import primevue from "~/utils/locators/primevue";

test.beforeEach(async({goto}) => {
  await goto("/specs/select-button", { waitUntil: "hydration" });
});

test.describe("Base", () => {
  const id = "Base";

  test("options", async({page}) => {
    const component = page.getByTestId(id);
    const options = component.locator(primevue.selectbutton.option);

    await expect(options.nth(0)).toHaveText("OPTION 1");
    await expect(options.nth(1)).toHaveText("OPTION 2");
  });

  test("select option", async({page}) => {
    const component = page.getByTestId(id);
    const options = component.locator(primevue.selectbutton.option);
    const model = page.getByTestId(`${id}:model`);

    await options.nth(0).click();

    await expect(model).toHaveText("OPTION 1");
  });

  test("prevent empty after select", async({page}) => {
    const component = page.getByTestId(id);
    const options = component.locator(primevue.selectbutton.option);
    const model = page.getByTestId(`${id}:model`);

    await options.nth(0).click(); // select
    await options.nth(0).click(); // try deselect

    await expect(model).toHaveText("OPTION 1");
  });

  test("visual", { tag: "@visual" }, async({page}) => {
    const component = page.getByTestId(id);
    const options = component.locator(primevue.selectbutton.option);

    await options.nth(0).click();

    await expect(component).toHaveScreenshot();
  });
});

test.describe("Option Label and Value", () => {
  const id = "Option Label and Value";

  test("options", async({page}) => {
    const component = page.getByTestId(id);
    const options = component.locator(primevue.selectbutton.option);

    await expect(options.nth(0)).toHaveText("LABEL 1");
    await expect(options.nth(1)).toHaveText("LABEL 2");
  });

  test("select option", async({page}) => {
    const component = page.getByTestId(id);
    const options = component.locator(primevue.selectbutton.option);
    const model = page.getByTestId(`${id}:model`);

    await options.nth(0).click();

    await expect(model).toHaveText("VALUE_1");
  });
});

test.describe("Allow Empty", () => {
  const id = "Allow Empty";

  test("clears selection", async({page}) => {
    const component = page.getByTestId(id);
    const option = component.locator(primevue.selectbutton.option);
    const model = page.getByTestId(`${id}:model`);

    await option.click(); // select
    await option.click(); // deselect

    await expect(model).not.toHaveText("OPTION");
  });
});
