using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration.ConfigMapColors;

public class ConfigMapColors : IModConfig
{
    public bool Enabled { get; set; }

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
