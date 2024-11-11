using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration;

public interface IModConfigWithAutoFill : IModConfig
{
    bool AutoFill { get; set; }

    void FillDefault(ICoreAPI api);
}