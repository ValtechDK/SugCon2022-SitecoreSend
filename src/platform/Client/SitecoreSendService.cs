using Newtonsoft.Json;
using SugCon.SitecoreSend.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SugCon.SitecoreSend.Services
{
    public interface ISendService
    {
        Task<ICollection<MooSendList>> GetLists();
    }

    public class SitecoreSendService : ISendService, IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public SitecoreSendService(string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient();
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }

        public async Task<ICollection<MooSendList>> GetLists()
        {
            var response = await _client.GetAsync($"https://api.moosend.com/v3/lists.json?apikey={_apiKey}");
            var model = await Read<MooSendResponse<MooSendListContext>>(response);
            return model?.Context?.MailingLists;
        }

        public async Task<T> Read<T>(HttpResponseMessage response)
        {
            var content = response?.Content != null ? await response.Content.ReadAsStringAsync() : null;
            if (!response.IsSuccessStatusCode || content == null)
            {
                throw new Exception("Error: " + content);
            }

            return JsonConvert.DeserializeObject<T>(content); ;
        } 
    }
}
