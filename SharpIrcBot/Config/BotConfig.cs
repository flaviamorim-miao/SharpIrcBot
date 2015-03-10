﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpIrcBot
{
    [JsonObject(MemberSerialization.OptOut)]
    public class BotConfig
    {
        public string ServerHostname { get; set; }
        public int ServerPort { get; set; }

        public string Nickname { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ServerPassword { get; set; }
        public string Encoding { get; set; }
        public int SendDelay { get; set; }
        public List<string> AutoConnectCommands { get; set; }
        public List<string> AutoJoinChannels { get; set; }
        public List<PluginConfig> Plugins { get; set; }

        public BotConfig(JObject obj)
        {
            ServerPort = 6669;
            AutoConnectCommands = new List<string>();
            AutoJoinChannels = new List<string>();
            Plugins = new List<PluginConfig>();
            Encoding = "utf-8";
            SendDelay = 200;

            JsonSerializer.CreateDefault().Populate(obj.CreateReader(), this);

            if (Username == null)
            {
                Username = Nickname;
            }
            else if (Nickname == null)
            {
                Nickname = Username;
            }
        }
    }
}
