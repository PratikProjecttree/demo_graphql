using demo_graphql.Controllers;
using demo_graphql.Models.EmailModels;
using Newtonsoft.Json;

namespace demo_graphql.Services
{
    public static class DynamicEmailContent
    {
        public static Dictionary<string, string> SetTemplate(string? content, PersonDetail member)
        {
            var data = content != null ? JsonConvert.DeserializeObject<List<DynamicVariables>>(content) : new List<DynamicVariables>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    string? val = null;
                    switch (item.Variable)
                    {
                        case CommanDynamicParaNameForEmail.eventName:
                            val = member?.EventName ?? "";
                            break;
                        case CommanDynamicParaNameForEmail.ParticipantName:
                            val = member?.Name ?? "";
                            break;
                    }
                    dictionary.Add(item.Variable ?? "", val ?? "");
                }
            }
            return dictionary;
        }
    }
}