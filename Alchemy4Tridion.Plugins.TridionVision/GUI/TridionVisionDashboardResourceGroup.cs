using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy4Tridion.Plugins.TridionVision.GUI
{
    public class TridionVisionDashboardResourceGroup : ResourceGroup
    {
        public TridionVisionDashboardResourceGroup()
        {
            AddFile("TridionVisionDashboard.js");

            Dependencies.AddLibraryJQuery();

            AddWebApiProxy();
        }
    }

    public class TridionVisionDashboardExtensionGroup : ExtensionGroup
    {
        public TridionVisionDashboardExtensionGroup()
        {
            AddExtension<TridionVisionDashboardResourceGroup>("Tridion.Web.UI.Editors.CME.Views.Dashboard");
        }

    }

}