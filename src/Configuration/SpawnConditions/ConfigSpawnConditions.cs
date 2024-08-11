using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace ConfigureEverything.Configuration.ConfigSpawnConditions;

public class ConfigSpawnConditions : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }
    public bool FillWithDefaultValues { get; set; }

    public readonly string Documentation = "https://wiki.vintagestory.at/index.php/Modding:Entity_Json_Properties";

    public readonly Dictionary<string, List<string>> Examples = new()
    {
        [nameof(RuntimeSpawnConditions.ClimateValueMode)] = Enum.GetValues(typeof(EnumGetClimateMode)).Cast<EnumGetClimateMode>().Select(e => $"{(int)e} = {e}").ToList(),
        [nameof(RuntimeSpawnConditions.LightLevelType)] = Enum.GetValues(typeof(EnumLightLevelType)).Cast<EnumLightLevelType>().Select(e => $"{(int)e} = {e}").ToList(),
        [nameof(NatFloat.dist)] = Enum.GetValues(typeof(EnumDistribution)).Cast<EnumDistribution>().Select(e => $"{(int)e} = {e}").ToList(),
    };

    public Dictionary<string, SpawnConditions> EntityTypes { get; set; } = new();

    public ConfigSpawnConditions(ICoreAPI api, ConfigSpawnConditions previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            Documentation = previousConfig.Documentation;
            Examples = previousConfig.Examples;

            foreach ((string key, SpawnConditions value) in previousConfig.EntityTypes)
            {
                if (!EntityTypes.ContainsKey(key))
                {
                    EntityTypes.Add(key, value);
                }
            }
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (EntityProperties entityType in api.World.EntityTypes)
        {
            if (entityType?.Server?.SpawnConditions != null && !EntityTypes.ContainsKey(entityType.Code.ToString()))
            {
                EntityTypes.Add(entityType.Code.ToString(), entityType.Server.SpawnConditions);
            }
        }
    }
}