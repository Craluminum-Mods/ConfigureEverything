using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigSpawnConditions : IModConfigWithAutoFill
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool AutoFill { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure spawn conditions for creatures. Documentation: https://wiki.vintagestory.at/index.php/Modding:Entity_Json_Properties#p_spawnconditions";

    [JsonProperty(Order = 4)]
    public Dictionary<string, IEnumerable<string>> Examples => new()
    {
        [nameof(RuntimeSpawnConditions.ClimateValueMode)] = Enum.GetValues(typeof(EnumGetClimateMode)).Cast<EnumGetClimateMode>().Select(e => $"{(int)e} = {e}"),
        [nameof(RuntimeSpawnConditions.LightLevelType)] = Enum.GetValues(typeof(EnumLightLevelType)).Cast<EnumLightLevelType>().Select(e => $"{(int)e} = {e}"),
        [nameof(NatFloat.dist)] = Enum.GetValues(typeof(EnumDistribution)).Cast<EnumDistribution>().Select(e => $"{(int)e} = {e}"),
    };

    [JsonProperty(Order = 5)]
    public Dictionary<string, SpawnConditions> EntityTypes { get; set; } = new();

    public ConfigSpawnConditions(ICoreAPI api, ConfigSpawnConditions previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            AutoFill = previousConfig.AutoFill;

            EntityTypes.AddRange(previousConfig.EntityTypes);
        }

        if (api != null && AutoFill)
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

    public void ApplyPatches(EntityProperties obj)
    {
        if (!EntityTypes.Any())
        {
            return;
        }

        foreach ((string key, SpawnConditions value) in EntityTypes)
        {
            if (obj.WildCardMatchExt(key))
            {
                obj.Server.SpawnConditions = value;
                break;
            }
        }
    }
}