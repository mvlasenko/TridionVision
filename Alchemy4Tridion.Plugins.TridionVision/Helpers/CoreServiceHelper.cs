using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
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
        /// <param name="componentTypes">component types</param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemsByParentContainer(IAlchemyCoreServiceClient client, string tcmContainer, bool recursive, ComponentType[] componentTypes)
        {
            return client.GetListXml(tcmContainer, new OrganizationalItemItemsFilterData { Recursive = recursive, ItemTypes = new ItemType[] { ItemType.Component }, ComponentTypes = componentTypes }).ToList();
        }

        /// <summary>
        /// Gets Tridion items by parent container.
        /// </summary>
        /// <param name="client">Tridion client object.</param>
        /// <param name="tcmContainer">The TCM container.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="schemaPurposes">schema types</param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemsByParentContainer(IAlchemyCoreServiceClient client, string tcmContainer, bool recursive, SchemaPurpose[] schemaPurposes)
        {
            return client.GetListXml(tcmContainer, new OrganizationalItemItemsFilterData { Recursive = recursive, ItemTypes = new ItemType[] { ItemType.Schema }, SchemaPurposes = schemaPurposes }).ToList();
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

        public static List<ItemInfo> GetKeywordsByCategory(IAlchemyCoreServiceClient client, string tcmCategory)
        {
            return client.GetListXml(tcmCategory, new OrganizationalItemItemsFilterData { ItemTypes = new[] { ItemType.Keyword } }).ToList(ItemType.Keyword);
        }

        public static List<ItemInfo> GetContainersByPublication(IAlchemyCoreServiceClient client, string tcmPublication)
        {
            return client.GetListXml(tcmPublication, new RepositoryItemsFilterData { ItemTypes = new[] { ItemType.Folder, ItemType.StructureGroup } }).ToList();
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

        #region Multimedia

        public static List<MultimediaTypeData> GetMimeTypes(IAlchemyCoreServiceClient client, string[] mimeTypes)
        {
            List<MultimediaTypeData> allMimeTypes = client.GetSystemWideList(new MultimediaTypesFilterData()).Cast<MultimediaTypeData>().ToList();
            return allMimeTypes.Where(x => mimeTypes.Contains(x.MimeType)).ToList();
        }

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

        public static void AddKeyword(IAlchemyCoreServiceClient client, string tcmCategory, string keywordName)
        {
            List<ItemInfo> keywords = GetKeywordsByCategory(client, tcmCategory);
            if (keywords.All(x => x.Title != keywordName))
            {
                KeywordData keyword = new KeywordData
                {
                    Title = keywordName,
                    Key = keywordName,
                    Description = keywordName,
                    LocationInfo = new LocationInfo { OrganizationalItem = new LinkToOrganizationalItemData { IdRef = tcmCategory } },
                    Id = "tcm:0-0-0"
                };

                keyword = (KeywordData)client.Create(keyword, new ReadOptions());
            }
        }

        public static void AppendMetadata(IAlchemyCoreServiceClient client, string componentUri, List<string> list)
        {
            ComponentData component = ReadItem(client, componentUri) as ComponentData;

            if (component.BluePrintInfo.IsShared == true)
            {
                componentUri = GetBluePrintLocalizedTcmId(component);
                component = ReadItem(client, componentUri) as ComponentData;
            }

            SchemaData schema = ReadItem(client, component.Schema.IdRef) as SchemaData;
            XNamespace ns = schema.NamespaceUri;

            var fields = GetSchemaMetadataFields(client, component.Schema.IdRef);
            if (fields.All(x => x.Name != "Keywords"))
                throw new Exception("Keywords metadata field is not present");

            try
            {
                component = client.CheckOut(component.Id, true, new ReadOptions()) as ComponentData;
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (string.IsNullOrEmpty(component.Metadata))
                {
                    XElement metadataXml = new XElement(ns + "Metadata");

                    foreach (string keywordName in list)
                    {
                        metadataXml.Add(new XElement(ns + "Keywords", keywordName));
                    }

                    component.Metadata = metadataXml.ToString();
                }
                else
                {
                    XDocument docMetadata = XDocument.Parse(component.Metadata);
                    XElement metadataXml = docMetadata.Root;

                    foreach (string keywordName in list)
                    {
                        metadataXml.Add(new XElement(ns + "Keywords", keywordName));
                    }

                    component.Metadata = metadataXml.ToString();
                }

                client.Update(component, new ReadOptions());

                client.CheckIn(component.Id, true, "Updated by Tridion Vision Alchemy Plugin", new ReadOptions());
            }
            catch (Exception ex)
            {
                client.UndoCheckOut(componentUri, true, new ReadOptions());
            }
        }

        #endregion

        #region Tridion Blueprint

        public static string GetBluePrintTopTcmId(IAlchemyCoreServiceClient client, string id)
        {
            if (id.StartsWith("tcm:0-"))
                return id;

            var list = client.GetSystemWideList(new BluePrintFilterData { ForItem = new LinkToRepositoryLocalObjectData { IdRef = id } });
            if (list == null || list.Length == 0)
                return id;

            var list2 = list.Cast<BluePrintNodeData>().Where(x => x.Item != null).ToList();

            return list2.First().Item.Id;
        }

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