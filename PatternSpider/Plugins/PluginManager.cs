using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;


namespace PatternSpider.Plugins
{
    class PluginManager
    {
        [ImportMany] // This is a signal to the MEF framework to load all matching exported assemblies.
        public IEnumerable<IPlugin> Plugins { get; private set; }
        
        public PluginManager()
        {
            LoadPlugins();
        }
        
        public void ReloadPlugins()
        {
            UnloadPlugins();
            LoadPlugins();            
        }

        public void UnloadPlugins()
        {
            if (Plugins != null)
            {
                Plugins = null;
                Console.WriteLine("Unloaded Plugins");    
            }            
        }
       
        private void LoadPlugins()
        {
            if (!Directory.Exists("Plugins"))
            {
                Directory.CreateDirectory("Plugins");
            }
            
            var catalog = new DirectoryCatalog("Plugins", "*.dll");
            
            using (var container = new CompositionContainer(catalog))
            {                
                container.ComposeParts(this);
            }

            string pluginNames = "";

            foreach (var plugin in Plugins)
            {
                pluginNames += " " + plugin.Name;
            }

            Console.WriteLine("Loaded the following plugins: {0}", pluginNames.Trim());
        }
    }
}
