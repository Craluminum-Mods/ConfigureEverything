using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigCropProperties;

public class ConfigCropProperties : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }
    public bool FillWithDefaultValues { get; set; }

    public readonly Dictionary<string, List<string>> Examples = new()
    {
        [nameof(BlockCropProperties.RequiredNutrient)] = Enum.GetValues(typeof(EnumSoilNutrient)).Cast<EnumSoilNutrient>().Select(e => $"{(int)e} = {e}").ToList(),
    };

    public Dictionary<string, BlockCropProperties> Crops { get; set; } = new();

    public ConfigCropProperties(ICoreAPI api, ConfigCropProperties previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            Examples = previousConfig.Examples;

            foreach ((string key, BlockCropProperties value) in previousConfig.Crops)
            {
                if (!Crops.ContainsKey(key))
                {
                    Crops.Add(key, value);
                }
            }
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
}