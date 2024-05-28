namespace SamuelIH.Nwn.Blueprints;

public interface IOverridableConstruct
{
    void ResolveFromParent(object? parent);
    bool IsResolved { get; }
}