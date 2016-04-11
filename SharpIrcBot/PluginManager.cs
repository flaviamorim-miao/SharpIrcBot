﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using Newtonsoft.Json.Linq;
using SharpIrcBot.Config;

namespace SharpIrcBot
{
    public class PluginManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [NotNull]
        protected BotConfig Config;
        [NotNull, ItemNotNull]
        protected List<IPlugin> Plugins;

        public PluginManager([NotNull] BotConfig config)
        {
            Config = config;
            Plugins = new List<IPlugin>();
        }

        public void LoadPlugins([NotNull] IConnectionManager connManager)
        {
            foreach (var plugin in Config.Plugins)
            {
                var ass = Assembly.Load(plugin.Assembly);
                var type = ass.GetType(plugin.Class);
                if (!typeof(IPlugin).IsAssignableFrom(type))
                {
                    throw new ArgumentException("class is not a plugin");
                }
                var ctor = type.GetConstructor(new [] {typeof(IConnectionManager), typeof(JObject)});
                var pluginObject = (IPlugin)ctor.Invoke(new object[] {connManager, plugin.Config});
                Plugins.Add(pluginObject);
            }
        }

        public void ReloadConfigurations([NotNull] List<PluginConfig> newPluginConfigs)
        {
            if (Plugins.Count != newPluginConfigs.Count)
            {
                throw new ArgumentException("number of plugins changed", nameof(newPluginConfigs));
            }

            foreach (var pluginPair in Enumerable.Zip(Plugins, newPluginConfigs, Tuple.Create))
            {
                IPlugin plugin = pluginPair.Item1;
                PluginConfig newConfig = pluginPair.Item2;

                var pluginType = plugin.GetType();

                if (pluginType.FullName != newConfig.Class)
                {
                    throw new ArgumentException($"plugin order changed; existing plugin of type {plugin.GetType().FullName} clashes with configured plugin of type {newConfig.Class}", nameof(newPluginConfigs));
                }

                var updatablePlugin = plugin as IReloadableConfiguration;
                if (updatablePlugin == null)
                {
                    // nope
                    continue;
                }

                Logger.InfoFormat("updating configuration of plugin of type {0}", pluginType.FullName);
                try
                {
                    updatablePlugin.ReloadConfiguration(newConfig.Config);
                }
                catch (Exception exc)
                {
                    Logger.ErrorFormat("failed to update configuration of plugin of type {0}: {1}", pluginType.FullName, exc);
                }
            }
        }
    }
}
