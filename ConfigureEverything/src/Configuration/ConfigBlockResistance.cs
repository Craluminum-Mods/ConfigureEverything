using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigBlockResistance : IModConfigWithAutoFill
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool AutoFill { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure how long it takes to break a block in seconds";

    [JsonProperty(Order = 4)]
    public Dictionary<string, float> Blocks { get; set; } = new();

    public ConfigBlockResistance(ICoreAPI api, ConfigBlockResistance previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            AutoFill = previousConfig.AutoFill;

            Blocks.AddRange(previousConfig.Blocks);
        }

        if (api != null && AutoFill)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks)
        {
            if (block is BlockMultiblock || block.IsLiquid() || block.Resistance == 0)
            {
                continue;
            }

            string code = block.Code.GetCompactCode().ToString();
            if (!Blocks.ContainsKey(code))
            {
                Blocks.Add(code, block.Resistance);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        if (obj is not Block block || !Blocks.Any())
        {
            return;
        }

        foreach ((string key, float value) in Blocks)
        {
            if (obj.WildCardMatchExt(key))
            {
                block.Resistance = value;
                break;
            }
        }
    }
}