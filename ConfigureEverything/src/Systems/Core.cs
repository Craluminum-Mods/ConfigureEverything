using ConfigureEverything.Configuration;
using ConfigureEverything.Configuration.ConfigCropProperties;
using ConfigureEverything.Configuration.ConfigDurability;
using ConfigureEverything.Configuration.ConfigMapColors;
using ConfigureEverything.Configuration.ConfigNutritionProperties;
using ConfigureEverything.Configuration.ConfigSpawnConditions;
using ConfigureEverything.Configuration.ConfigStackSizes;
using ConfigureEverything.Configuration.ConfigTransitionableProperties;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

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

    public static ConfigMapColors ConfigMapColors { get; private set; }

    public override void StartPre(ICoreAPI api)
    {
        if (api.Side.IsClient())
        {
            ConfigMapColors = ModConfig.ReadConfig<ConfigMapColors>(api, $"ConfigureEverything/{api.Side}/MapColors.json");

            if (ConfigMapColors?.Enabled == true) ConfigMapColors.ApplyPatches();
        }        
    }

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

            foreach (CollectibleObject obj in api.World.Collectibles)
            {   
                if (obj == null || obj.Code == null) continue;

                if (ConfigCropProperties?.Enabled == true) ConfigCropProperties.ApplyPatches(obj);
                if (ConfigDurability?.Enabled == true) ConfigDurability.ApplyPatches(obj);
                if (ConfigNutritionProperties?.Enabled == true) ConfigNutritionProperties.ApplyPatches(obj);
                if (ConfigStackSizes?.Enabled == true) ConfigStackSizes.ApplyPatches(obj);
                //if (ConfigTransitionableProperties?.Enabled == true) ConfigTransitionableProperties.ApplyPatches(api, obj);
            }

            foreach (EntityProperties entityType in api.World.EntityTypes)
            {
                if (ConfigSpawnConditions?.Enabled == true) ConfigSpawnConditions.ApplyPatches(entityType);
            }
        }

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}