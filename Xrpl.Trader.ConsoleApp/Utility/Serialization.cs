using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xrpl.Trader.ConsoleApp.Utility;

public static class Serialization
{
    public static string Serialize(object data)
    {
        if (data is null) throw new NullReferenceException("No data to serialize.");

        return JsonConvert.SerializeObject(data, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
        });
    }

    public static T Deserialize<T>(string data)
    {
        if (data is null || data is not object) throw new NullReferenceException("Cannot deserialize invalid type.");

        return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
        {
            Error = (sender, args) => args.ErrorContext.Handled = true
        });
    }
}
