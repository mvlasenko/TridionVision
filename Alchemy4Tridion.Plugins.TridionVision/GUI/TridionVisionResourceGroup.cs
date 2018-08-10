using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy4Tridion.Plugins.TridionVision.GUI
{
    public class TridionVisionResourceGroup : ResourceGroup
    {
        public TridionVisionResourceGroup()
        {
            AddFile("TridionVisionCommand.js");
            AddFile("Styles.css");

            AddFile<TridionVisionCommandSet>();

            AddWebApiProxy();
        }
    }
}
