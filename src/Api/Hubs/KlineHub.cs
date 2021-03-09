using Api.Hubs;
using Api.Middleware;
using CryptoVision.Api.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoVision.Api.Hubs
{
    public class KlineHub : Hub
    {
        private readonly KlineService _klineService;
        private readonly GameService _gameService;

        public KlineHub(KlineService klineService, GameService gameService)
        {
            _klineService = klineService;
            _gameService = gameService;
        }

        public async Task<List<ResponseKlineModel>> GetKline(string symbol, string interval, long startTime, long endTime)
        {
            if (endTime - startTime > 60000000)
            {
                var end = startTime + 60000000;
                var resp = await GetByData(symbol, interval, startTime, end);
                var secondStart = end + 60000;
                var resp2 = await GetByData(symbol, interval, secondStart, endTime);
                resp.AddRange(resp2);
                return resp;
            }
            else
            {
                return await GetByData(symbol, interval, startTime, endTime);
            }
        }

        [AllowAnonymous]
        public void SubscribeKline(string symbol, string interval)
        {
            //_klineService.Subscribe(Context.ConnectionId);
        }

        public void RegisterConnection()
        {
            var acc = Context.GetHttpContext().Items["Account"] as Account;
            _gameService.EmailConnectionId.Add(acc.Email, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var acc = Context.GetHttpContext().Items["Account"] as Account;

            await Task.Run(() => _klineService.Unsubscribe(Context.ConnectionId));

            if (acc != null)
            {
                _gameService.EmailConnectionId.Remove(acc.Email);
            }
        }

        private async Task<List<ResponseKlineModel>> GetByData(string symbol, string interval, long startTime, long end)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.binance.com")
            };
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v3/klines?symbol={symbol.ToUpper()}&interval={interval}&startTime={startTime}&endTime={end}&limit=1000");
            var response = await client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<object[]>>(content);

            var set = new List<ResponseKlineModel>();

            foreach (var collection in data)
            {
                set.Add(new ResponseKlineModel
                {
                    OpenTime = Convert.ToInt64(collection[0]),
                    Open = Convert.ToDecimal(collection[1]),
                    High = Convert.ToDecimal(collection[2]),
                    Low = Convert.ToDecimal(collection[3]),
                    Close = Convert.ToDecimal(collection[4]),
                    Volume = Convert.ToDecimal(collection[5]),
                    CloseTime = Convert.ToInt64(collection[6]),
                    QuoteAssetsValue = Convert.ToDecimal(collection[7]),
                    Trades = Convert.ToInt32(collection[8]),
                    BuyBaseAsset = Convert.ToDecimal(collection[9]),
                    BuyQuoteAsset = Convert.ToDecimal(collection[10]),
                    Ignore = Convert.ToDecimal(collection[11])
                });
            }

            return set;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task Authenticate(string token)
        {
            _klineService.Authenticate(Context.ConnectionId, token);
        }
    }
}
