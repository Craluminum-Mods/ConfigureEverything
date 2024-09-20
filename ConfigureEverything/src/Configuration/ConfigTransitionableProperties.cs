using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigTransitionableProperties : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public readonly Dictionary<string, IEnumerable<string>> Examples = new()
    {
        [nameof(TransitionableProperties.Type)] = Enum.GetValues(typeof(EnumTransitionType)).Cast<EnumTransitionType>().Select(e => $"{(int)e} = {e}"),
        [nameof(TransitionableProperties.FreshHours.dist)] = Enum.GetValues(typeof(EnumDistribution)).Cast<EnumDistribution>().Select(e => $"{(int)e} = {e}"),
    };

    [JsonProperty(Order = 4)]
    public Dictionary<string, TransitionableProperties[]> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, TransitionableProperties[]> Items { get; set; } = new();

    public ConfigTransitionableProperties(ICoreAPI api, ConfigTransitionableProperties previousConfig = null)
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
        foreach (Block obj in api.World.Blocks.Where(x => x.TransitionableProps != null && x.TransitionableProps?.Length != 0))
        {
            string code = obj.Code.ToString().Replace("game:", "");
            if (!Blocks.ContainsKey(code))
            {
                TransitionableProperties[] transitionableProps = obj.TransitionableProps;
                transitionableProps.Foreach(x => x.TransitionedStack.ResolvedItemstack = null);
                Blocks.Add(code, transitionableProps);
            }
        }

        foreach (Item obj in api.World.Items.Where(x => x.TransitionableProps != null && x.TransitionableProps?.Length != 0))
        {
            string code = obj.Code.ToString().Replace("game:", "");
            if (!Items.ContainsKey(code))
            {
                TransitionableProperties[] transitionableProps = obj.TransitionableProps;
                transitionableProps.Foreach(x => x.TransitionedStack.ResolvedItemstack = null);
                Items.Add(code, transitionableProps);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj, ICoreAPI api)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, TransitionableProperties[] value) in Blocks)
                {
                    if (obj.WildCardMatch(key) && value.All(x => x.TransitionedStack.Resolve(api.World, "")))
                    {
                        obj.TransitionableProps = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, TransitionableProperties[] value) in Items)
                {
                    if (obj.WildCardMatch(key) && value.All(x => x.TransitionedStack.Resolve(api.World, "")))
                    {
                        obj.TransitionableProps = value;
                        break;
                    }
                }
                break;
        }
    }
}