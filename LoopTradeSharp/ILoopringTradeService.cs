using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public interface ILoopringTradeService
    {
        Task<StorageId> GetNextStorageId(string apiKey, int accountId, int sellTokenId);
        Task<string> SubmitNftTradeValidateOrder(string apiKey, NftOrder nftOrder);
        Task<string> SubmitNftTrade(string apiKey, NftTrade nftrade, string apiSig);
    }
}
