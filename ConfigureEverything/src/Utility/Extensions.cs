using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace ConfigureEverything;

public static class Extensions
{
    public static void EnsureAttributesNotNull(this CollectibleObject obj)
    {
        obj.Attributes ??= new JsonObject(new JObject());
    }

    public static void EnsureAttributesNotNull(this EntityProperties obj)
    {
        obj.Attributes ??= new JsonObject(new JObject());
    }
}