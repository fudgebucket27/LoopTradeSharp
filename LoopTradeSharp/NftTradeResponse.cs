using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class BuyFilled
    {
        public int tokenId { get; set; }
        public string amount { get; set; }
        public string nftData { get; set; }
    }

    public class Fee
    {
        public int tokenId { get; set; }
        public string amount { get; set; }
    }

    public class MakerFills
    {
        public string orderHash { get; set; }
        public SellFilled sellFilled { get; set; }
        public BuyFilled buyFilled { get; set; }
        public Fee fee { get; set; }
    }

    public class SellFilled
    {
        public int tokenId { get; set; }
        public string nftData { get; set; }
        public string amount { get; set; }
    }

    public class TakerFills
    {
        public string orderHash { get; set; }
        public SellFilled sellFilled { get; set; }
        public BuyFilled buyFilled { get; set; }
        public Fee fee { get; set; }
    }


    public class NftTradeResponse
    {
        public MakerFills makerFills { get; set; }
        public TakerFills takerFills { get; set; }
        public string tradeHash { get; set; }
    }
}
