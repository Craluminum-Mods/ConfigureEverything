using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigToolMiningSpeed : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public readonly Dictionary<string, IEnumerable<string>> Examples = new()
    {
        [nameof(EnumBlockMaterial)] = Enum.GetValues(typeof(EnumBlockMaterial)).Cast<EnumBlockMaterial>().Select(e => $"{(int)e} = {e}"),
    };

    [JsonProperty(Order = 4)]
    public Dictionary<string, Dictionary<EnumBlockMaterial, float>> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, Dictionary<EnumBlockMaterial, float>> Items { get; set; } = new();

    public ConfigToolMiningSpeed(ICoreAPI api, ConfigToolMiningSpeed previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Blocks.AddRange(previousConfig.Blocks);
            Items.AddRange(previousConfig.Items);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj == null || obj.Code == null || obj.MiningSpeed == null)
            {
                continue;
            }

            string code = obj.Code.CodeWithoutDefaultDomain();

            if (obj is Block && !Blocks.ContainsKey(code))
            {
                Blocks.Add(code, obj.MiningSpeed);
            }

            if (obj is Item && !Items.ContainsKey(code))
            {
                Items.Add(code, obj.MiningSpeed);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, Dictionary<EnumBlockMaterial, float> value) in Blocks)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.MiningSpeed = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, Dictionary<EnumBlockMaterial, float> value) in Items)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.MiningSpeed = value;
                        break;
                    }
                }
                break;
        }
    }
}