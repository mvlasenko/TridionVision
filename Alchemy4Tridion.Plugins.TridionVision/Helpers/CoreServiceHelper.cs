using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Alchemy4Tridion.Plugins.TridionVision.Models;
using Alchemy4Tridion.Plugins.Clients;
using Alchemy4Tridion.Plugins.Clients.CoreService;

namespace Alchemy4Tridion.Plugins.TridionVision.Helpers
{
    public static class CoreServiceHelper
    {
        #region Tridion items access

        /// <summary>
        /// Reads the item from Tridion.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="id">Tcm ID.</param>
        /// <returns></returns>
        public static IdentifiableObjectData ReadItem(IAlchemyCoreServiceClient client, string id)
        {
            try
            {
                return client.Read(id, new ReadOptions() { LoadFlags = LoadFlags.Expanded });
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Tridion list access

        /// <summary>
        /// Gets Tridion items from parent container (folder, structure group, category).
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmContainer">The TCM container.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemsByParentContainer(IAlchemyCoreServiceClient client, string tcmContainer, bool recursive)
        {
            return client.GetListXml(tcmContainer, new OrganizationalItemItemsFilterData { Recursive = recursive }).ToList();
        }

        /// <summary>
        /// Gets Tridion items by parent container.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmContainer">The TCM container.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="itemTypes">Item types.</param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemsByParentContainer(IAlchemyCoreServiceClient client, string tcmContainer, bool recursive, ItemType[] itemTypes)
        {
            return client.GetListXml(tcmContainer, new OrganizationalItemItemsFilterData { Recursive = recursive, ItemTypes = itemTypes }).ToList();
        }

        /// <summary>
        /// Gets Tridion items by parent container.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmContainer">The TCM container.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="itemTypes">Item types.</param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemsByParentContainer(IAlchemyCoreServiceClient client, string tcmContainer, bool recursive, ComponentType[] componentTypes)
        {
            return client.GetListXml(tcmContainer, new OrganizationalItemItemsFilterData { Recursive = recursive, ItemTypes = new ItemType[] { ItemType.Component }, ComponentTypes = componentTypes }).ToList();
        }

        /// <summary>
        /// Gets the web dav of Tridion item.
        /// </summary>
        /// <param name="item">Tridion item.</param>
        /// <returns></returns>
        public static string GetWebDav(this RepositoryLocalObjectData item)
        {
            if (item.LocationInfo == null || string.IsNullOrEmpty(item.LocationInfo.WebDavUrl))
                return string.Empty;

            string webDav = HttpUtility.UrlDecode(item.LocationInfo.WebDavUrl.Replace("/webdav/", string.Empty));
            if (string.IsNullOrEmpty(webDav))
                return string.Empty;

            int dotIndex = webDav.LastIndexOf(".", StringComparison.Ordinal);
            int slashIndex = webDav.LastIndexOf("/", StringComparison.Ordinal);

            return dotIndex >= 0 && dotIndex > slashIndex ? webDav.Substring(0, dotIndex) : webDav;
        }

        /// <summary>
        /// Gets using items of source item. All revisions discovered
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmItem">Item TCM id.</param>
        /// <param name="current">if set to <c>true</c> [current].</param>
        /// <param name="itemTypes">The item types.</param>
        /// <returns></returns>
        public static List<string> GetUsingItems(IAlchemyCoreServiceClient client, string tcmItem, bool current = false, ItemType[] itemTypes = null)
        {
            UsingItemsFilterData filter = new UsingItemsFilterData();
            filter.IncludedVersions = current ? VersionCondition.OnlyLatestVersions : VersionCondition.AllVersions;
            filter.BaseColumns = ListBaseColumns.Id;
            if (itemTypes != null)
                filter.ItemTypes = itemTypes;

            List<string> items = client.GetListXml(tcmItem, filter).ToList().Select(x => x.TcmId).ToList();
            return items;
        }

        /// <summary>
        /// Gets the using items of source item. Only current revision discovered
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmItem">Item TCM id.</param>
        /// <returns></returns>
        public static List<string> GetUsingCurrentItems(IAlchemyCoreServiceClient client, string tcmItem, ItemType[] itemTypes = null)
        {
            return GetUsingItems(client, tcmItem, true, itemTypes);
        }

        /// <summary>
        /// Gets the used items of source item.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmItem">Item TCM id.</param>
        /// <param name="itemTypes">The item types.</param>
        /// <returns></returns>
        public static List<string> GetUsedItems(IAlchemyCoreServiceClient client, string tcmItem, ItemType[] itemTypes = null)
        {
            UsedItemsFilterData filter = new UsedItemsFilterData();
            filter.BaseColumns = ListBaseColumns.Id;
            if (itemTypes != null)
                filter.ItemTypes = itemTypes;

            List<string> items = client.GetListXml(tcmItem, filter).ToList().Select(x => x.TcmId).ToList();
            return items;
        }

        #endregion

        #region Tridion schemas

        /// <summary>
        /// Gets the Tridion schema fields.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmSchema">The TCM schema.</param>
        /// <returns></returns>
        public static List<ItemFieldDefinitionData> GetSchemaFields(IAlchemyCoreServiceClient client, string tcmSchema)
        {
            SchemaFieldsData schemaFieldsData = client.ReadSchemaFields(tcmSchema, true, null);
            if (schemaFieldsData == null || schemaFieldsData.Fields == null)
                return null;

            return schemaFieldsData.Fields.ToList();
        }

        /// <summary>
        /// Gets the metadata schema fields.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmSchema">The TCM schema.</param>
        /// <returns></returns>
        public static List<ItemFieldDefinitionData> GetSchemaMetadataFields(IAlchemyCoreServiceClient client, string tcmSchema)
        {
            SchemaFieldsData schemaFieldsData = client.ReadSchemaFields(tcmSchema, true, null);
            if (schemaFieldsData == null || schemaFieldsData.MetadataFields == null)
                return null;

            return schemaFieldsData.MetadataFields.ToList();
        }

        #endregion

        #region Tridion components

        /// <summary>
        /// Gets xml element by xPath.
        /// </summary>
        /// <param name="root">Xml root object.</param>
        /// <param name="xPath">XPath</param>
        /// <param name="ns">Xml namespase</param>
        /// <returns></returns>
        public static XElement GetByXPath(this XElement root, string xPath, XNamespace ns)
        {
            if (root == null || string.IsNullOrEmpty(xPath))
                return null;

            xPath = xPath.Trim('/');
            if (string.IsNullOrEmpty(xPath))
                return null;

            if (xPath.Contains("/"))
            {
                xPath = "/xhtml:" + xPath.Replace("/", "/xhtml:");
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("xhtml", ns.ToString());
                return root.XPathSelectElement(xPath, namespaceManager);
            }

            return root.Element(ns + xPath);
        }

        /// <summary>
        /// Gets the list or xml elements by xPath.
        /// </summary>
        /// <param name="root">Xml root object.</param>
        /// <param name="xPath">The x path.</param>
        /// <param name="ns">Xml namespase</param>
        /// <returns></returns>
        public static List<XElement> GetListByXPath(this XElement root, string xPath, XNamespace ns)
        {
            if (root == null || string.IsNullOrEmpty(xPath))
                return new List<XElement>();

            xPath = xPath.Trim('/');

            if (string.IsNullOrEmpty(xPath))
                return null;

            if (xPath.Contains("/"))
            {
                xPath = "/xhtml:" + xPath.Replace("/", "/xhtml:");
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("xhtml", ns.ToString());
                return root.XPathSelectElements(xPath, namespaceManager).ToList();
            }

            return root.Elements(ns + xPath).ToList();
        }

        /// <summary>
        /// Gets the list or xml elements by xPath.
        /// </summary>
        /// <param name="elements">Xml elements.</param>
        /// <param name="xPath">xPath.</param>
        /// <param name="ns">Xml namespase</param>
        /// <returns></returns>
        public static List<XElement> GetListByXPath(this List<XElement> elements, string xPath, XNamespace ns)
        {
            return elements.SelectMany(x => x.GetListByXPath(xPath, ns)).ToList();
        }

        #endregion

        #region Multimedia

        public static byte[] GetBinaryFromMultimediaComponent(IAlchemyStreamDownload client, ComponentData multimediaComponent)
        {
            byte[] binaryContent = null;
            using (Stream tempStream = client.DownloadBinaryContent(multimediaComponent.Id))
            {
                if (multimediaComponent.BinaryContent.FileSize != -1)
                {
                    var memoryStream = new MemoryStream();
                    tempStream.CopyTo(memoryStream);
                    binaryContent = memoryStream.ToArray();
                }
            }
            return binaryContent;
        }

        #endregion

        #region Tridion publishing

        /// <summary>
        /// Gets the published items.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="id">item tcm id.</param>
        /// <returns></returns>
        public static List<string> GetPublishedTargets(IAlchemyCoreServiceClient client, string id)
        {
            return client.GetListPublishInfo(id).Select(publishData => publishData.PublicationTarget.IdRef).Distinct().ToList();
        }

        /// <summary>
        /// Gets publishing targets.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <returns></returns>
        public static string[] GetTargets(IAlchemyCoreServiceClient client)
        {
            return client.GetSystemWideList(new PublicationTargetsFilterData()).Select(x => x.Id).ToArray();
        }

        #endregion

        #region Tridion Blueprint

        /// <summary>
        /// Gets tcm id of first localized or original item up the blue print.
        /// </summary>
        /// <param name="dataItem">Tridion item.</param>
        /// <returns></returns>
        public static string GetBluePrintLocalizedTcmId(RepositoryLocalObjectData dataItem)
        {
            if (dataItem.BluePrintInfo.OwningRepository == null)
                return dataItem.Id;

            string localizedPublication = dataItem.BluePrintInfo.OwningRepository.IdRef;
            return TextHelper.GetBluePrintItemTcmId(dataItem.Id, localizedPublication);
        }

        #endregion

        #region Collection helpers

        /// <summary>
        /// Converts XML retrieved from Core Service ItemInfo list.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns></returns>
        public static List<ItemInfo> ToList(this XElement xml, ItemType itemType)
        {
            List<ItemInfo> res = new List<ItemInfo>();
            if (xml != null && xml.HasElements)
            {
                foreach (XElement element in xml.Elements())
                {
                    ItemInfo item = new ItemInfo();
                    item.TcmId = element.Attribute("ID").Value;
                    item.ItemType = itemType;
                    item.Title = element.Attributes().Any(x => x.Name == "Title") ? element.Attribute("Title").Value : item.TcmId;
                    item.Icon = element.Attributes().Any(x => x.Name == "Icon") ? element.Attribute("Icon").Value : "T16L0P0";
                    item.Path = element.Attributes().Any(x => x.Name == "Path") ? element.Attribute("Path").Value : string.Empty;
                    item.Modified = element.Attributes().Any(x => x.Name == "Modified") ? DateTime.Parse(element.Attribute("Modified").Value) : (DateTime?)null;

                    if (item.ItemType == ItemType.Component)
                    {
                        item.MimeType = element.Attributes().Any(x => x.Name == "MIMEType") ? element.Attribute("MIMEType").Value : null;
                    }
                    
                    item.FromPub = element.Attributes().Any(x => x.Name == "FromPub") ? element.Attribute("FromPub").Value : null;
                    item.IsPublished = element.Attributes().Any(x => x.Name == "Icon") && element.Attribute("Icon").Value.EndsWith("P1");
                    
                    res.Add(item);
                }
            }
            return res;
        }

        /// <summary>
        /// Converts XML retrieved from Core Service ItemInfo list.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static List<ItemInfo> ToList(this XElement xml)
        {
            List<ItemInfo> res = new List<ItemInfo>();
            if (xml != null && xml.HasElements)
            {
                foreach (XElement element in xml.Elements())
                {
                    ItemInfo item = new ItemInfo();
                    item.TcmId = element.Attribute("ID").Value;
                    item.ItemType = element.Attributes().Any(x => x.Name == "Type") ? (ItemType)int.Parse(element.Attribute("Type").Value) : GetItemType(item.TcmId);
                    item.Title = element.Attributes().Any(x => x.Name == "Title") ? element.Attribute("Title").Value : item.TcmId;
                    item.Icon = element.Attributes().Any(x => x.Name == "Icon") ? element.Attribute("Icon").Value : "T16L0P0";
                    item.Path = element.Attributes().Any(x => x.Name == "Path") ? element.Attribute("Path").Value : string.Empty;
                    item.Modified = element.Attributes().Any(x => x.Name == "Modified") ? DateTime.Parse(element.Attribute("Modified").Value) : (DateTime?)null;

                    if (item.ItemType == ItemType.Component)
                    {
                        item.MimeType = element.Attributes().Any(x => x.Name == "MIMEType") ? element.Attribute("MIMEType").Value : null;    
                    }

                    item.FromPub = element.Attributes().Any(x => x.Name == "FromPub") ? element.Attribute("FromPub").Value : null;
                    item.IsPublished = element.Attributes().Any(x => x.Name == "Icon") && element.Attribute("Icon").Value.EndsWith("P1");
                    
                    res.Add(item);
                }
            }
            return res;
        }

        /// <summary>
        /// To the item.
        /// </summary>
        /// <param name="dataItem">Tridion item.</param>
        /// <returns></returns>
        public static ItemInfo ToItem(this RepositoryLocalObjectData dataItem)
        {
            ItemInfo item = new ItemInfo();
            item.TcmId = dataItem.Id;
            item.ItemType = GetItemType(dataItem.Id);
            item.Title = dataItem.Title;

            string webDav = dataItem.GetWebDav();
            item.Path = string.IsNullOrEmpty(webDav) ? string.Empty : webDav.Substring(0, webDav.LastIndexOf('/')).Replace('/', '\\');

            item.Modified = dataItem.VersionInfo != null ? dataItem.VersionInfo.RevisionDate : null;

            if (dataItem.IsPublishedInContext != null)
                item.IsPublished = dataItem.IsPublishedInContext.Value;

            item.Icon = "T" + (int)item.ItemType + "L0P" + (item.IsPublished ? "1" : "0");

            if (item.ItemType == ItemType.Component)
            {
                ComponentData componentDataItem = (ComponentData)dataItem;
                if (componentDataItem.ComponentType == ComponentType.Multimedia)
                {
                    item.MimeType = componentDataItem.BinaryContent.MimeType;

                    if (componentDataItem.BinaryContent.Filename != null)
                    {
                        string ext = Path.GetExtension(componentDataItem.BinaryContent.Filename);
                        item.Icon += "M" + ext.Trim('.');
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the type of Tridion item.
        /// </summary>
        /// <param name="tcmItem">Item TCM id.</param>
        /// <returns></returns>
        public static ItemType GetItemType(string tcmItem)
        {
            if (string.IsNullOrEmpty(tcmItem))
                return ItemType.None;

            string[] arr = tcmItem.Split('-');
            if (arr.Length == 2 || arr.Length == 3 && arr[2].StartsWith("v")) return ItemType.Component;

            return (ItemType)int.Parse(arr[2]);
        }

        /// <summary>
        /// Determines whether this instance is multimedia.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if the specified field is multimedia; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMultimedia(this ItemFieldDefinitionData field)
        {
            return field is MultimediaLinkFieldDefinitionData;
        }

        /// <summary>
        /// Determines whether this instance is embedded.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if the specified field is embedded; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmbedded(this ItemFieldDefinitionData field)
        {
            return field is EmbeddedSchemaFieldDefinitionData;
        }

        /// <summary>
        /// Determines whether [is component link].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [is component link] [the specified field]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsComponentLink(this ItemFieldDefinitionData field)
        {
            return field is ComponentLinkFieldDefinitionData;
        }

        /// <summary>
        /// Determines whether [is multimedia component link].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [is multimedia component link] [the specified field]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMultimediaComponentLink(this ItemFieldDefinitionData field)
        {
            ComponentLinkFieldDefinitionData clField = field as ComponentLinkFieldDefinitionData;
            if (clField == null)
                return false;
            return clField.AllowMultimediaLinks;
        }

        /// <summary>
        /// Determines whether [is multi value].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [is multi value] [the specified field]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMultiValue(this ItemFieldDefinitionData field)
        {
            return field.MaxOccurs == -1 || field.MaxOccurs > 1;
        }

        /// <summary>
        /// Determines whether [is mandatory].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [is multi value] [the specified field]; otherwise, <c>false</c>.
        /// </returns>

        public static bool IsMandatory(this ItemFieldDefinitionData field)
        {
            return field.MinOccurs == 1;
        }

        #endregion

    }
}