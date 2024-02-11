using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace itu_new_minitwit.Utils;

public static class TempDataExtensions
{
    public static void QueueFlashMessage(this ITempDataDictionary tempData, string value)
    {
        List<string> messages = tempData.Get<List<string>>("FLASH_MESSAGE") ?? new();

        messages.Add(value);

        tempData.Add("FLASH_MESSAGE", messages);
    }

    public static T? Get<T>(this ITempDataDictionary tempData, string key)
    {
        if (tempData.TryGetValue(key, out object? o))
            if (o is T)
                return (T)o;

        return default;
    }
}
