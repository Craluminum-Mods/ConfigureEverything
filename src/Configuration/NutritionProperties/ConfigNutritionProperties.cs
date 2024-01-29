using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigNutritionProperties;

public class ConfigNutritionProperties : IModConfig
{
    public bool Enabled { get; set; }

    public readonly Dictionary<string, List<string>> Examples = new()
    {
        [nameof(FoodNutritionProperties.FoodCategory)] = Enum.GetValues(typeof(EnumFoodCategory)).Cast<EnumFoodCategory>().Select(e => $"{(int)e} = {e}").ToList(),
    };

    public Dictionary<string, FoodNutritionProperties> Food { get; set; } = new();

    public ConfigNutritionProperties(ICoreAPI api, ConfigNutritionProperties previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            Examples = previousConfig.Examples;

            foreach ((string key, FoodNutritionProperties value) in previousConfig.Food)
            {
                if (!Food.ContainsKey(key))
                {
                    Food.Add(key, value);
                }
            }
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
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