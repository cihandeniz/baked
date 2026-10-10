namespace Baked.Ui;

public static class Components
{
    public static ComponentDescriptor<Button> Button(
        Action<Button>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<CardLink> CardLink(
        Action<CardLink>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Check> Check() =>
        new(new());

    public static ComponentDescriptor<Composite> Composite(
        Action<Composite>? options = default
    ) => new(options.Apply(new()));

    public static Content Content(
        Action<Content>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<DataPanel> DataPanel(
        Action<DataPanel>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<DataContainer> DataContainer(
        Action<DataContainer>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<DataTable> DataTable(
        Action<DataTable>? options = default
    ) => new(options.Apply(new()));

    public static DataTable.Column DataTableColumn(
        Action<DataTable.Column>? options = default
    ) => options.Apply(new());

    public static DataTable.Export DataTableExport(
        Action<DataTable.Export>? options = default
    ) => options.Apply(new());

    public static DataTable.Footer DataTableFooter(
        Action<DataTable.Footer>? options = default
    ) => options.Apply(new());

    public static DataTable.VirtualScroller DataTableVirtualScroller(
        Action<DataTable.VirtualScroller>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<Date> Date(
        Action<Date>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<DefaultLayout> DefaultLayout(string name,
        Action<DefaultLayout>? options = default
    ) => new(options.Apply(new(name)));

    public static DefaultLayout.ScrollTop DefaultLayoutScrollTop(
        Action<DefaultLayout.ScrollTop>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<Dialog> Dialog(
        Action<Dialog>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<ErrorPage> ErrorPage(
        Action<ErrorPage>? options = default,
        IData? data = default
    ) => new(options.Apply(new())) { Data = data };

    public static Field Field(
        Action<Field>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<Fieldset> Fieldset(
        Action<Fieldset>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Filter> Filter(
        Action<Filter>? options = default,
        IAction? action = default
    ) => new(options.Apply(new())) { Action = action };

    public static ComponentDescriptor<FormPage> FormPage(string path,
        Action<FormPage>? options = default
    ) => new(options.Apply(new(path)));

    public static FormPage.InputGroup FormPageInputGroup(
        Action<FormPage.InputGroup>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<Header> Header(
        Action<Header>? options = default
    ) => new(options.Apply(new()));

    public static Header.Item HeaderItem(string route,
        Action<Header.Item>? options = default
    ) => options.Apply(new(route));

    public static ComponentDescriptor<Icon> Icon(
        Action<Icon>? options = default
    ) => new(options.Apply(new()));

    public static Input Input(
        Action<Input>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<InputCheckbox> InputCheckbox(
        Action<InputCheckbox>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputDate> InputDate(
        Action<InputDate>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputMailAddress> InputMailAddress(
        Action<InputMailAddress>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputMoney> InputMoney(
        Action<InputMoney>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputNumber> InputNumber(
        Action<InputNumber>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputRate> InputRate(
        Action<InputRate>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputText> InputText(
        Action<InputText>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<InputUrl> InputUrl(
        Action<InputUrl>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<LanguageSwitcher> LanguageSwitcher(
        Action<LanguageSwitcher>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<MenuPage> MenuPage(string path,
        Action<MenuPage>? options = default
    ) => new(options.Apply(new(path)));

    public static MenuPage.Section MenuPageSection(
        Action<MenuPage.Section>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<Message> Message(
        Action<Message>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<ModalLayout> ModalLayout(string name,
        Action<ModalLayout>? options = default
    ) => new(options.Apply(new(name)));

    public static ComponentDescriptor<MissingComponent> MissingComponent(
        Action<MissingComponent>? options = default
    ) => new(options.Apply(new()));

    public static MissingComponent.DomainSource MissingComponentDomainSource(string type,
        Action<MissingComponent.DomainSource>? options = default
    ) => options.Apply(new(type));

    public static ComponentDescriptor<Money> Money(
        Action<Money>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<MultiSelect> MultiSelect(
        Action<MultiSelect>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<MultiSelectButton> MultiSelectButton(
        Action<MultiSelectButton>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<NavLink> NavLink(
        Action<NavLink>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Number> Number(
        Action<Number>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<PageSize> PageSize(
        Action<PageSize>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<PageTitle> PageTitle(
        Action<PageTitle>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Paginator> Paginator(
        Action<Paginator>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Rate> Rate(
        Action<Rate>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Select> Select(
        Action<Select>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<SelectButton> SelectButton(
        Action<SelectButton>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<SideMenu> SideMenu(
        Action<SideMenu>? options = default
    ) => new(options.Apply(new()));

    public static SideMenu.Item SideMenuItem(string route, string icon,
        Action<SideMenu.Item>? options = default
    ) => options.Apply(new(route, icon));

    public static ComponentDescriptor<SimpleForm> SimpleForm(
        Action<SimpleForm>? options = default
    ) => new(options.Apply(new()));

    public static SimpleForm.Dialog SimpleFormDialog(
        Action<SimpleForm.Dialog>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<SimplePage> SimplePage(string path,
        Action<SimplePage>? options = default
    ) => new(options.Apply(new(path)));

    public static Tab Tab(
        Action<Tab>? options = default
    ) => options.Apply(new());

    public static ComponentDescriptor<TabbedPage> TabbedPage(string path,
        Action<TabbedPage>? options = default
    ) => new(options.Apply(new(path)));

    public static ComponentDescriptor<Text> Text(
        Action<Text>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<Textarea> Textarea(
        Action<Textarea>? options = default
    ) => new(options.Apply(new()));

    public static ComponentDescriptor<TextLink> TextLink(
        Action<TextLink>? options = default
    ) => new(options.Apply(new()));
}