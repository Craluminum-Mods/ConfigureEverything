using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigBlockFertility : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }

    public bool FillWithDefaultValues { get; set; }

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

            string code = block.Code.GetCompactBlockCode();
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
            if (obj.WildCardMatch(key))
            {
                block.Fertility = value;
                break;
            }
        }
    }
}