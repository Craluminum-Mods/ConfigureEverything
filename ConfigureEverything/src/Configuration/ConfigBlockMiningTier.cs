using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigBlockMiningTier : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure tool tier required to break a block";

    [JsonProperty(Order = 4)]
    public Dictionary<string, int> Blocks { get; set; } = new();

    public ConfigBlockMiningTier(ICoreAPI api, ConfigBlockMiningTier previousConfig = null)
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
            if (block is BlockMultiblock || block.IsLiquid() || block.RequiredMiningTier == 0)
            {
                continue;
            }

            string code = block.Code.GetCompactCode().ToString();
            if (!Blocks.ContainsKey(code))
            {
                Blocks.Add(code, block.RequiredMiningTier);
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
                block.RequiredMiningTier = value;
                break;
            }
        }
    }
}