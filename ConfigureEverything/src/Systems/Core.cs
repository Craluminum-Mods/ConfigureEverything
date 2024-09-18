using ConfigureEverything.Configuration;
using ConfigureEverything.Configuration.ConfigCropProperties;
using ConfigureEverything.Configuration.ConfigDurability;
using ConfigureEverything.Configuration.ConfigNutritionProperties;
using ConfigureEverything.Configuration.ConfigSpawnConditions;
using ConfigureEverything.Configuration.ConfigStackSizes;
using ConfigureEverything.Configuration.ConfigTransitionableProperties;
using Vintagestory.API.Common;

[assembly: ModInfo(name: "Configure Everything", modID: "configureeverything", Side = "Universal", RequiredOnClient = false, RequiredOnServer = false)]

namespace ConfigureEverything;

public class Core : ModSystem
{
    public static ConfigCropProperties ConfigCropProperties { get; private set; }
    public static ConfigDurability ConfigDurability { get; private set; }
    public static ConfigNutritionProperties ConfigNutritionProperties { get; private set; }
    public static ConfigSpawnConditions ConfigSpawnConditions { get; private set; }
    public static ConfigStackSizes ConfigStackSizes { get; private set; }
    public static ConfigTransitionableProperties ConfigTransitionableProperties { get; private set; }

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side.IsServer())
        {
            ConfigCropProperties = ModConfig.ReadConfig<ConfigCropProperties>(api, $"ConfigureEverything/{api.Side}/CropProperties.json");
            ConfigDurability = ModConfig.ReadConfig<ConfigDurability>(api, $"ConfigureEverything/{api.Side}/Durability.json");
            ConfigNutritionProperties = ModConfig.ReadConfig<ConfigNutritionProperties>(api, $"ConfigureEverything/{api.Side}/NutritionProperties.json");
            ConfigSpawnConditions = ModConfig.ReadConfig<ConfigSpawnConditions>(api, $"ConfigureEverything/{api.Side}/SpawnConditions.json");
            ConfigStackSizes = ModConfig.ReadConfig<ConfigStackSizes>(api, $"ConfigureEverything/{api.Side}/StackSizes.json");
            ConfigTransitionableProperties = ModConfig.ReadConfig<ConfigTransitionableProperties>(api, $"ConfigureEverything/{api.Side}/TransitionableProperties.json");

            if (ConfigCropProperties?.Enabled == true) api.ApplyPatches(ConfigCropProperties);
            if (ConfigDurability?.Enabled == true) api.ApplyPatches(ConfigDurability);
            if (ConfigNutritionProperties?.Enabled == true) api.ApplyPatches(ConfigNutritionProperties);
            if (ConfigSpawnConditions?.Enabled == true) api.ApplyPatches(ConfigSpawnConditions);
            if (ConfigStackSizes?.Enabled == true) api.ApplyPatches(ConfigStackSizes);
            if (ConfigTransitionableProperties?.Enabled == true) api.ApplyPatches(ConfigTransitionableProperties);
        }

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}