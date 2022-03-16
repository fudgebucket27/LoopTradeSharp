
using LoopTradeSharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PoseidonSharp;
using System.Numerics;

#region Initial Setup
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

int tokenId = 33032; //tokenId to trade
string nftData = "0x013a4a886f3de035bd82666c043abc90d1ce1b353463b32f3aa3a20aca21ec59"; //nftData to trade

#endregion

#region Get storage id
ILoopringTradeService loopringTradeService = new LoopringTradeService();
//Getting the storage id
var storageId = await loopringTradeService.GetNextStorageId(settings.LoopringApiKey, settings.LoopringAccountId, 1);
Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");
#endregion

#region Create order, calculate poseidon hash and submit order validation
NftOrder nftOrder = new NftOrder()
{
    exchange = settings.Exchange,
    accountId = settings.LoopringAccountId,
    storageId = storageId.orderId,
    sellToken = new SellToken
    {
        tokenId = tokenId,
        nftData = nftData,
        amount = "1"
    },
    buyToken = new BuyToken
    {
        tokenId = 1,
        amount = "10000000000000",
        nftData = nftData
    },
    allOrNone = false,
    fillAmountBOrS = false,
    validUntil = 1700000000,
    maxFeeBips = 80
};

int fillAmountBOrSValue = 0;
if(nftOrder.fillAmountBOrS == true)
{
    fillAmountBOrSValue = 1;
}

BigInteger[] poseidonInputs =
{
    Utils.ParseHexUnsigned(settings.Exchange),
    (BigInteger) nftOrder.storageId,
    (BigInteger) nftOrder.accountId,
    (BigInteger) nftOrder.sellToken.tokenId,
    !String.IsNullOrEmpty(nftOrder.buyToken.nftData) ? Utils.ParseHexUnsigned(nftOrder.buyToken.nftData) : (BigInteger) nftOrder.buyToken.tokenId ,
    !String.IsNullOrEmpty(nftOrder.sellToken.amount) ? BigInteger.Parse(nftOrder.sellToken.amount) : (BigInteger) 0,
    !String.IsNullOrEmpty(nftOrder.buyToken.amount) ? BigInteger.Parse(nftOrder.buyToken.amount) : (BigInteger) 0,
    (BigInteger) nftOrder.validUntil,
    (BigInteger) nftOrder.maxFeeBips,
    (BigInteger) fillAmountBOrSValue,
    Utils.ParseHexUnsigned("0x0000000000000000000000000000000000000000")
};

//Generate the poseidon hash
Poseidon poseidon = new Poseidon(12, 6, 53, "poseidon", 5, _securityTarget: 128);
BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);

//Generate the poseidon eddsa signature
Eddsa eddsa = new Eddsa(poseidonHash, settings.LoopringPrivateKey);
string eddsaSignature = eddsa.Sign();

var nftTradeValidateResponse = await loopringTradeService.SubmitNftTradeValidateOrder(settings.LoopringApiKey, nftOrder, eddsaSignature);

#endregion