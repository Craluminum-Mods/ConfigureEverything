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
    public Dictionary<string, FoodNutritionProperties> Blocks { get; set; } = new();

    [JsonProperty(Order = 5)]
    public Dictionary<string, FoodNutritionProperties> Items { get; set; } = new();

    public ConfigNutritionProperties(ICoreAPI api, ConfigNutritionProperties previousConfig = null)
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
        foreach (CollectibleObject obj in api.World.Collectibles.Where(x => x.NutritionProps != null))
        {
            if (obj == null || obj.Code == null)
            {
                return;
            }

            string code = obj.Code.ToString().Replace("game:", "");

            if (obj is Block && !Blocks.ContainsKey(code))
            {
                Blocks.Add(code, obj.NutritionProps);
            }

            if (obj is Item && !Items.ContainsKey(code))
            {
                Items.Add(code, obj.NutritionProps);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, FoodNutritionProperties value) in Blocks)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.NutritionProps = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, FoodNutritionProperties value) in Items)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.NutritionProps = value;
                        break;
                    }
                }
                break;
        }
    }
}