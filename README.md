# LoopTradeSharp
NFT Trading via the Loopring API in C#. This is more of a demo on how to make a valid trade between two accounts. You will need three seperate Loopring Accounts to use this demo. You will need a maker, taker and settler account.

You need an appsettings.json file in the directory with the setting "Copy to Output Directory" set to "Copy Always" like below. 

```json
{

  "Settings": {
    "LoopringApiKey": "kdlblahaha", //The maker api key
    "LoopringPrivateKey": "0xbalahha", //The maker private key
    "LoopringAddress": "0xblahaha", //The maker address
    "LoopringAccountId": 40940, //The maker account id

    "LoopringApiKey2": "0Vtblahaha", //The taker api key
    "LoopringPrivateKey2": "0xblahaha", //The taker private key
    "LoopringAddress2": "0xblahblah", //The taker address
    "LoopringAccountId2": 77900, //The taker account id

    "LoopringApiKey3": "B2pblahblah", //The settler api key
    "LoopringPrivateKey3": "0x36blah", //The settler private key
    "LoopringAddress3": "0x99nblaha", //The settler address
    "LoopringAccountId3": 136736, //The settler account id
    
    "Exchange": "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4" //Loopring Exchange address
  }
}
```

Edit the nftTokenId and nftData in the code to the specific NFT in the maker's account.
