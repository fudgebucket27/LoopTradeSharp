﻿
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

int tokenId2 = 32794; //tokenId to trade
string nftData2 = "0x00fed22012e524652211a522836e2e872066da7405192321088615bc116550ef"; //nftData to trade

#endregion

#region Get storage ids
ILoopringTradeService loopringTradeService = new LoopringTradeService();
//Getting the storage id
var storageId = await loopringTradeService.GetNextStorageId(settings.LoopringApiKey, settings.LoopringAccountId, 1);
var storageId2 = await loopringTradeService.GetNextStorageId(settings.LoopringApiKey2, settings.LoopringAccountId2, 1);
Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");
Console.WriteLine($"Storage id2: {JsonConvert.SerializeObject(storageId2, Formatting.Indented)}");
#endregion

#region Create maker order, calculate poseidon hash and submit order validation
NftOrder nftMakerOrder = new NftOrder()
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
        tokenId = tokenId2,
        nftData = nftData2,
        amount = "1"
    },
    allOrNone = false,
    fillAmountBOrS = false,
    validUntil = 1700000000,
    maxFeeBips = 80
};

int fillAmountBOrSValue = 0;
if(nftMakerOrder.fillAmountBOrS == true)
{
    fillAmountBOrSValue = 1;
}

BigInteger[] poseidonMakerOrderInputs =
{
    Utils.ParseHexUnsigned(settings.Exchange),
    (BigInteger) nftMakerOrder.storageId,
    (BigInteger) nftMakerOrder.accountId,
    (BigInteger) nftMakerOrder.sellToken.tokenId,
    !String.IsNullOrEmpty(nftMakerOrder.buyToken.nftData) ? Utils.ParseHexUnsigned(nftMakerOrder.buyToken.nftData) : (BigInteger) nftMakerOrder.buyToken.tokenId ,
    !String.IsNullOrEmpty(nftMakerOrder.sellToken.amount) ? BigInteger.Parse(nftMakerOrder.sellToken.amount) : (BigInteger) 0,
    !String.IsNullOrEmpty(nftMakerOrder.buyToken.amount) ? BigInteger.Parse(nftMakerOrder.buyToken.amount) : (BigInteger) 0,
    (BigInteger) nftMakerOrder.validUntil,
    (BigInteger) nftMakerOrder.maxFeeBips,
    (BigInteger) fillAmountBOrSValue,
    Utils.ParseHexUnsigned("0x0000000000000000000000000000000000000000")
};

//Generate the poseidon hash
Poseidon poseidon = new Poseidon(12, 6, 53, "poseidon", 5, _securityTarget: 128);
BigInteger makeOrderPoseidonHash = poseidon.CalculatePoseidonHash(poseidonMakerOrderInputs);

//Generate the poseidon eddsa signature
Eddsa eddsa = new Eddsa(makeOrderPoseidonHash, settings.LoopringPrivateKey);
string makerEddsaSignature = eddsa.Sign();

var nftMakerTradeValidateResponse = await loopringTradeService.SubmitNftTradeValidateOrder(settings.LoopringApiKey, nftMakerOrder, makerEddsaSignature);
#endregion

#region Create taker order, calculate poseidon hash and submit order validation
NftOrder nftTakerOrder = new NftOrder()
{
    exchange = settings.Exchange,
    accountId = settings.LoopringAccountId2,
    storageId = storageId2.orderId,
    sellToken = new SellToken
    {
        tokenId = tokenId2,
        nftData = nftData2,
        amount = "1"
    },
    buyToken = new BuyToken
    {
        tokenId = tokenId,
        nftData = nftData,
        amount = "1"
    },
    allOrNone = false,
    fillAmountBOrS = true,
    validUntil = 1700000000,
    maxFeeBips = 80
};

int fillAmountBOrSValue2 = 0;
if (nftTakerOrder.fillAmountBOrS == true)
{
    fillAmountBOrSValue2 = 1;
}

BigInteger[] poseidonTakerOrderInputs =
{
    Utils.ParseHexUnsigned(settings.Exchange),
    (BigInteger) nftTakerOrder.storageId,
    (BigInteger) nftTakerOrder.accountId,
    (BigInteger) nftTakerOrder.sellToken.tokenId,
    !String.IsNullOrEmpty(nftTakerOrder.buyToken.nftData) ? Utils.ParseHexUnsigned(nftTakerOrder.buyToken.nftData) : (BigInteger) nftTakerOrder.buyToken.tokenId ,
    !String.IsNullOrEmpty(nftTakerOrder.sellToken.amount) ? BigInteger.Parse(nftTakerOrder.sellToken.amount) : (BigInteger) 0,
    !String.IsNullOrEmpty(nftTakerOrder.buyToken.amount) ? BigInteger.Parse(nftTakerOrder.buyToken.amount) : (BigInteger) 0,
    (BigInteger) nftTakerOrder.validUntil,
    (BigInteger) nftTakerOrder.maxFeeBips,
    (BigInteger) fillAmountBOrSValue2,
    Utils.ParseHexUnsigned("0x0000000000000000000000000000000000000000")
};

//Generate the poseidon hash
Poseidon poseidon2 = new Poseidon(12, 6, 53, "poseidon", 5, _securityTarget: 128);
BigInteger takerOrderPoseidonHash = poseidon2.CalculatePoseidonHash(poseidonTakerOrderInputs);

//Generate the poseidon eddsa signature
Eddsa eddsa2 = new Eddsa(takerOrderPoseidonHash, settings.LoopringPrivateKey);
string takerEddsaSignature = eddsa2.Sign();

var nftTakerTradeValidateResponse = await loopringTradeService.SubmitNftTradeValidateOrder(settings.LoopringApiKey2, nftTakerOrder, takerEddsaSignature);
#endregion

#region Create trade, calculate api sig value and submit trade
NftTrade nftTrade = new NftTrade
{
    maker = nftMakerOrder,
    makerFeeBips = 80,
    taker = nftTakerOrder,
    takerFeeBips = 80
};

//Calculate api sig value
Dictionary<string, object> dataToSig = new Dictionary<string, object>();
dataToSig.Add("maker", nftTrade.maker);
dataToSig.Add("makerFeeBips", nftTrade.makerFeeBips);
dataToSig.Add("taker", nftTrade.taker);
dataToSig.Add("takerFeeBips", nftTrade.takerFeeBips);
var signatureBase = "POST&";
var parameterString = JsonConvert.SerializeObject(dataToSig);
signatureBase += Utils.UrlEncodeUpperCase("https://api3.loopring.io/" + "api/v3/nft/trade") + "&";
signatureBase += Utils.UrlEncodeUpperCase(parameterString);
var sha256Number = SHA256Helper.CalculateSHA256HashNumber(signatureBase);

var sha256Signer = new Eddsa(sha256Number, settings.LoopringPrivateKey);
var sha256Signed = sha256Signer.Sign();

var nftTradeResponse = await loopringTradeService.SubmitNftTrade(settings.LoopringApiKey, nftTrade, makerEddsaSignature, takerEddsaSignature, sha256Signed);

#endregion