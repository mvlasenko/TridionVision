using System;
using System.Collections.Generic;
using Alchemy4Tridion.Plugins.Clients.CoreService;

namespace Alchemy4Tridion.Plugins.TridionVision.Models
{
    public class ItemInfo
    {
        public string TcmId { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Path { get; set; }

        public DateTime? Modified { get; set; }

        public ItemType ItemType { get; set; }

        public string MimeType { get; set; }

        public string FromPub { get; set; }

        public bool IsPublished { get; set; }

        public string WebDav
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return this.TcmId;
                return this.Path.Trim('\\').Replace('\\', '/') + "/" + this.Title;
            }
        }

        public bool IsLocalized
        {
            get
            {
                return this.FromPub == "(Local copy)";
            }
        }
        
        public bool IsLocal
        {
            get
            {
                return string.IsNullOrEmpty(this.FromPub);
            }
        }

        public bool IsShared
        {
            get
            {
                return !string.IsNullOrEmpty(this.FromPub) && this.FromPub != "(Local copy)";
            }
        }

        public List<ItemInfo> ChildItems { get; set; }

        public ItemInfo Parent { get; set; }

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }
    }
}