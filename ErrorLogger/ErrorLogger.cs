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

namespace ErrorLogger
{
    public class ErrorLogger : Plugin<Config>
    {
        public override string Author => "noah";
        public override string Name => "ErrorLogger";
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

            try
            {
                string webhookUrl = Config.WebhookLink;

                string serverNum = (Server.Port - 7777 + 1).ToString();

                using (HttpClient client = new HttpClient())
                {
                    var payloadObj = new { content = $"# **[SERVER {serverNum}]**\n\nError in **{pluginName}**:\n\n{payload}" };
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
