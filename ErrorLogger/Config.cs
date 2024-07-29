using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLogger
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Specify which webhook errors should be sent to. ")]
        public string WebhookLink { get; set; } = string.Empty;

        [Description("Whether the Webhook should send it's contents in Embed form or not.")]
        public bool Embeds { get; set; } = true;

        [Description("Blacklist - if an error contains any of these words, they will not be posted through the webhook. Leave blank [] to not blacklist anything.")]
        public List<string> Blacklist { get; set; } = new List<string>()
        {
            "Word1",
            "Word2"
        };
    }
}
