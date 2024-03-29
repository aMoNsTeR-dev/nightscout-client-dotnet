﻿using NightscoutClientDotNet.Interfaces;
using NightscoutClientDotNet.Models;
using NightscoutClientDotNet.Models.Enums;
using RestSharp;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NightscoutClientDotNet
{
    public class NightscoutRestApiV1Client : INightscoutRestApiClient
    {
        private const string _API_PATH = "/api/v1/";
        private readonly string _baseAddress;
        private readonly ApiKeyType _authenticationType;
        private readonly string _apiKey;
        private readonly RestClient _client;


        public NightscoutRestApiV1Client(string baseAddress) : this(baseAddress, ApiKeyType.None, string.Empty) { }

        public NightscoutRestApiV1Client(string baseAddress, ApiKeyType authenticationType, string apiKey)
        {
            _baseAddress = baseAddress;
            _authenticationType = authenticationType;
            _apiKey = apiKey;

            _client= new RestClient(new RestClientOptions { BaseUrl = new System.Uri($"{baseAddress}{_API_PATH}"), RemoteCertificateValidationCallback = RemoteCertificateValidationCallback});
        }


        public async Task<IEnumerable<Entry>> GetEntriesAsync(string query, int count)
        {
            string countString = count >= 0 ? $"{(!string.IsNullOrEmpty(query) ? '&'.ToString() : string.Empty)}count={count}" : string.Empty;
            string queryString = string.Empty;
            if (!string.IsNullOrEmpty(query) || count >= 0) 
            {
                queryString = $"?{query}{countString}";
            }

            RestResponse<IEnumerable<Entry>> response = await _client.ExecuteAsync<IEnumerable<Entry>>(this.BuildRequest($"entries{queryString}", Method.Get));
            return (response.IsSuccessful && response.Data != null) ? response.Data : new List<Entry>();
        }

        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private RestRequest BuildRequest(string resource, RestSharp.Method method, object? body = null)
        {
            if (_authenticationType == ApiKeyType.TokenInUrl) 
            {
                resource = $"{resource}{(resource.Contains('?') ? '&' : '?')}token={_apiKey}";
            }

            RestRequest request = new RestRequest(resource, method);

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            switch (_authenticationType)
            {
                case ApiKeyType.ApiSecret:
                    request.AddHeader("api-secret", _apiKey);
                    break;

                case ApiKeyType.JwToken:
                    request.AddHeader("Authorization", $"Bearer {_apiKey}");
                    break;

                case ApiKeyType.TokenInUrl:
                case ApiKeyType.None:
                default:
                    break;
            }

            return request;
        }
    }
}
