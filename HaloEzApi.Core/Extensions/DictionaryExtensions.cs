using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HaloExApi.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static Uri BuildUri(this Dictionary<string,string> dict, string root)
        {
            var query = "";
            for (int i = 0; i < dict.Keys.Count; i++)
            {
                if (i != 0)
                {
                    query += "&";
                }

                var key = dict.Keys.ElementAt(i);
                var value = dict[key];
                if (!string.IsNullOrEmpty(value))
                {
                    query += key + "&" + value;
                }
            }

            var builder = new UriBuilder(root) { Query = query };
            return builder.Uri;
        }
    }
}
