global using ConfigureEverything.Configuration;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

[assembly: ModInfo(name: "Configure Everything", modID: "configureeverything", Side = "Universal", RequiredOnClient = false, RequiredOnServer = false)]

namespace ConfigureEverything;

public class Core : ModSystem
{
    public static ConfigMapColors ConfigMapColors { get; private set; }

    public static ConfigBlockFertility ConfigBlockFertility { get; private set; }
    public static ConfigBlockMiningTier ConfigBlockMiningTier { get; private set; }
    public static ConfigBlockResistance ConfigBlockResistance { get; private set; }
    public static ConfigCombustibleProperties ConfigCombustibleProperties { get; private set; }
    public static ConfigCropProperties ConfigCropProperties { get; private set; }
    public static ConfigDurability ConfigDurability { get; private set; }
    public static ConfigGrindingProperties ConfigGrindingProperties { get; private set; }
    public static ConfigItemDimensions ConfigItemDimensions { get; private set; }
    public static ConfigNutritionProperties ConfigNutritionProperties { get; private set; }
    public static ConfigSpawnConditions ConfigSpawnConditions { get; private set; }
    public static ConfigStackSizes ConfigStackSizes { get; private set; }
    public static ConfigToolMiningSpeed ConfigToolMiningSpeed { get; private set; }
    public static ConfigToolTier ConfigToolTier { get; private set; }
    public static ConfigTransitionableProperties ConfigTransitionableProperties { get; private set; }

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
            ConfigBlockFertility = ModConfig.ReadConfig<ConfigBlockFertility>(api, $"ConfigureEverything/{api.Side}/BlockFertility.json");
            ConfigBlockMiningTier = ModConfig.ReadConfig<ConfigBlockMiningTier>(api, $"ConfigureEverything/{api.Side}/BlockMiningTier.json");
            ConfigBlockResistance = ModConfig.ReadConfig<ConfigBlockResistance>(api, $"ConfigureEverything/{api.Side}/BlockResistance.json");
            ConfigCombustibleProperties = ModConfig.ReadConfig<ConfigCombustibleProperties>(api, $"ConfigureEverything/{api.Side}/CombustibleProperties.json");
            ConfigCropProperties = ModConfig.ReadConfig<ConfigCropProperties>(api, $"ConfigureEverything/{api.Side}/CropProperties.json");
            ConfigDurability = ModConfig.ReadConfig<ConfigDurability>(api, $"ConfigureEverything/{api.Side}/Durability.json");
            ConfigGrindingProperties = ModConfig.ReadConfig<ConfigGrindingProperties>(api, $"ConfigureEverything/{api.Side}/GrindingProperties.json");
            ConfigItemDimensions = ModConfig.ReadConfig<ConfigItemDimensions>(api, $"ConfigureEverything/{api.Side}/ItemDimensions.json");
            ConfigNutritionProperties = ModConfig.ReadConfig<ConfigNutritionProperties>(api, $"ConfigureEverything/{api.Side}/NutritionProperties.json");
            ConfigSpawnConditions = ModConfig.ReadConfig<ConfigSpawnConditions>(api, $"ConfigureEverything/{api.Side}/SpawnConditions.json");
            ConfigStackSizes = ModConfig.ReadConfig<ConfigStackSizes>(api, $"ConfigureEverything/{api.Side}/StackSizes.json");
            ConfigToolMiningSpeed = ModConfig.ReadConfig<ConfigToolMiningSpeed>(api, $"ConfigureEverything/{api.Side}/ToolMiningSpeed.json");
            ConfigToolTier = ModConfig.ReadConfig<ConfigToolTier>(api, $"ConfigureEverything/{api.Side}/ToolTier.json");
            ConfigTransitionableProperties = ModConfig.ReadConfig<ConfigTransitionableProperties>(api, $"ConfigureEverything/{api.Side}/TransitionableProperties.json");

            foreach (CollectibleObject obj in api.World.Collectibles)
            {   
                if (obj == null || obj.Code == null) continue;

                if (ConfigBlockFertility?.Enabled == true) ConfigBlockFertility.ApplyPatches(obj);
                if (ConfigBlockMiningTier?.Enabled == true) ConfigBlockMiningTier.ApplyPatches(obj);
                if (ConfigBlockResistance?.Enabled == true) ConfigBlockResistance.ApplyPatches(obj);
                if (ConfigCombustibleProperties?.Enabled == true) ConfigCombustibleProperties.ApplyPatches(obj, api);
                if (ConfigCropProperties?.Enabled == true) ConfigCropProperties.ApplyPatches(obj);
                if (ConfigDurability?.Enabled == true) ConfigDurability.ApplyPatches(obj);
                if (ConfigGrindingProperties?.Enabled == true) ConfigGrindingProperties.ApplyPatches(obj, api);
                if (ConfigItemDimensions?.Enabled == true) ConfigItemDimensions.ApplyPatches(obj);
                if (ConfigNutritionProperties?.Enabled == true) ConfigNutritionProperties.ApplyPatches(obj);
                if (ConfigStackSizes?.Enabled == true) ConfigStackSizes.ApplyPatches(obj);
                if (ConfigToolMiningSpeed?.Enabled == true) ConfigToolMiningSpeed.ApplyPatches(obj);
                if (ConfigToolTier?.Enabled == true) ConfigToolTier.ApplyPatches(obj);
                if (ConfigTransitionableProperties?.Enabled == true) ConfigTransitionableProperties.ApplyPatches(obj, api);
            }

            foreach (EntityProperties entityType in api.World.EntityTypes)
            {
                if (ConfigSpawnConditions?.Enabled == true) ConfigSpawnConditions.ApplyPatches(entityType);
            }
        }

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}