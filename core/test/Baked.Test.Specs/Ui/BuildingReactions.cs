using Baked.Ui;

namespace Baked.Test.Ui;

public class BuildingReactions : TestSpec
{
    [Test]
    public void Reactions_is_null_by_default()
    {
        var descriptor = new Text().ToDescriptor();

        descriptor.Reactions.ShouldBeNull();
    }

    [Test]
    public void Show_reaction_is_set_to_show_key()
    {
        var descriptor = new Text().ToDescriptor();

        descriptor.ShowOn(GiveMe.AString());

        descriptor.Reactions.ShouldNotBeNull();
        descriptor.Reactions.ContainsKey("show");
    }

    [Test]
    public void Reload_reaction_is_set_to_when_key()
    {
        var descriptor = new Text().ToDescriptor();

        descriptor.ReloadOn(GiveMe.AString());

        descriptor.Reactions.ShouldNotBeNull();
        descriptor.Reactions.ContainsKey("reload");
    }

    [Test]
    public void First_trigger_is_set_to_reactions_directly()
    {
        var descriptor = new Text().ToDescriptor();

        descriptor.ReloadOn(GiveMe.AString());

        descriptor.Reactions.ShouldNotBeNull();
        descriptor.Reactions["reload"].ShouldBeOfType<OnTrigger>();
    }

    [Test]
    public void When_further_triggers_are_added__root_trigger_is_converted_to_composite()
    {
        var descriptor = new Text().ToDescriptor();

        descriptor.ReloadOn(GiveMe.AString());
        descriptor.ReloadWhen(GiveMe.AString());
        descriptor.ReloadWhen(GiveMe.AString());

        descriptor.Reactions.ShouldNotBeNull();
        var composite = descriptor.Reactions["reload"].ShouldBeOfType<CompositeTrigger>();
        composite.Parts.Count.ShouldBe(3);
        composite.Parts[0].ShouldBeOfType<OnTrigger>();
        composite.Parts[1].ShouldBeOfType<WhenTrigger>();
        composite.Parts[2].ShouldBeOfType<WhenTrigger>();
    }
}