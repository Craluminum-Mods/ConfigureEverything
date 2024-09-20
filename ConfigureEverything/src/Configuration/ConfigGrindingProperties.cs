using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigGrindingProperties : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure whether item can be ground into something else in quern";

    [JsonProperty(Order = 4)]
    public Dictionary<string, GrindingProperties> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, GrindingProperties> Items { get; set; } = new();

    public ConfigGrindingProperties(ICoreAPI api, ConfigGrindingProperties previousConfig = null)
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
            if (obj == null || obj.Code == null || obj.GrindingProps == null)
            {
                continue;
            }

            string code = obj.Code.CodeWithoutDefaultDomain();

            if (obj is Block && !Blocks.ContainsKey(code))
            {
                GrindingProperties props = obj.GrindingProps.Clone();
                if (props.GroundStack != null)
                {
                    props.GroundStack.ResolvedItemstack = null;
                }
                Blocks.Add(code, props);
            }

            if (obj is Item && !Items.ContainsKey(code))
            {
                GrindingProperties props = obj.GrindingProps.Clone();
                if (props.GroundStack != null)
                {
                    props.GroundStack.ResolvedItemstack = null;
                }
                Items.Add(code, props);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj, ICoreAPI api)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, GrindingProperties value) in Blocks)
                {
                    if (!obj.WildCardMatch(key))
                    {
                        continue;
                    }

                    if (value.GroundStack != null && !value.GroundStack.Resolve(api.World, ""))
                    {
                        break;
                    }

                    obj.GrindingProps = value;
                    break;
                }
                break;
            case Item when Items.Any():
                foreach ((string key, GrindingProperties value) in Items)
                {
                    if (!obj.WildCardMatch(key))
                    {
                        continue;
                    }

                    if (value.GroundStack != null && !value.GroundStack.Resolve(api.World, ""))
                    {
                        break;
                    }

                    obj.GrindingProps = value;
                    break;
                }
                break;
        }
    }
}