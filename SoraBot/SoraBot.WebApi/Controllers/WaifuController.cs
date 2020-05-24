﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SoraBot.Data.Models.SoraDb;
using SoraBot.Services.Users;
using SoraBot.Services.Waifu;
using SoraBot.WebApi.Dtos;

namespace SoraBot.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaifuController : ControllerBase
    {
        private readonly IWaifuService _waifuService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        // TODO this is quite heavy. might want to get a lighter way to fetch all waifus.
        public WaifuController(
            IWaifuService waifuService, 
            IUserService userService,
            IMapper mapper)
        {
            _waifuService = waifuService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaifuDto>>> GetAllWaifus()
        {
            var waifus = await _waifuService.GetAllWaifus().ConfigureAwait(false);
            if (waifus == null || waifus.Count == 0)
                return NotFound("No Waifus found");
            // Sort in-place by rarity descending
            waifus.Sort((x,y) => -x.Rarity.CompareTo(y.Rarity));

            var waifusToReturn = _mapper.Map<IEnumerable<WaifuDto>>(waifus);
            
            return Ok(waifusToReturn);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<UserWaifusDto>> GetAllUserWaifus(ulong userId)
        {
            var user = await _userService.GetOrSetAndGet(userId);
            if (!user)
                return NotFound("User could not be found!");

            var userWaifus = await _waifuService.GetAllUserWaifus(userId);
            if (userWaifus == null || userWaifus.Count == 0)
                return NotFound("User has no waifus");
            
            var userWaifusDto = new UserWaifusDto()
            {
                AvatarUrl = (~user).GetAvatarUrl() ?? (~user).GetDefaultAvatarUrl(),
                Username = (~user).Username,
                Waifus = new List<UserWaifuDto>(userWaifus.Count) // efficient initialization of list
            };

            // Now we get the entire waifu list for a couple of reasons.
            // We dont want to query EVERY SINGLE userwaifu bcs that's gonna be slow
            // The entire waifu list has a high probability of being in the cache
            // which would get rid of all the requests anyway, and if not we populate it
            // which is nice in it's own right :)
            // Also convert it to a dictionary for quick access later on
            var waifus = (await _waifuService.GetAllWaifus())
                .ToDictionary(x=> x.Id, x => x);

            for (var i = 0; i < userWaifus.Count; i++)
            {
                var userWaifu = userWaifus[i];
                if (!waifus.TryGetValue(userWaifu.WaifuId, out var w))
                    continue;
                userWaifusDto.Waifus.Add(new UserWaifuDto()
                {
                    Count = userWaifu.Count,
                    Id = w.Id,
                    ImageUrl = w.ImageUrl,
                    Name = w.Name,
                    Rarity = w.Rarity
                });
            }
            
            // Sort it by rarity descending
            userWaifusDto.Waifus.Sort((x,y) => -x.Rarity.CompareTo(y.Rarity));
            return Ok(userWaifusDto);
        }
    }
}