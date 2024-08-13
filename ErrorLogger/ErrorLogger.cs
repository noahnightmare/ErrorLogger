using Exiled.API.Features;
using Exiled.CreditTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MEC;
using System.Net.Http;
using HarmonyLib;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using System.Reflection;
using ErrorLogger.Models;

namespace ErrorLogger
{
    public class ErrorLogger : Plugin<Config>
    {
        public override string Author => "noah";
        public override string Name => "ErrorLogger";

        public override Version Version => new Version(1, 0, 3);
        public override Version RequiredExiledVersion => new Version(8, 9, 11);
        public override string Prefix => Name;

        public static ErrorLogger Instance;

        public EventHandlers _handlers;

        private Harmony _harmony;
        private string HarmonyId { get; } = "noah.errorlogger";

        public override void OnEnabled()
        {
            if (string.IsNullOrEmpty(Config.WebhookLink))
            {
                Log.Error("The webhook link in the config is currently empty. Please specify a valid webhook link for this plugin to work.");
                return;
            }

            Instance = this;

            RegisterEvents();
            RegisterPatch();

            /* Timing.CallDelayed(3f, () =>
            {
                Task.Run(async () =>
                {
                    await ErrorLogger.Instance.SendMessage("TestName", $"```test content```");
                });
            }); */

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            UnregisterEvents();
            UnregisterPatch();

            base.OnDisabled();
        }
        private void RegisterEvents()
        {
            _handlers = new EventHandlers();

            Application.logMessageReceived += _handlers.ErrorLogCall;
        }

        private void UnregisterEvents()
        {
            Application.logMessageReceived -= _handlers.ErrorLogCall;

            _handlers = null;
        }

        private void RegisterPatch()
        {
            try
            {
                _harmony = new(HarmonyId);
                _harmony.PatchAll();
            }
            catch (HarmonyException ex)
            {
                Log.Error($"[RegisterPatch] Patching Failed : {ex}");
            }
        }

        private void UnregisterPatch()
        {
            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;
        }

        public async Task SendMessage(string pluginName, string payload)
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

            foreach (string word in Config.Blacklist)
            {
                if (pluginName.ToLower().Contains(word.ToLower()) || payload.ToLower().Contains(word.ToLower()))
                {
                    Log.Debug("Error contains blacklisted word! Error will not be logged.");
                    return;
                }
            }

            try
            {
                string webhookUrl = Config.WebhookLink;

                string serverNum = (Server.Port - 7777 + 1).ToString();

                // data being sent to the webhook
                // either embed or content based on what the user preference is
                object payloadObj;

                using (HttpClient client = new HttpClient())
                {
                    if (Config.Embeds)
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
                            content = $"# **Server {serverNum}**\n\n⚠️ Error in **{pluginName}**:\n\n{payload}",
                        };
                    }
                    
                    var jsonPayload = JsonConvert.SerializeObject(payloadObj, Formatting.Indented);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(webhookUrl, content);
                    Log.Debug("Error message sent to webhook");

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Info("Webhook sent.");
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
