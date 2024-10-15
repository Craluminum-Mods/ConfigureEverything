using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigBlockFertility : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure what can grow on a block. 0 = nothing can grow, 10 = some tallgrass and small trees can be grow on it, 100 = all grass and trees can grow on it";

    [JsonProperty(Order = 4)]
    public Dictionary<string, int> Blocks { get; set; } = new();

    public ConfigBlockFertility(ICoreAPI api, ConfigBlockFertility previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Blocks.AddRange(previousConfig.Blocks);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks)
        {
            if (block is BlockMultiblock || block.IsLiquid() || block.Fertility == 0)
            {
                continue;
            }

            string code = block.Code.GetCompactCode().ToString();
            if (!Blocks.ContainsKey(code))
            {
                Blocks.Add(code, block.Fertility);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        if (obj is not Block block || !Blocks.Any())
        {
            return;
        }

        foreach ((string key, int value) in Blocks)
        {
            if (obj.WildCardMatchExt(key))
            {
                block.Fertility = value;
                break;
            }
        }
    }
}