﻿using System;
using log4net;
using Newtonsoft.Json.Linq;
using SharpIrcBot;
using SharpIrcBot.Events;

namespace IdentityNickMapping
{
    public class IdentityPlugin : IPlugin
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IConnectionManager ConnectionManager;

        public IdentityPlugin(IConnectionManager connMgr, JObject config)
        {
            ConnectionManager = connMgr;

            ConnectionManager.NickMapping += HandleNickMapping;
        }

        protected virtual void HandleNickMapping(object sender, NickMappingEventArgs e)
        {
            try
            {
                ActuallyHandleNickMapping(sender, e);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling nick mapping", exc);
            }
        }

        protected virtual void ActuallyHandleNickMapping(object sender, NickMappingEventArgs e)
        {
            e.MapsTo.Add(e.Nickname);
        }
    }
}
