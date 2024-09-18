using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration.ConfigSpawnConditions;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigSpawnConditions config)
    {
        if (config.EntityTypes?.Count == 0)
        {
            return;
        }

        foreach (EntityProperties entityType in api.World.EntityTypes)
        {
            foreach ((string key, SpawnConditions value) in config.EntityTypes)
            {
                if (WildcardUtil.Match(key, entityType.Code.ToString()))
                {
                    entityType.Server.SpawnConditions = value;
                    break;
                }
            }
        }
    }
}