﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTubeClone.Data;
using YouTubeClone.Models;
using YouTubeClone.Models.Dtos;
using YouTubeClone.Services;
using YouTubeClone.Settings;

namespace YouTubeClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly YouTubeContext context;
        private readonly HashingSettings settings;
        private readonly IMapper mapper;

        public IdentityController(YouTubeContext context, HashingSettings settings, IMapper mapper)
        {
            this.context = context;
            this.settings = settings;
            this.mapper = mapper;
        }

        public class SigninDto
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        public class SignupDto
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }
        }

        /// <summary>
        /// Signin
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/identity/signin
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "Username": "",
        ///             "Password": ""
        ///         }
        ///     }
        /// 
        /// </remarks>
        [HttpPost("signin")]
        public async Task<ActionResult<UserDto>> Signin(SigninDto _user)
        {
            var hashedPassword = _user.Password.HashPassword(settings.Salt);

            var user = await context.User
                .Include(u => u.Channel)
                .Include(u => u.Subscriptions).ThenInclude(s => s.Channel)
                .FirstOrDefaultAsync(u => u.Username == _user.Username && u.HashedPassword == hashedPassword);

            if (user == null)
            {
                return NotFound();
            }

            return mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Signup
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/identity/signup
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "FirstName": "",
        ///             "LastName": "",
        ///             "Username": "",
        ///             "Password": ""
        ///         }
        ///     }
        /// 
        /// </remarks>
        [HttpPost("signup")]
        public async Task<ActionResult<UserDto>> Signup(SignupDto _user)
        {
            var user = await context.User
                .FirstOrDefaultAsync(u => u.Username == _user.Username);

            if (user != null)
            {
                return BadRequest("Username is already taken.");
            }

            var hashedPassword = _user.Password.HashPassword(settings.Salt);

            user = new User
            {
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Username = _user.Username,
                HashedPassword = hashedPassword,
                Secret = Guid.NewGuid()
            };

            await context.User.AddAsync(user);
            await context.SaveChangesAsync();

            return mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Get the user's WatchLater videos
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/identity/watch-later?userId=0&userSecret=null
        /// 
        /// </remarks>
        [HttpGet("watch-later")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetUserWatchLaterVideos([FromQuery] int userId, [FromQuery] string userSecret)
        {
            var user = await context.User
                .Include(u => u.WatchLater)
                    .ThenInclude(uv => uv.Video)
                    .ThenInclude(v => v.Author)
                .Include(u => u.WatchLater)
                    .ThenInclude(uv => uv.Video)
                    .ThenInclude(v => v.UserVideoComments)
                .Include(u => u.WatchLater)
                    .ThenInclude(uv => uv.Video)
                    .ThenInclude(v => v.UserVideoViews)
                .Include(u => u.WatchLater)
                    .ThenInclude(uv => uv.Video)
                    .ThenInclude(v => v.UserVideoReactions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            user.WatchLater.ForEach(uv => uv.Video.Author.Name = GetChannelName(uv.Video));

            if (user == null)
            {
                return NotFound();
            }

            if (user.Secret != Guid.Parse(userSecret))
            {
                return Unauthorized();
            }

            var videos = user.WatchLater.Select(uv => mapper.Map<VideoDto>(uv.Video)).ToList();
            return videos;
        }

        /// <summary>
        /// Get the user's history
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/identity/history?userId=0&userSecret=null
        /// 
        /// </remarks>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<VideoSummaryDto>>> GetUserHistoryVideos([FromQuery] int userId, [FromQuery] string userSecret)
        {
            var user = await context.User
                .Include(u => u.UserVideoViews).ThenInclude(vv => vv.Video).ThenInclude(v => v.Author)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Secret != Guid.Parse(userSecret))
            {
                return Unauthorized();
            }

            var videos = user.UserVideoViews
                .OrderByDescending(vv => vv.DateTime)
                .Select(vv => mapper.Map<VideoSummaryDto>(vv.Video))
                .Distinct()
                .Take(10)
                .ToList();

            videos.ForEach(v => v.Author.Name = GetChannelName(v.Author.Id));

            //var userViewsVideoIds = await context.UserVideoViews
            //    .Where(vv => vv.User.Id == userId)
            //    .OrderByDescending(vv => vv.DateTime)
            //    .Take(10)
            //    .Select(vv => vv.Video.Id)
            //    .ToListAsync();

            //var videos = await context.Video
            //    .Where(v => userViewsVideoIds.Contains(v.Id))
            //    .Include(v => v.Author)
            //    .ToListAsync();

            //videos.ForEach(v => v.Author.Name = GetChannelName(v));

            //var result = videos
            //    .Select(v => mapper.Map<VideoDto>(v))
            //    .ToList();

            return videos;
        }

        private string GetChannelName(Video v)
        {
            var user = context.User.FirstOrDefault(u => u.Channel.Id == v.Author.Id);

            return user.FirstName + " " + user.LastName;
        }

        private string GetChannelName(int channelId)
        {
            var user = context.User.FirstOrDefault(u => u.Channel.Id == channelId);

            return user.FirstName + " " + user.LastName;
        }
    }
}
