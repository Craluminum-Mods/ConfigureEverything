using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigMapColors : IModConfig
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public string Description => "Adjut map colors on world map and minimap";

    [JsonProperty(Order = 3)]
    public OrderedDictionary<string, string> HexColorsByCode { get; set; } = new();

    public ConfigMapColors(ICoreAPI api, ConfigMapColors previousConfig = null)
    {
        // in case some values are missing
        HexColorsByCode.AddRange(ChunkMapLayer.hexColorsByCode);

        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            HexColorsByCode.AddRange(previousConfig.HexColorsByCode);
        }
    }

    public void ApplyPatches() => ChunkMapLayer.hexColorsByCode = HexColorsByCode;
}
