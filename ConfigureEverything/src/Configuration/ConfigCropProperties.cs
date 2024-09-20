using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration.ConfigCropProperties;

public class ConfigCropProperties : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public Dictionary<string, IEnumerable<string>> Examples = new()
    {
        [nameof(BlockCropProperties.RequiredNutrient)] = Enum.GetValues(typeof(EnumSoilNutrient)).Cast<EnumSoilNutrient>().Select(e => $"{(int)e} = {e}"),
    };

    [JsonProperty(Order = 4)]
    public Dictionary<string, BlockCropProperties> Crops { get; set; } = new();

    public ConfigCropProperties(ICoreAPI api, ConfigCropProperties previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Crops.AddRange(previousConfig.Crops);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks.Where(x => x.CropProps != null && x.CropProps?.Behaviors?.Length == 0))
        {
            string _codeWildcard = block.Code.ToString().Replace(block.Code.EndVariant().ToString(), "*");

            if (!Crops.ContainsKey(_codeWildcard))
            {
                Crops.Add(_codeWildcard, block.CropProps);
            }
        }
    }

    public void ApplyPatches(ICoreAPI api)
    {
        if (Crops?.Count == 0)
        {
            return;
        }

        foreach ((string key, BlockCropProperties value) in Crops)
        {
            Block[] blocks = api.World.SearchBlocks(new AssetLocation(key));

            if (blocks?.Length == 0)
            {
                continue;
            }

            foreach (Block block in blocks)
            {
                CropBehavior[] behaviors = block.CropProps.Behaviors;
                block.CropProps = value;
                block.CropProps.Behaviors = behaviors;
            }
        }
    }
}