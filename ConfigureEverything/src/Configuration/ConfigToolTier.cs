using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigToolTier : IModConfigWithAutoFill
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool AutoFill { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure what tier a tool can mine";

    [JsonProperty(Order = 4)]
    public Dictionary<string, int> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, int> Items { get; set; } = new();

    public ConfigToolTier(ICoreAPI api, ConfigToolTier previousConfig = null)
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
            if (obj == null || obj.Code == null || obj.ToolTier == 0)
            {
                continue;
            }

            // no need for compact code here
            string code = obj.Code.ToString();

            switch (obj)
            {
                case Block when !Blocks.ContainsKey(code):
                    Blocks.Add(code, obj.ToolTier);
                    break;
                case Item when !Items.ContainsKey(code):
                    Items.Add(code, obj.ToolTier);
                    break;
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, int value) in Blocks)
                {
                    if (obj.WildCardMatchExt(key))
                    {
                        obj.ToolTier = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, int value) in Items)
                {
                    if (obj.WildCardMatchExt(key))
                    {
                        obj.ToolTier = value;
                        break;
                    }
                }
                break;
        }
    }
}