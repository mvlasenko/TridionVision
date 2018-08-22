using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy4Tridion.Plugins.TridionVision.GUI
{
    public class TridionVisionContextMenuExtension : ContextMenuExtension
    {
        public TridionVisionContextMenuExtension()
        {
            AssignId = "TridionVisionContextMenu";

            // The name of the extension menu
            Name = "TridionVisionContextMenu";

            // Use this property to specify where in the context menu your items will go
            InsertBefore = Constants.ContextMenuIds.MainContextMenu.Versioning;

            AddItem("cm_tridion_vision", "Auto-classify Images", "TridionVisionCommand");

            // We need to add our resource group as a dependency to this extension
            Dependencies.Add<TridionVisionResourceGroup>();

            // apply the extension to a specific view.
            Apply.ToView(Constants.Views.DashboardView);
        }
    }
}
