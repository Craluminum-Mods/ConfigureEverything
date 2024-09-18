using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration;

public interface IModConfigWithDefaultValues : IModConfig
{
    bool FillWithDefaultValues { get; set; }

    void FillDefault(ICoreAPI api);
}