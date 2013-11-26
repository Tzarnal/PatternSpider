using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace PatternSpider.Plugins
{
    class PluginManager
    {
        [ImportMany] // This is a signal to the MEF framework to load all matching exported assemblies.
        private IEnumerable<IPlugin> Plugins { get; set; }
        
        public void LoadPlugins()
        {
            var catalog = new DirectoryCatalog("Plugins", "*.dll");
            
            using (var container = new CompositionContainer(catalog))
            {                
                container.ComposeParts(this);
            }

            foreach (var plugin in Plugins)
            {
                plugin.ParseMessage("Hi");
            }
        }
    }
}
