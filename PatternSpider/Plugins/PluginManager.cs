using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using IrcDotNet;

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
            Plugins = null;
            LoadPlugins();
        }

        private void LoadPlugins()
        {
            var catalog = new DirectoryCatalog("Plugins", "*.dll");
            
            using (var container = new CompositionContainer(catalog))
            {                
                container.ComposeParts(this);
            }
        }
    }
}
