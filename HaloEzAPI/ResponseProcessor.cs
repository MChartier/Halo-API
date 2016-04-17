using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using HaloEzAPI.Caching;
using HaloEzAPI.Limits;
using HaloEzAPI.Model.Response.Error;

namespace HaloEzAPI
{
    public class ResponseProcessor : ResponseProcessorCore
    {
        public ResponseProcessor()
        {
        }

        private async Task<Image> HandleImageResponse(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                Image image;
                using (var stream = await message.Content.ReadAsStreamAsync())
                {
                    image = Image.FromStream(stream);
                }
                return image;
            }
            BaseHandleResponse(message);
            throw new HaloAPIException(string.Format("Unknown Error in HandleImageResponse: {0}", message.RequestMessage));
        }

        public async Task<Image> ProcessImageRequest(Uri endpoint, int cacheExpiryMin)
        {
            string key = endpoint.AbsoluteUri;
            if (CacheManager.Contains(key))
            {
                return CacheManager.Get<Image>(key);
            }
            var message = await RequestRateHttpClient.GetRequest(endpoint);
            var messageObject = await HandleImageResponse(message);
            CacheManager.Add<Image>(messageObject, key, cacheExpiryMin);
            return messageObject;
        }
    }
}