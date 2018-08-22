using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Alchemy4Tridion.Plugins.Clients.CoreService;
using Alchemy4Tridion.Plugins.TridionVision.Helpers;
using Alchemy4Tridion.Plugins.TridionVision.Models;
using Google.Cloud.Vision.V1;

namespace Alchemy4Tridion.Plugins.TridionVision.Controllers
{
    [AlchemyRoutePrefix("TridionVisionService")]
    public class TridionVisionController : AlchemyApiController
    {
        [HttpGet]
        [Route("Items/{tcmFolder}/{word}/{generate}")]
        public string GetItems(string tcmFolder, string word, bool generate)
        {
            try
            {
                //var yyy = CoreServiceHelper.ReadItem(this.Clients.SessionAwareCoreServiceClient, "tcm:5061-29508");

                if (!this.Clients.IsImpersonationUserSet())
                    throw new Exception("Core service client is not impersonated");

                Settings settings = Plugin.Settings.Get<Settings>();

                if(string.IsNullOrEmpty(settings.CategoryId) || settings.CategoryId == "tcm:1-1-512")
                    throw new Exception("Settings CategoryId is not set");

                List<ItemInfo> keywords = CoreServiceHelper.GetKeywordsByCategory(this.Clients.SessionAwareCoreServiceClient, settings.CategoryId);

                if (string.IsNullOrEmpty(settings.ApiKeyFilePath))
                    throw new Exception("Settings ApiKeyFilePath is not set");

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", settings.ApiKeyFilePath);

                // Instantiates a client
                var client = ImageAnnotatorClient.Create();

                string html = "<table class=\"usingItems results\">";

                List<ItemInfo> items = GetMultimediaCompoents("tcm:" + tcmFolder);

                foreach (ItemInfo item in items)
                {
                    if (!item.IsLocal)
                    {
                        item.TcmId = CoreServiceHelper.GetBluePrintTopTcmId(this.Clients.SessionAwareCoreServiceClient, item.TcmId);
                    }
                }

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
                    if (usedKeywords != null && usedKeywords.Any(kw => keywords.Any(x => x.TcmId.GetId() == kw.GetId())))
                    {
                        foreach (string usedKeyword in usedKeywords)
                        {
                            if (keywords.Any(x => x.TcmId.GetId() == usedKeyword.GetId()))
                            {
                                list.Add(keywords.First(x => x.TcmId.GetId() == usedKeyword.GetId()).Title);
                            }
                        }
                    }
                    else if(generate)
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

                        //add to taxonomy
                        foreach (string keywordName in list)
                        {
                            CoreServiceHelper.AddKeyword(this.Clients.SessionAwareCoreServiceClient, settings.CategoryId, keywordName);
                        }

                        //add to metadata
                        CoreServiceHelper.AppendMetadata(this.Clients.SessionAwareCoreServiceClient, item.TcmId, list);
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

        private List<ItemInfo> GetMultimediaCompoents(string tcmItem)
        {
            if (CoreServiceHelper.GetItemType(tcmItem) == ItemType.Folder)
            {
                return CoreServiceHelper.GetItemsByParentContainer(this.Clients.SessionAwareCoreServiceClient, tcmItem, true, new ComponentType[] { ComponentType.Multimedia });
            }
            if (CoreServiceHelper.GetItemType(tcmItem) == ItemType.Component)
            {
                ComponentData component = (ComponentData)CoreServiceHelper.ReadItem(this.Clients.SessionAwareCoreServiceClient, tcmItem);

                if (component.ComponentType == ComponentType.Multimedia)
                {
                    return new List<ItemInfo> { component.ToItem() };
                }
                else
                {
                    List<string> used = CoreServiceHelper.GetUsedItems(this.Clients.SessionAwareCoreServiceClient, tcmItem, new ItemType[] { ItemType.Component });
                    return used.Select(x => (ComponentData)CoreServiceHelper.ReadItem(this.Clients.SessionAwareCoreServiceClient, x)).Where(x => x.ComponentType == ComponentType.Multimedia).Select(x => x.ToItem()).ToList();
                }
            }
            return new List<ItemInfo>();
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

        [HttpGet]
        [Route("Counts/{tcmFolder}")]
        public List<string> GetCounts(string tcmFolder)
        {
            try
            {
                List<ItemInfo> topItems = CoreServiceHelper.GetItemsByParentContainer(this.Clients.SessionAwareCoreServiceClient, "tcm:" + tcmFolder, false, new ItemType[] { ItemType.Folder, ItemType.Component });

                List<string> res = new List<string>();

                foreach (ItemInfo topItem in topItems)
                {
                    if (topItem.ItemType == ItemType.Folder)
                    {
                        int count = CoreServiceHelper.GetItemsByParentContainer(this.Clients.SessionAwareCoreServiceClient, topItem.TcmId, true, new ComponentType[] { ComponentType.Multimedia }).Count;
                        if (count > 0)
                        {
                            res.Add(topItem.TcmId.Replace("tcm:", ""));
                        }
                    }
                    else
                    {
                        res.Add(topItem.TcmId.Replace("tcm:", ""));
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}