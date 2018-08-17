using System;
using System.Collections.Generic;
using System.Web.Http;
using Alchemy4Tridion.Plugins.Clients.CoreService;
using Alchemy4Tridion.Plugins.TridionVision.Helpers;
using Alchemy4Tridion.Plugins.TridionVision.Models;
using Google.Cloud.Vision.V1;
using System.Linq;

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
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Projects\\GoogleVisionTest\\tridion-vision.json");

                string val = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

                // Instantiates a client
                var client = ImageAnnotatorClient.Create();
                // Load the image file into memory
                //var image = Image.FromFile("c:\\Projects\\GoogleVisionTest\\testl.jpg");


                string html = "<table class=\"usingItems results\">";

                List<ItemInfo> items = CoreServiceHelper.GetItemsByParentContainer(this.Clients.SessionAwareCoreServiceClient, "tcm:" + tcmFolder, true, new ComponentType[] { ComponentType.Multimedia });

                html += CreateItemsHeading();

                // Iterate over all items returned by the above filtered list returned.
                foreach (ItemInfo item in items)
                {
                    ComponentData component = CoreServiceHelper.ReadItem(this.Clients.SessionAwareCoreServiceClient, item.TcmId) as ComponentData;
                    if (component == null || component.BinaryContent == null)
                        continue;

                    item.Icon = "/WebUI/Editors/Base/icon.png?target=view&maxwidth=40&maxheight=200&uri=tcm%3A" + item.TcmId.Replace("tcm:", "") + "&mode=thumb&modified=" + component.VersionInfo.RevisionDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.000");

                    List<string> list = new List<string>();

                    List<string> usedKeywords = CoreServiceHelper.GetUsedItems(this.Clients.SessionAwareCoreServiceClient, item.TcmId, new ItemType[] { ItemType.Keyword });
                    if (usedKeywords != null && usedKeywords.Any())
                    {


                    }
                    else
                    {
                        byte[] bytes = CoreServiceHelper.GetBinaryFromMultimediaComponent(this.Clients.SessionAwareCoreServiceDownloadClient, component);

                        var image = Image.FromBytes(bytes);

                        var response1 = client.DetectLabels(image);
                        list.AddRange(response1.ToList().Select(x => x.Description));

                        //detect landmarks 
                        if (list.Contains("landmark"))
                        {
                            var response2 = client.DetectLandmarks(image);
                            list.AddRange(response2.ToList().Select(x => x.Description));
                        }

                        //detect logos 
                        if (list.Contains("logo"))
                        {
                            var response3 = client.DetectLogos(image);
                            list.AddRange(response3.ToList().Select(x => x.Description));
                        }
                    }

                    item.Path = string.Join(", ", list);

                    if (word == "all" || list.Contains(word))
                    {
                        html += CreateItem(item) + Environment.NewLine;
                    }
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
            html += "<th class=\"icon\">Image</th>";
            html += "<th class=\"name\">Name</th>";
            html += "<th class=\"path\">Keywords</th>";
            html += "</tr>";

            return html;
        }

        private string CreateItem(ItemInfo item)
        {
            string html = "";
            html += string.Format("<tr class=\"item\" id=\"{0}\">", item.TcmId);
            html += string.Format("<td class=\"icon\"><img src='{0}' width='40'/></td>", item.Icon);
            html += string.Format("<td class=\"name\" title=\"{0} ({1})\">{0}</td>", item.Title, item.TcmId);
            html += string.Format("<td class=\"path\" title=\"{1} ({2})\">{0}</td>", item.Path, item.Title, item.TcmId);
            html += "</tr>";
            return html;
        }

    }
}