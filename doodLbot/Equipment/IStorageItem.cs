using Newtonsoft.Json;

namespace doodLbot.Equipment
{
    public interface IStorageItem
    {
        [JsonProperty("name")]
        string Name { get; }

        int Cost { get; }
    }
}
