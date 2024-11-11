using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigItemDimensions : IModConfigWithAutoFill
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool AutoFill { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Some containers (e.g. crucible) can fit only items with certain dimensions. Default dimensions are { 0.5, 0.5, 0.5 }";

    [JsonProperty(Order = 4)]
    public Dictionary<string, Size3f> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, Size3f> Items { get; set; } = new();

    public ConfigItemDimensions(ICoreAPI api, ConfigItemDimensions previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            AutoFill = previousConfig.AutoFill;

            Blocks.AddRange(previousConfig.Blocks);
            Items.AddRange(previousConfig.Items);
        }

        if (api != null && AutoFill)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj == null
                || obj.Code == null
                || obj.Dimensions == null
                || (obj.Dimensions is { Width: 0.5f, Height: 0.5f, Length: 0.5f }))
            {
                continue;
            }

            string code = obj.Code.GetCompactCode().ToString();

            switch (obj)
            {
                case Block when !Blocks.ContainsKey(code):
                    Blocks.Add(code, obj.Dimensions);
                    break;
                case Item when !Items.ContainsKey(code):
                    Items.Add(code, obj.Dimensions);
                    break;
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, Size3f value) in Blocks)
                {
                    if (obj.WildCardMatchExt(key))
                    {
                        obj.Dimensions = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, Size3f value) in Items)
                {
                    if (obj.WildCardMatchExt(key))
                    {
                        obj.Dimensions = value;
                        break;
                    }
                }
                break;
        }
    }
}