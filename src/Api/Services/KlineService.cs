using Api.Hubs;
using CryptoVision.Api.Hubs;
using CryptoVision.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;

namespace CryptoVision.Api.Services
{
    public class KlineService
    {
        private readonly ConcurrentDictionary<string, List<string>> NumberOfSubscribed = new ConcurrentDictionary<string, List<string>>();
        private readonly ConcurrentDictionary<string, WebSocket> WebSockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly object _lock = new object();
        private const string BaseUrl = "wss://stream.binance.com:9443";

        private GameService gameService;

        public Dictionary<string, string> Auths { get; set; }

        private readonly IHubContext<KlineHub> _hubContext;

        public KlineService(IHubContext<KlineHub> hubContext, GameService gameService)
        {
            _hubContext = hubContext;
            this.gameService = gameService;
            Subscribe(Guid.NewGuid().ToString());
        }

        public void Authenticate(string callerId, string token)
        {
            Auths.Add(callerId, token);
        }

        public void Subscribe(string callerId)
        {
            var key = "btcusdt";
            lock(_lock)
            {
                if (!WebSockets.ContainsKey(key))
                {
                    try
                    {
                        WebSockets.TryAdd(key, new WebSocket($"{BaseUrl}/ws/btcusdt@kline_1m"));
                        WebSockets[key].SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12;
                        WebSockets[key].OnOpen += Opened;
                        WebSockets[key].OnMessage += (sender, e) => MessageReceived(e);
                        WebSockets[key].OnError += ErrorMsg;
                        WebSockets[key].Connect();
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
                if (NumberOfSubscribed.ContainsKey(key))
                {
                    NumberOfSubscribed[key].Add(callerId);
                }
                else
                {
                    NumberOfSubscribed.TryAdd(key, new List<string> { callerId });
                }
            }
        }

        public void Unsubscribe(string callerId)
        {
            if(NumberOfSubscribed.Values.Any(x => x.Contains(callerId)))
            {
                var subsription = NumberOfSubscribed.Where(x => x.Value.Contains(callerId)).FirstOrDefault();
                NumberOfSubscribed[subsription.Key].Remove(callerId);
                if(NumberOfSubscribed[subsription.Key].Count == 0)
                {
                    WebSockets[subsription.Key].OnMessage -= (sender, e) => MessageReceived(e);
                    WebSockets[subsription.Key].Close();
                    WebSockets.TryRemove(subsription.Key, out _);
                }
            }
        }

        private void ErrorMsg(object sender, ErrorEventArgs e)
        {
            
        }

        private void Opened(object sender, EventArgs e)
        {
            
        }

        private void MessageReceived(MessageEventArgs e)
        {
            var parsed = JObject.Parse(e.Data);
            var item = parsed["k"];
            var data = new ResponseKlineStreamModel
            {
                EventType = parsed["e"].ToString(),
                EventTime = Convert.ToInt64(parsed["E"]),
                Symbol = parsed["s"].ToString(),
                KlineItems = new KlineItems
                {
                    KlineStartTime = Convert.ToInt64(item["t"]),
                    KlineCloseTime = Convert.ToInt64(item["T"]),
                    Symbol = item["s"].ToString(),
                    Interval = item["i"].ToString(),
                    FirstTradeId = Convert.ToInt32(item["f"]),
                    LastTradeId = Convert.ToInt32(item["L"]),
                    OpenPrice = Convert.ToDecimal(item["o"]),
                    ClosePrice = Convert.ToDecimal(item["c"]),
                    HighPrice = Convert.ToDecimal(item["h"]),
                    LowPrice = Convert.ToDecimal(item["l"]),
                    BaseAssetVolume = Convert.ToDecimal(item["v"]),
                    NumberOfTrades = Convert.ToInt32(item["n"]),
                    IsThisKlineClosed = Convert.ToBoolean(item["x"]),
                    QuoteAssetsVolume = Convert.ToDecimal(item["q"]),
                    TakerBuyBaseAssetVolume = Convert.ToDecimal(item["V"]),
                    TakerBuyQuoteAssetVolume = Convert.ToDecimal(item["Q"]),
                    Ignore = item["B"].ToString()
                }
            };

            gameService.PriceUpdated(data);

            _hubContext.Clients.All.SendAsync($"btcusdt_1m_Get", new GeneralMessage<ResponseKlineStreamModel> { Symbol = "btcusdt", Interval = "1m", Message = data });
            // MessageEmitter.Invoke(this, new GeneralMessage { Symbol = symbol, Interval = interval, Message = e.Data });
        }
    }
}
