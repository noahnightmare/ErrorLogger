using ErrorLogger.Models;
using Exiled.API.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLogger
{
    public class API
    {
        public static async Task SendMessage(string pluginName, string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                Log.Debug("Error was empty.");
                return;
            }

            if (pluginName == Assembly.GetExecutingAssembly().GetName().Name)
            {
                Log.Debug("Error originated from ErrorLogger, will not be logged.");
                return;
            }

            foreach (string word in ErrorLogger.Instance.Config.Blacklist)
            {
                if (pluginName.ToLower().Contains(word.ToLower()) || payload.ToLower().Contains(word.ToLower()))
                {
                    Log.Debug("Error contains blacklisted word! Error will not be logged.");
                    return;
                }
            }

            try
            {
                string webhookUrl = ErrorLogger.Instance.Config.WebhookLink;

                string serverNum = (Server.Port - 7777 + 1).ToString();

                // data being sent to the webhook
                // either embed or content based on what the user preference is
                object payloadObj;

                using (HttpClient client = new HttpClient())
                {
                    if (ErrorLogger.Instance.Config.Embeds)
                    {
                        payloadObj = new
                        {
                            embeds = new Embed[]
                            {
                                new Embed()
                                {
                                    author = new Dictionary<string, string>()
                                    {
                                        { "name", $"Server {serverNum}" }
                                    },
                                    title = $"⚠️ Error in **{pluginName}**",
                                    color = 16711680, // decimal value for red
                                    description = $"{payload}"
                                }
                            }
                        };
                    }
                    else
                    {
                        payloadObj = new
                        {
                            content = $"# **Server {serverNum}** | Port {Server.Port}\n\n⚠️ Error in **{pluginName}**:\n\n{payload}",
                        };
                    }

                    var jsonPayload = JsonConvert.SerializeObject(payloadObj, Formatting.Indented);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(webhookUrl, content);
                    Log.Debug("Error message sent to webhook");

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Debug($"Error found in {pluginName}, webhook sent to discord successfully!");
                    }
                    else
                    {
                        Log.Error($"Error in sending webhook, {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error handling the webhook: {ex.Message}");
            }

        }
    }
}
