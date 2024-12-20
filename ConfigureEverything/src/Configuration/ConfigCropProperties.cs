using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigCropProperties : IModConfigWithAutoFill
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool AutoFill { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure crop growth time, required nutrients, allowed temperatures etc.";

    [JsonProperty(Order = 4)]
    public Dictionary<string, IEnumerable<string>> Examples = new()
    {
        [nameof(BlockCropProperties.RequiredNutrient)] = Enum.GetValues(typeof(EnumSoilNutrient)).Cast<EnumSoilNutrient>().Select(e => $"{(int)e} = {e}"),
    };

    [JsonProperty(Order = 5)]
    public Dictionary<string, BlockCropProperties> Crops { get; set; } = new();

    public ConfigCropProperties(ICoreAPI api, ConfigCropProperties previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            AutoFill = previousConfig.AutoFill;

            Crops.AddRange(previousConfig.Crops);
        }

        if (api != null && AutoFill)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (Block obj in api.World.Blocks.Where(x => x.CropProps != null && x.CropProps?.Behaviors?.Length == 0))
        {
            string code = obj.Code.ToString().Replace(obj.Code.EndVariant(), "*");

            if (!Crops.ContainsKey(code))
            {
                Crops.Add(code, obj.CropProps);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        if (obj is not Block block || !Crops.Any())
        {
            return;
        }

        foreach ((string key, BlockCropProperties value) in Crops)
        {
            if (obj.WildCardMatchExt(key))
            {
                CropBehavior[] behaviors = block.CropProps.Behaviors;
                block.CropProps = value;
                block.CropProps.Behaviors = behaviors;
                break;
            }
        }
    }
}