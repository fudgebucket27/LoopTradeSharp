using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class LoopringTradeService : ILoopringTradeService, IDisposable
    {
        const string _baseUrl = "https://api3.loopring.io";

        readonly RestClient _client;

        public LoopringTradeService()
        {
            _client = new RestClient(_baseUrl);
        }

        public async Task<StorageId> GetNextStorageId(string apiKey, int accountId, int sellTokenId)
        {
            var request = new RestRequest("api/v3/storageId");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("sellTokenId", sellTokenId);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<StorageId>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Error getting storage id: {httpException.Message}");
                return null;
            }
        }

        public async Task<string> SubmitNftTradeValidateOrder(string apiKey,NftOrder nftOrder,string eddsaSignature)
        {
            var request = new RestRequest("api/v3/nft/validateOrder");
            request.AddHeader("x-api-key", apiKey);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("exchange", nftOrder.exchange);
            request.AddParameter("accountId", nftOrder.accountId);
            request.AddParameter("storageId", nftOrder.storageId);
            request.AddParameter("sellToken.tokenId", nftOrder.sellToken.tokenId);
            request.AddParameter("sellToken.amount", nftOrder.sellToken.amount);
            request.AddParameter("buyToken.tokenId", nftOrder.buyToken.tokenId);
            request.AddParameter("buyToken.amount", nftOrder.buyToken.amount);
            request.AddParameter("validUntil", nftOrder.validUntil);
            request.AddParameter("maxFeeBips", nftOrder.maxFeeBips);
            request.AddParameter("eddsaSignature", eddsaSignature);

            try
            {
                var response = await _client.ExecutePostAsync(request);
                var data = response.Content;
                Console.WriteLine($"NFT Order Validate Response: {response.Content}");

                return data;
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Error validating nft order!: {httpException.Message}");
                return null;
            }
        }

        public async Task<string> SubmitNftTrade(string apiKey, NftTrade nftTrade, string makerEddsaSignature, string takerEddsaSignature, string apiSig)
        {
            var request = new RestRequest("api/v3/nft/trade");
            request.AddHeader("x-api-key", apiKey);
            request.AddHeader("x-api-sig", apiSig);
            request.AlwaysMultipartFormData = true;
            //Maker params
            request.AddParameter("maker.exchange", nftTrade.maker.exchange);
            request.AddParameter("maker.accountId", nftTrade.maker.accountId);
            request.AddParameter("maker.storageId", nftTrade.maker.storageId);
            request.AddParameter("maker.sellToken.tokenId", nftTrade.maker.sellToken.tokenId);
            request.AddParameter("maker.sellToken.amount", nftTrade.maker.sellToken.amount);
            request.AddParameter("maker.buyToken.tokenId", nftTrade.maker.buyToken.tokenId);
            request.AddParameter("maker.buyToken.amount", nftTrade.maker.buyToken.amount);
            request.AddParameter("maker.validUntil", nftTrade.maker.validUntil);
            request.AddParameter("maker.maxFeeBips", nftTrade.maker.maxFeeBips);
            request.AddParameter("maker.eddsaSignature", makerEddsaSignature);
            request.AddParameter("makerFeeBips", nftTrade.makerFeeBips);

            //taker params
            request.AddParameter("taker.exchange", nftTrade.taker.exchange);
            request.AddParameter("taker.accountId", nftTrade.taker.accountId);
            request.AddParameter("taker.storageId", nftTrade.taker.storageId);
            request.AddParameter("taker.sellToken.tokenId", nftTrade.taker.sellToken.tokenId);
            request.AddParameter("taker.sellToken.amount", nftTrade.taker.sellToken.amount);
            request.AddParameter("taker.buyToken.tokenId", nftTrade.taker.buyToken.tokenId);
            request.AddParameter("taker.buyToken.amount", nftTrade.taker.buyToken.amount);
            request.AddParameter("taker.validUntil", nftTrade.taker.validUntil);
            request.AddParameter("taker.maxFeeBips", nftTrade.taker.maxFeeBips);
            request.AddParameter("taker.eddsaSignature", takerEddsaSignature);
            request.AddParameter("takerFeeBips", nftTrade.takerFeeBips);

            try
            {
                var response = await _client.ExecutePostAsync(request);
                var data = response.Content;
                Console.WriteLine($"NFT Trade Response: {response.Content}");
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Error with nft trade!: {httpException.Message}");
                return null;
            }
        }
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
