using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpIrcBot.Plugins.CasinoBot.Player
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PlayerConfig
    {
        public string CasinoChannel { get; set; }
        public string GameMasterNickname { get; set; }
        public List<string> Curses { get; set; }
        public int CurseNum { get; set; }
        public int CurseDen { get; set; }
        public List<string> Gloats { get; set; }
        public int GloatNum { get; set; }
        public int GloatDen { get; set; }
        public decimal AddRiskFactor { get; set; }
        public decimal MulRiskFactor { get; set; }
        public int MinBet { get; set; }
        public int MaxBet { get; set; }

        public PlayerConfig(JObject obj)
        {
            CasinoChannel = "#casino";
            GameMasterNickname = "CasinoBot";
            Curses = new List<string>();
            CurseNum = 1;
            CurseDen = 5;
            Gloats = new List<string>();
            GloatNum = 1;
            GloatDen = 1;
            AddRiskFactor = 0.0m;
            MulRiskFactor = 0.5m;
            MinBet = 5;
            MaxBet = 400;

            var ser = new JsonSerializer();
            ser.Populate(obj.CreateReader(), this);
        }
    }
}
