using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy4Tridion.Plugins.TridionVision.GUI
{
    public class TridionVisionOptionsPopupResourceGroup : ResourceGroup
    {
        public TridionVisionOptionsPopupResourceGroup()
        {
            AddFile("SearchImagesPopup.js");
            AddFile("SearchImagesPopup.css");

            Dependencies.AddLibraryJQuery();
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
            Dependencies.Add("Tridion.Web.UI.Editors.CME.commands");

            AddWebApiProxy();

            AttachToView("SearchImagesPopup.aspx");
        }
    }
}