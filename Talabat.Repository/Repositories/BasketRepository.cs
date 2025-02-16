﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;

namespace Talabat.Repository.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database; 

        public BasketRepository(IConnectionMultiplexer redis   )
        {
            _database = redis.GetDatabase();
        }



        public async Task<bool> DeleteBasket(string BasketId)
        {
           return await _database.KeyDeleteAsync( BasketId );
        }






        public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
        {
            var basket = await _database.StringGetAsync( BasketId );
            return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }







        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var createdorupdated = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(20));
            if(createdorupdated is false) return null;

            return await GetBasketAsync(basket.Id);
        }
    }
}
