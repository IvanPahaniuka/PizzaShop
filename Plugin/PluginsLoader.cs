using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Plugin
{
    public static class PluginsLoader
    {
        public static IEnumerable<Plugin> LoadPlugins(string dir)
        {
            string[] pluginFiles = Directory.GetFiles(dir, "*.dll");
            var plugins = new List<Plugin>();

            foreach (string pluginPath in pluginFiles)
            {
                Type[] types = null;
                try
                {
                    Assembly assembly = Assembly.LoadFrom(pluginPath);
                    if (assembly != null)
                    {
                        types = assembly.GetTypes()?.Where(t => t.IsSubclassOf(typeof(Plugin))).ToArray();
                    }
                }
                catch
                {
                    continue;
                }

                var tmpPlugins = new List<Plugin>(types.Length);
                try
                {
                    if (types != null)
                    {

                        foreach (var type in types)
                            tmpPlugins.Add((Plugin)Activator.CreateInstance(type));
                    }
                }
                catch
                {
                    continue;
                }

                if (tmpPlugins != null)
                    plugins.AddRange(tmpPlugins);
            }

            return plugins;
        }
    }
}
