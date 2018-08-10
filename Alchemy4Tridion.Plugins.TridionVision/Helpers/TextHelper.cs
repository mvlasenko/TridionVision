using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Alchemy4Tridion.Plugins.Clients.CoreService;

namespace Alchemy4Tridion.Plugins.TridionVision.Helpers
{
    public static class TextHelper
    {
        /// <summary>
        /// Gets the the middle part of tcm id.
        /// </summary>
        /// <param name="tcmId">The TCM.</param>
        /// <returns></returns>
        public static string GetId(this string tcmId)
        {
            if (string.IsNullOrEmpty(tcmId) || !tcmId.StartsWith("tcm:") || !tcmId.Contains("-"))
                return string.Empty;

            return tcmId.Split('-')[1];
        }

        /// <summary>
        /// Gets the publication TCM id from item tcm id.
        /// </summary>
        /// <param name="id">TCM id.</param>
        /// <returns></returns>
        public static string GetPublicationTcmId(string id)
        {
            ItemType itemType = CoreServiceHelper.GetItemType(id);
            if (itemType == ItemType.Publication)
                return id;

            return "tcm:0-" + id.Replace("tcm:", string.Empty).Split('-')[0] + "-1";
        }

        /// <summary>
        /// Gets the TCM id of item in sperific publication up or down the plue print.
        /// </summary>
        /// <param name="id">Tcm id.</param>
        /// <param name="publicationId">The publication tcm id.</param>
        /// <returns></returns>
        public static string GetBluePrintItemTcmId(string id, string publicationId)
        {
            if (string.IsNullOrEmpty(id) || !id.StartsWith("tcm:") || !id.Contains("-") || string.IsNullOrEmpty(publicationId) || !publicationId.StartsWith("tcm:") || !publicationId.Contains("-"))
                return string.Empty;

            return "tcm:" + publicationId.GetId() + "-" + id.GetId() + (id.Split('-').Length > 2 ? "-" + id.Split('-')[2] : string.Empty);
        }

        /// <summary>
        /// Strips the HTML fromtrinf.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns></returns>
        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Converts string to nullable date.
        /// </summary>
        /// <param name="strDate">The string date.</param>
        /// <returns></returns>
        public static DateTime? ToDate(this string strDate)
        {
            try
            {
                return DateTime.ParseExact(strDate, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
