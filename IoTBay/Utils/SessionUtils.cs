using System.Text.Json;
using System.Text.Json.Serialization;

namespace IoTBay.Utils;

public static class SessionUtils
{
    // Configure the JsonSerializer to ignore cyclical references, to avoid issues with EF Core's navigation properties 
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
    
    /// <summary>
    /// Serialize an object as Json, and store it in the session data for the calling client.
    /// </summary>
    /// <param name="session">Client session</param>
    /// <param name="key">Session data key</param>
    /// <param name="value">Session data value</param>
    public static void SetObjectAsJson(ISession session, string key, object value)
    {
        string jsonString = JsonSerializer.Serialize(value, JsonSerializerOptions);
        session.SetString(key, jsonString);
    }

    /// <summary>
    /// Deserialize an object from the session data and return it as the specified object, or null if it doesn't exist.
    /// </summary>
    /// <param name="session">Client session</param>
    /// <param name="key">Session data key</param>
    /// <typeparam name="T">Session data type</typeparam>
    /// <returns>Session data value or null</returns>
    public static T? GetObjectFromJson<T>(ISession session, string key)
    {
        var jsonString = session.GetString(key);
        var value = jsonString == null ? default : JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOptions);
        return value;
    }
}