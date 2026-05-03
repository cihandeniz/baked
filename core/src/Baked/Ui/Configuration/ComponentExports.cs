namespace Baked.Ui.Configuration;

public class ComponentExports : List<string>
{
    public void AddFromExtensions(Type type)
    {
        var extensions = type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public) ?? [];
        var componentTypes = extensions
            .Where(m =>
                m.ReturnType.IsAssignableTo(typeof(IComponentDescriptor)) &&
                !m.GetGenericArguments().Any()
            )
            .Select(m => m.Name);

        AddRange(componentTypes);
    }
}