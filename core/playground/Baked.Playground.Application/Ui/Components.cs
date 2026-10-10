using Baked.Ui;

namespace Baked.Playground.Ui;

public static class Components
{
    public static ComponentDescriptor<Container> Container() =>
        new(new());

    public static ComponentDescriptor<Expected> Expected() =>
        new(new());

    public static ComponentDescriptor<ExpectedInput> ExpectedInput() =>
        new(new());

    public static ComponentDescriptor<ExpectedInputGroup> ExpectedInputGroup() =>
        new(new());

    public static ComponentDescriptor<LoginPage> LoginPage() =>
        new(new());

    public static ComponentDescriptor<MyText> MyText() =>
        new(new());

    public static ComponentDescriptor<RoutedPage> RoutedPage() =>
        new(new());
}