using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Diagnostics;

namespace ErrorLogger.Patches
{
    [HarmonyPatch(typeof(Log), nameof(Log.Error), typeof(object))]
    internal static class ErrorPatch
    {
        private static void Postfix(object message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Task.Run(async () =>
            {
                await ErrorLogger.Instance.SendMessage(callingAssembly.GetName().Name, $"```{message}```");
            });
        }
    }

    [HarmonyPatch(typeof(Log), nameof(Log.Error), typeof(string))]
    internal static class ErrorStringPatch
    {
        private static void Postfix(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Task.Run(async () =>
            {
                await ErrorLogger.Instance.SendMessage(callingAssembly.GetName().Name, $"```{message}```");
            });
        }
    }
}
