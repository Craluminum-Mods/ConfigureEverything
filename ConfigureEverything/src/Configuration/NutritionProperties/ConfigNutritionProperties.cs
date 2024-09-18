using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration.ConfigNutritionProperties;

public class ConfigNutritionProperties : IModConfigWithDefaultValues
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool FillWithDefaultValues { get; set; }

    [JsonProperty(Order = 3)]
    public Dictionary<string, IEnumerable<string>> Examples = new()
    {
        [nameof(FoodNutritionProperties.FoodCategory)] = Enum.GetValues(typeof(EnumFoodCategory)).Cast<EnumFoodCategory>().Select(e => $"{(int)e} = {e}"),
    };

    [JsonProperty(Order = 4)]
    public Dictionary<string, FoodNutritionProperties> Food { get; set; } = new();

    public ConfigNutritionProperties(ICoreAPI api, ConfigNutritionProperties previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Food.AddRange(previousConfig.Food);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (CollectibleObject obj in api.World.Collectibles.Where(x => x.NutritionProps != null))
        {
            if (!Food.ContainsKey(obj.Code.ToString()))
            {
                Food.Add(obj.Code.ToString(), obj.NutritionProps);
            }
        }
    }
}