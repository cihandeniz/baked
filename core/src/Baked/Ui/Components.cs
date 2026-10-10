namespace Baked.Ui;

public static class Components
{
    public static ComponentDescriptor<Button> Button() =>
        new(new());

    public static ComponentDescriptor<CardLink> CardLink() =>
        new(new());

    public static ComponentDescriptor<Check> Check() =>
        new(new());

    public static ComponentDescriptor<Composite> Composite() =>
        new(new());

    public static Content Content() =>
        new();

    public static ComponentDescriptor<DataPanel> DataPanel() =>
        new(new());

    public static ComponentDescriptor<DataContainer> DataContainer() =>
        new(new());

    public static ComponentDescriptor<DataTable> DataTable() =>
        new(new());

    public static DataTable.Column DataTableColumn() =>
        new();

    public static DataTable.Export DataTableExport() =>
        new();

    public static DataTable.Footer DataTableFooter() =>
        new();

    public static DataTable.VirtualScroller DataTableVirtualScroller() =>
        new();

    public static ComponentDescriptor<Date> Date() =>
        new(new());

    public static ComponentDescriptor<DefaultLayout> DefaultLayout() =>
        new(new());

    public static DefaultLayout.ScrollTop DefaultLayoutScrollTop() =>
        new();

    public static ComponentDescriptor<Dialog> Dialog() =>
        new(new());

    public static ComponentDescriptor<ErrorPage> ErrorPage() =>
        new(new());

    public static Field Field() =>
        new();

    public static ComponentDescriptor<Fieldset> Fieldset() =>
        new(new());

    public static ComponentDescriptor<Filter> Filter() =>
        new(new());

    public static ComponentDescriptor<FormPage> FormPage() =>
        new(new());

    public static FormPage.InputGroup FormPageInputGroup() =>
        new();

    public static ComponentDescriptor<Header> Header() =>
        new(new());

    public static Header.Item HeaderItem() =>
        new();

    public static ComponentDescriptor<Icon> Icon() =>
        new(new());

    public static Input Input() =>
        new();

    public static ComponentDescriptor<InputCheckbox> InputCheckbox() =>
        new(new());

    public static ComponentDescriptor<InputDate> InputDate() =>
        new(new());

    public static ComponentDescriptor<InputMailAddress> InputMailAddress() =>
        new(new());

    public static ComponentDescriptor<InputMoney> InputMoney() =>
        new(new());

    public static ComponentDescriptor<InputNumber> InputNumber() =>
        new(new());

    public static ComponentDescriptor<InputRate> InputRate() =>
        new(new());

    public static ComponentDescriptor<InputText> InputText() =>
        new(new());

    public static ComponentDescriptor<InputUrl> InputUrl() =>
        new(new());

    public static ComponentDescriptor<LanguageSwitcher> LanguageSwitcher() =>
        new(new());

    public static ComponentDescriptor<MenuPage> MenuPage() =>
        new(new());

    public static MenuPage.Section MenuPageSection() =>
        new();

    public static ComponentDescriptor<Message> Message() =>
        new(new());

    public static ComponentDescriptor<ModalLayout> ModalLayout() =>
        new(new());

    public static ComponentDescriptor<MissingComponent> MissingComponent() =>
        new(new());

    public static MissingComponent.DomainSource MissingComponentDomainSource() =>
        new();

    public static ComponentDescriptor<Money> Money() =>
        new(new());

    public static ComponentDescriptor<MultiSelect> MultiSelect() =>
        new(new());

    public static ComponentDescriptor<MultiSelectButton> MultiSelectButton() =>
        new(new());

    public static ComponentDescriptor<NavLink> NavLink() =>
        new(new());

    public static ComponentDescriptor<Number> Number() =>
        new(new());

    public static ComponentDescriptor<PageSize> PageSize() =>
        new(new());

    public static ComponentDescriptor<PageTitle> PageTitle() =>
        new(new());

    public static ComponentDescriptor<Paginator> Paginator() =>
        new(new());

    public static ComponentDescriptor<Rate> Rate() =>
        new(new());

    public static ComponentDescriptor<Select> Select() =>
        new(new());

    public static ComponentDescriptor<SelectButton> SelectButton() =>
        new(new());

    public static ComponentDescriptor<SideMenu> SideMenu() =>
        new(new());

    public static SideMenu.Item SideMenuItem() =>
        new();

    public static ComponentDescriptor<SimpleForm> SimpleForm() =>
        new(new());

    public static SimpleForm.Dialog SimpleFormDialog() =>
        new();

    public static ComponentDescriptor<SimplePage> SimplePage() =>
        new(new());

    public static Tab Tab() =>
        new();

    public static ComponentDescriptor<TabbedPage> TabbedPage() =>
        new(new());

    public static ComponentDescriptor<Text> Text() =>
        new(new());

    public static ComponentDescriptor<Textarea> Textarea() =>
        new(new());

    public static ComponentDescriptor<TextLink> TextLink() =>
        new(new());
}