using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration.ConfigTransitionableProperties;

public class ConfigTransitionableProperties : IModConfig
{
    public bool Enabled { get; set; }

    public readonly Dictionary<string, List<string>> Examples = new()
    {
        [nameof(TransitionableProperties.Type)] = Enum.GetValues(typeof(EnumTransitionType)).Cast<EnumTransitionType>().Select(e => $"{(int)e} = {e}").ToList(),
        [nameof(TransitionableProperties.FreshHours.dist)] = Enum.GetValues(typeof(EnumDistribution)).Cast<EnumDistribution>().Select(e => $"{(int)e} = {e}").ToList(),
    };

    public Dictionary<string, TransitionableProperties[]> BlocksTransitionableProperties { get; set; } = new();
    public Dictionary<string, TransitionableProperties[]> ItemsTransitionableProperties { get; set; } = new();

    public ConfigTransitionableProperties(ICoreAPI api, ConfigTransitionableProperties previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            Examples = previousConfig.Examples;
            BlocksTransitionableProperties.AddRange(previousConfig.BlocksTransitionableProperties);
            ItemsTransitionableProperties.AddRange(previousConfig.ItemsTransitionableProperties);
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks.Where(x => x.TransitionableProps != null && x.TransitionableProps?.Length != 0))
        {
            if (!BlocksTransitionableProperties.ContainsKey(block.Code.ToString()))
            {
                TransitionableProperties[] transitionableProps = block.TransitionableProps;
                transitionableProps.Foreach(x => x.TransitionedStack.ResolvedItemstack = null);
                BlocksTransitionableProperties.Add(block.Code.ToString(), transitionableProps);
            }
        }

        foreach (Item item in api.World.Items.Where(x => x.TransitionableProps != null && x.TransitionableProps?.Length != 0))
        {
            if (!ItemsTransitionableProperties.ContainsKey(item.Code.ToString()))
            {
                TransitionableProperties[] transitionableProps = item.TransitionableProps;
                transitionableProps.Foreach(x => x.TransitionedStack.ResolvedItemstack = null);
                ItemsTransitionableProperties.Add(item.Code.ToString(), transitionableProps);
            }
        }
    }
}