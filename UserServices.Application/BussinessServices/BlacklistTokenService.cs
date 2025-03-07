﻿using Microsoft.Extensions.Caching.Distributed;

namespace UserServices.Application.BussinessServices
{
    public class BlacklistTokenService
    {
        private readonly IDistributedCache _cache;

        public BlacklistTokenService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task AddToBlacklistAsync(string token, TimeSpan expiry)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };

            await _cache.SetStringAsync(token, "blacklisted", options);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            var result = await _cache.GetStringAsync(token);
            return result == "blacklisted";
        }
    }
}
