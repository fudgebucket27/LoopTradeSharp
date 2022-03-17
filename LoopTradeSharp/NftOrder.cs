﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class NftOrder
    {
        public string exchange { get; set; }
        public int accountId { get; set; }
        public int storageId { get; set; }
        public bool allOrNone { get; set; }
        public bool fillAmountBOrS { get; set; }
        public long validUntil { get; set; }
        public int maxFeeBips { get; set; }
        public string eddsaSignature { get; set; }
        public string clientOrderId { get; set; } = "";
        public string orderType { get; set; } = "";
        public string tradeChannel { get; set; } = "";
        public string taker { get; set; } = "";
        public string affiliate { get; set; } = "";
        public SellToken sellToken { get; set; }
        public BuyToken buyToken { get; set; }
    }

    public class SellToken
    {
        public int tokenId { get; set; }
        public string nftData { get; set; }
        public string amount { get; set; }
    }

    public class BuyToken
    {
        public int tokenId { get; set; }
        public string amount { get; set; }
        public string nftData { get; set; }
    }
}
