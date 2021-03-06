﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ArgonautCore.Maybe;
using SoraBot.Data.Models.SoraDb;
using WaifuDbo = SoraBot.Data.Models.SoraDb.Waifu;

namespace SoraBot.Services.Waifu
{
    public interface IWaifuService
    {
        Task<List<WaifuDbo>> GetAllWaifus();
        Task<WaifuDbo> GetRandomWaifu();
        Task<bool> TryGiveWaifusToUser(ulong userid, List<WaifuDbo> waifus, uint boxCost);
        Task<WaifuDbo> GetRandomSpecialWaifu(ulong userId, WaifuRarity specialRarity);
        Task<List<WaifuDbo>> GetAllWaifusFromUser(ulong userId);
        Task<List<UserWaifu>> GetAllUserWaifus(ulong userId);

        Task<Dictionary<WaifuRarity, int>> GetTotalWaifuRarityStats();
        Task<Maybe<(uint waifusSold, uint coinAmount)>> SellDupes(ulong userId);

        Task<WaifuDbo> GetWaifuByName(string name);
        Task<WaifuDbo> GetWaifuById(int id);
        Task<Maybe<uint>> TrySellWaifu(ulong userId, int waifuId, uint amount, WaifuRarity? rarity = null);
        Task<UserWaifu> GetUserWaifu(ulong userid, int waifuId);
        Task<bool> SetUserFavWaifu(ulong userId, int waifuId);
        Task RemoveUserFavWaifu(ulong userId);
        Task<bool> TryTradeWaifus(ulong offerUser, ulong wantUser, int offerWaifuId, int requestWaifuId);
        Task RemoveWaifu(int waifuId);
    }
}