using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy4Tridion.Plugins.TridionVision.GUI
{
    public class TridionVisionRibbonToolbarButton : RibbonToolbarExtension
    {
        public TridionVisionRibbonToolbarButton()
        {
            // The unique identifier used for the html element created.
            AssignId = "TridionVisionButton";

            // Using command instead of .ascx so we can position it correctly
            Command = "TridionVisionCommand";

            // The label of the button.
            Name = "Auto-classify Images";

            // The tooltip label that will get applied.
            Title = "Auto-classify Images";

            // The page tab to assign this extension to. See Constants.PageIds.
            PageId = Constants.PageIds.HomePage;

            // Option GroupId, put this into an existing group
            GroupId = "TridionVision";

            // We need to add our resource group as a dependency to this extension
            Dependencies.Add<TridionVisionResourceGroup>();

            // apply the extension to a specific view.
            Apply.ToView(Constants.Views.DashboardView, "DashboardToolbar");
        }
    }
}