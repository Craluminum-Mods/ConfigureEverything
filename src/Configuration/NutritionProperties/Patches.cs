using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigNutritionProperties;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigNutritionProperties config)
    {
        if (config.Food?.Count == 0)
        {
            return;
        }

        foreach ((string key, FoodNutritionProperties value) in config.Food)
        {
            CollectibleObject obj = api.GetCollectible(key);

            if (obj == null || obj.Code == null)
            {
                continue;
            };

            obj.NutritionProps = value;
        }
    }
}