using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigStackSizes : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure max amount of item that one default inventory slot can hold";

    [JsonProperty(Order = 4)]
    public float Multiplier { get; set; } = 1.0f;

    [JsonProperty(Order = 5)]
    public Dictionary<string, int> Blocks { get; set; } = new();

    [JsonProperty(Order = 6)]
    public Dictionary<string, int> Items { get; set; } = new();

    public ConfigStackSizes(ICoreAPI api, ConfigStackSizes previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Multiplier = previousConfig.Multiplier;
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
            if (obj == null || obj.Code == null || obj is BlockMultiblock)
            {
                continue;
            }

            string code = obj.Code.GetCompactCode().ToString();

            switch (obj)
            {
                case Block when !Blocks.ContainsKey(code):
                    Blocks.Add(code, obj.MaxStackSize);
                    break;
                case Item when !Items.ContainsKey(code):
                    Items.Add(code, obj.MaxStackSize);
                    break;
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        if (Multiplier is not 0 and not 1)
        {
            if (obj.MaxStackSize * obj.MaxStackSize != obj.MaxStackSize)
            {
                obj.MaxStackSize = (int)(obj.MaxStackSize * Multiplier);
            }
            return;
        }

        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, int value) in Blocks)
                {
                    if (obj.WildCardMatchExt(key))
                    {
                        obj.MaxStackSize = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, int value) in Items)
                {
                    if (obj.WildCardMatchExt(key))
                    {
                        obj.MaxStackSize = value;
                        break;
                    }
                }
                break;
        }
    }
}
