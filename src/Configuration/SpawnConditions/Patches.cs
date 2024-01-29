using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace ConfigureEverything.Configuration.ConfigSpawnConditions;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigSpawnConditions config)
    {
        if (config.EntityTypes?.Count == 0)
        {
            return;
        }

        foreach ((string key, SpawnConditions value) in config.EntityTypes)
        {
            EntityProperties entityType = api.World.GetEntityType(new AssetLocation(key));

            if (entityType == null || entityType.Code == null)
            {
                continue;
            };

            entityType.Server.SpawnConditions = value;
        }
    }
}