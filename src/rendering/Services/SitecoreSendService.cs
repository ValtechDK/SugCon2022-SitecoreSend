using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SugCon.SitecoreSend.Configuration;
using SugCon.SitecoreSend.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SugCon.SitecoreSend.Services
{
    public interface ISendService
    {
        Task<ICollection<MooSendCustomField>> GetListCustomFields(string listId);
        Task Subscribe(string listId, MooSendSubscriber subscriber);
    }

    public class SitecoreSendService : ISendService, IDisposable
    {
        private readonly ILogger<SitecoreSendService> _logger;
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public SitecoreSendService(ILogger<SitecoreSendService> logger, SitecoreOptions options)
        {
            _logger = logger;
            _client = new HttpClient();
            _apiKey = options.SitecoreSendApiKey;
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }

        public async Task<ICollection<MooSendCustomField>> GetListCustomFields(string listId)
        {
            var response = await _client.GetAsync($"https://api.moosend.com/v3/lists/{listId}/details.json?apikey={_apiKey}");
            var model = await Read<MooSendResponse<MooSendListDetails>>(response);
            return model?.Context?.CustomFieldsDefinition;
        }

        public async Task Subscribe(string listId, MooSendSubscriber subscriber)
        {
            var content = JsonConvert.SerializeObject(subscriber);
            var response = await _client.PostAsync($"https://api.moosend.com/v3/subscribers/{listId}/subscribe.json?apikey={_apiKey}", new StringContent(content));
            var model = await Read<MooSendResponse<MooSendSubscriber>>(response);
            
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
