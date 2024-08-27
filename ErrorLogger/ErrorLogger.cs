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

        public override Version Version { get; } = new Version(1, 0, 4);
        public override Version RequiredExiledVersion { get; } = new Version(8, 11, 0);
        public override string Prefix => Name;

        public static ErrorLogger Instance;

        public EventHandlers _handlers;

        public Harmony _harmony;

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
                _harmony = new($"ErrorLogger.{DateTime.UtcNow.Ticks}");
                _harmony.PatchAll();
            }
            catch (HarmonyException ex)
            {
                Log.Error($"[RegisterPatch] Patching Failed : {ex}");
            }
        }

        private void UnregisterPatch()
        {
            _harmony.UnpatchAll(_harmony.Id);
            _harmony = null;
        }
    }
}
