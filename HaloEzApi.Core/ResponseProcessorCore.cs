using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HaloEzAPI.Caching;
using HaloEzAPI.Limits;
using HaloEzAPI.Model.Response.Error;
using Newtonsoft.Json;

namespace HaloEzAPI
{
    public class ResponseProcessorCore
    {
        public ResponseProcessorCore()
        {
        }

        /// <summary>
        /// Handle the HttpResponse, by throwing a known exception, or by deserializing into T
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="message">The HttpResponseMessage</param>
        /// <returns></returns>
        public async Task<T> HandleResponse<T>(HttpResponseMessage message) where T : class
        {
            if (message.IsSuccessStatusCode)
            {
                var messageJson = await message.Content.ReadAsStringAsync();
                var messageObject = JsonConvert.DeserializeObject<T>(messageJson);
                if (messageObject == null)
                {
                    throw new HaloAPIException(CommonErrorMessages.CantDeserialize);
                }
                return messageObject;
            }
            BaseHandleResponse(message);
            throw new HaloAPIException(string.Format("Unknown Error in HandleResponse: {0}", message.RequestMessage));
        }

        protected void BaseHandleResponse(HttpResponseMessage message)
        {
            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HaloAPIException(message.ReasonPhrase);
            }
            if (message.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HaloAPIException(message.ReasonPhrase);
            }
            if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HaloAPIException(CommonErrorMessages.AccessDenied);
            }
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HaloAPIException(CommonErrorMessages.BadRequest);   
            }
            if (message.StatusCode == (HttpStatusCode) 429)
            {
                throw new HaloAPIException(CommonErrorMessages.TooManyRequests);
            }
        }

        public async Task<T> ProcessRequest<T>(Uri endpoint,int cacheExpiryMin) where T : class
        {
            string key = endpoint.AbsoluteUri;
            if (CacheManager.Contains(key))
            {
                return CacheManager.Get<T>(key);
            }
            var message = await RequestRateHttpClient.GetRequest(endpoint);
            var messageObject = await HandleResponse<T>(message);
            CacheManager.Add<T>(messageObject, key, cacheExpiryMin);
            return messageObject;
        }
    }
}