using System;
using System.Collections.Generic;
using System.Web.Http;
using Alchemy4Tridion.Plugins.Clients.CoreService;
using Alchemy4Tridion.Plugins.TridionVision.Helpers;
using Alchemy4Tridion.Plugins.TridionVision.Models;

namespace Alchemy4Tridion.Plugins.TridionVision.Controllers
{
    [AlchemyRoutePrefix("TridionVisionService")]
    public class TridionVisionController : AlchemyApiController
    {
        [HttpGet]
        [Route("Items/{tcmFolder}/{word}")]
        public string GetItems(string tcmFolder, string word)
        {
            try
            {
                string html = "<table class=\"usingItems results\">";

                List<ItemInfo> items = CoreServiceHelper.GetItemsByParentContainer(this.Clients.SessionAwareCoreServiceClient, "tcm:" + tcmFolder, true, new ItemType[] { ItemType.Component });

                html += CreateItemsHeading();

                // Iterate over all items returned by the above filtered list returned.
                foreach (ItemInfo item in items)
                {
                    html += CreateItem(item) + Environment.NewLine;
                }

                // Close the div we opened above
                html += "</table>";

                // Return the html we've built.
                return html;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CreateItemsHeading()
        {
            string html = "<tr class=\"headings\">";
            html += "<th class=\"name\" style='padding-left: 18px !important;'>Name</th>";
            html += "<th class=\"path\">Path</th>";
            html += "<th class=\"operation\">Operation</th>";
            html += "</tr>";

            return html;
        }

        private string CreateItem(ItemInfo item)
        {
            string html = "";
            html += string.Format("<tr class=\"item\" id=\"{0}\">", item.TcmId);
            html += string.Format("<td class=\"name\" title=\"{0} ({1})\"><div class=\"treeicon\" style=\"width: {3}px; text-align: right;\">{4}</div><div class=\"icon\" style=\"background-image: url(/WebUI/Editors/CME/Themes/Carbon2/icon_v7.1.0.66.627_.png?name={2}&size=16)\"></div><div class=\"title\">{0}</div></td>", item.Title, item.TcmId, item.Icon, 16, "");
            html += string.Format("<td class=\"path\" title=\"{1} ({2})\">{0}</td>", item.Path, item.Path + "\\" + item.Title, item.TcmId);
            html += string.Format("<td class=\"operation\" title=\"{1}\"><img src=\"/Alchemy/Plugins/Tridion_Vision/assets/img/{0}\"/></td>", item.Icon, item.Title);
            html += "</tr>";
            return html;
        }

    }
}