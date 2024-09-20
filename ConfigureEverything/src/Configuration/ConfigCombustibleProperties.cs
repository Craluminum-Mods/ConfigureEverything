using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigCombustibleProperties : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure burning temperature and time, output stack (e.g. nugget => ingot) etc.";

    [JsonProperty(Order = 4)]
    public Dictionary<string, IEnumerable<string>> Examples => new()
    {
        [nameof(CombustibleProperties.SmeltingType)] = Enum.GetValues(typeof(EnumSmeltType)).Cast<EnumSmeltType>().Select(e => $"{(int)e} = {e}")
    };

    [JsonProperty(Order = 5)]
    public Dictionary<string, CombustibleProperties> Blocks { get; set; } = new();

    [JsonProperty(Order = 6)]
    public Dictionary<string, CombustibleProperties> Items { get; set; } = new();

    public ConfigCombustibleProperties(ICoreAPI api, ConfigCombustibleProperties previousConfig = null)
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
            if (obj == null || obj.Code == null || obj.CombustibleProps == null)
            {
                continue;
            }

            string code = obj.Code.CodeWithoutDefaultDomain();

            if (obj is Block && !Blocks.ContainsKey(code))
            {
                CombustibleProperties props = obj.CombustibleProps.Clone();
                if (props.SmeltedStack != null)
                {
                    props.SmeltedStack.ResolvedItemstack = null;
                }
                Blocks.Add(code, props);
            }

            if (obj is Item && !Items.ContainsKey(code))
            {
                CombustibleProperties props = obj.CombustibleProps.Clone();
                if (props.SmeltedStack != null)
                {
                    props.SmeltedStack.ResolvedItemstack = null;
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
                foreach ((string key, CombustibleProperties value) in Blocks)
                {
                    if (!obj.WildCardMatch(key))
                    {
                        continue;
                    }

                    if (value.SmeltedStack != null && !value.SmeltedStack.Resolve(api.World, ""))
                    {
                        break;
                    }

                    obj.CombustibleProps = value;
                    break;
                }
                break;
            case Item when Items.Any():
                foreach ((string key, CombustibleProperties value) in Items)
                {
                    if (!obj.WildCardMatch(key))
                    {
                        continue;
                    }

                    if (value.SmeltedStack != null && !value.SmeltedStack.Resolve(api.World, ""))
                    {
                        break;
                    }

                    obj.CombustibleProps = value;
                    break;
                }
                break;
        }
    }
}