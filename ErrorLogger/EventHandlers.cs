using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ErrorLogger
{
    public class EventHandlers
    {
        public void ErrorLogCall(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            { 
                Task.Run(async () =>
                {
                    await ErrorLogger.Instance.SendMessage("LocalAdmin log", $"{logString}\n{stackTrace}");
                });
            }
        }
    }
}
