using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigCrushingProperties : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure whether item can be crushed into something else in pulverizer";

    [JsonProperty(Order = 4)]
    public Dictionary<string, CrushingProperties> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, CrushingProperties> Items { get; set; } = new();

    public ConfigCrushingProperties(ICoreAPI api, ConfigCrushingProperties previousConfig = null)
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
            if (obj == null || obj.Code == null || obj.CrushingProps == null)
            {
                continue;
            }

            // no need for compact code here
            string code = obj.Code.ToString();

            switch (obj)
            {
                case Block when !Blocks.ContainsKey(code):
                    {
                        CrushingProperties props = obj.CrushingProps.Clone();
                        if (props.CrushedStack != null)
                        {
                            props.CrushedStack.ResolvedItemstack = null;
                        }
                        Blocks.Add(code, props);
                        break;
                    }
                case Item when !Items.ContainsKey(code):
                    {
                        CrushingProperties props = obj.CrushingProps.Clone();
                        if (props.CrushedStack != null)
                        {
                            props.CrushedStack.ResolvedItemstack = null;
                        }
                        Items.Add(code, props);
                        break;
                    }
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj, ICoreAPI api)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, CrushingProperties value) in Blocks)
                {
                    if (!obj.WildCardMatchExt(key))
                    {
                        continue;
                    }

                    if (value.CrushedStack != null && !value.CrushedStack.Resolve(api.World, ""))
                    {
                        break;
                    }

                    obj.CrushingProps = value;
                    break;
                }
                break;
            case Item when Items.Any():
                foreach ((string key, CrushingProperties value) in Items)
                {
                    if (!obj.WildCardMatchExt(key))
                    {
                        continue;
                    }

                    if (value.CrushedStack != null && !value.CrushedStack.Resolve(api.World, ""))
                    {
                        break;
                    }

                    obj.CrushingProps = value;
                    break;
                }
                break;
        }
    }
}