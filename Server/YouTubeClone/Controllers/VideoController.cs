﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using YouTubeClone.Data;
using YouTubeClone.Models;
using YouTubeClone.Models.Dtos;
using YouTubeClone.Services;

namespace YouTubeClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly YouTubeContext context;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;

        public VideoController(YouTubeContext context, IMapper mapper, IWebHostEnvironment env)
        {
            this.context = context;
            this.mapper = mapper;
            this.env = env;
        }

        public class PostVideoDto
        {
            public IFormCollection Video { get; set; }
            public IFormCollection Thubmnail { get; set; }
            public int UserId { get; set; }
            public string UserSecret { get; set; }
            public string Text { get; set; }
            public int PlaylistId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool Shown { get; set; }
            public bool Featured { get; set; }
        }

        /// <summary>
        /// Get a channel's videos
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/video/channel/3
        /// 
        /// </remarks>
        [HttpGet("channel/{channelId}")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetChannelVideos(int channelId)
        {
            var videos = await context.Video
                .Where(v => v.Author.Id == channelId)
                .Select(v => mapper.Map<VideoDto>(v))
                .ToListAsync();
                
            return videos;
        }

        /// <summary>
        /// Get list of videos by keyword search
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/video/search?q=hello+world?p=2
        /// 
        /// p is optional and defaults to 1
        /// </remarks>      
        [HttpGet("search")]
        public async Task<ActionResult> GetVideosFromSearch([FromQuery] string q, [FromQuery] int p = 1)
        {
            var keywords = q.ToLower().Split();

            var regexBody = keywords.Aggregate((current, next) => current + "|" + next);
            var regex = new Regex("(" + regexBody + ")");

            var videos = await context.Video
                .Where(v => regex.IsMatch(v.Title.ToLower()))
                .Select(v => mapper.Map<VideoDto>(v))
                .ToListAsync();

            var page = videos.Skip((p - 1) * 10)
                .Take(10);
                
            return Ok(new {
                PagesCount = Math.Ceiling(videos.Count / 10.0),
                Videos = page
            });
        }

        /// <summary>
        /// Get a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/video/4
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// 
        /// UserId and UserSecrets are OPTIONAL.
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<VideoDto>> GetVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var videoById = await context.Video
                .Include(v => v.Author)
                .Include(v => v.UserVideoComments)
                .Include(v => v.UserVideoReactions)
                .Include(v => v.UserVideoViews)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (videoById == null)
            {
                return NotFound();
            }

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId && u.Secret == Guid.Parse(postVideoDto.UserSecret));

            context.UserVideoViews.Add(new UserVideoView { User = user, Video = videoById, DateTime = DateTime.Now });
            await context.SaveChangesAsync();

            return mapper.Map<VideoDto>(videoById);
        }

        [HttpGet("stream/{id}")]
        public async Task<ActionResult> GetVideoStream(int id)
        {
            var videoById = await context.Video
                .FirstOrDefaultAsync(v => v.Id == id);

            if (videoById == null)
            {
                return NotFound();
            }

            FileStream content = System.IO.File.OpenRead(videoById.Url);
            var response = File(content, "application/octet-stream");
            return response;
        }

        [HttpGet("image-stream/{id}")]
        public async Task<ActionResult> GetImageStream(int id)
        {
            var videoById = await context.Video
                .FirstOrDefaultAsync(v => v.Id == id);

            if (videoById == null)
            {
                return NotFound();
            }

            FileStream content = System.IO.File.OpenRead(videoById.ThumbnailUrl);
            var response = File(content, "application/octet-stream");
            return response;
        }

        /// <summary>
        /// Upload a video (without its details)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/video/upload?userId=0&userSecret=secret, video.mp4, image.jpg
        ///     {
        ///         "Content-Type": "multipart/form-data"
        ///     }
        /// </remarks>
        [HttpPost("upload")]
        public async Task<ActionResult<VideoDto>> PostVideo([FromQuery] int userId, [FromQuery] string userSecret, IFormCollection files)
        {
            var user = await context.User
                .Include(u => u.Channel)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Secret != Guid.Parse(userSecret))
            {
                return Unauthorized();
            }

            if (files.Files.Count < 2)
            {
                return BadRequest("At least two files should be attached.");
            }

            string imagePath = null;
            string videoPath = null;
            for (int i = 0; i < 2; i++)
            {
                if (FileIsImage(files.Files.ElementAt(i)))
                {
                    imagePath = await HelperFunctions.AddFileToSystemAsync(files.Files.ElementAt(i), env);
                }
                else
                {
                    videoPath = await HelperFunctions.AddFileToSystemAsync(files.Files.ElementAt(i), env);
                }
            }

            var video = new Video { 
                Author = user.Channel, 
                ThumbnailUrl = imagePath,
                Url = videoPath,
                UploadDate = DateTime.Now
            };
            await context.Video.AddAsync(video);
            await context.SaveChangesAsync();

            return mapper.Map<VideoDto>(video);
        }

        /// <summary>
        /// Update a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/video/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": "",
        ///             "Title": "",
        ///             "Description": "",
        ///             "Featured": false,
        ///             "Shown": false,
        ///         }
        ///     }
        /// </remarks>
        [HttpPut]
        public async Task<ActionResult<VideoDto>> UpdateVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User
                .Include(u => u.Channel)
                .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var originalVideo = await context.Video
                .Include(v => v.Author)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (originalVideo.Author != user.Channel)
            {
                return Unauthorized();
            }

            originalVideo
                .SetTitle(postVideoDto.Title)
                .SetDescription(postVideoDto.Description)
                .SetFeatured(postVideoDto.Featured)
                .SetShown(postVideoDto.Shown);

            await context.SaveChangesAsync();

            return mapper.Map<VideoDto>(postVideoDto.Video);
        }

        /// <summary>
        /// Delete a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/video/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpDelete]
        public async Task<ActionResult> DeleteVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User
                .Include(u => u.Channel)
                .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video
                .Include(v => v.Author)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (video.Author != user.Channel)
            {
                return Unauthorized();
            }

            context.Video.Remove(video);
            await context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Show a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/video/show/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPut("show")]
        public async Task<ActionResult> ShowVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User
                .Include(u => u.Channel)
                .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video
                .Include(v => v.Author)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (video.Author != user.Channel)
            {
                return Unauthorized();
            }

            video.Shown = true;
            await context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Hide a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/video/hide/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPut("hide")]
        public async Task<ActionResult> HideVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User
                .Include(u => u.Channel)
                .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video
                .Include(v => v.Author)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (video.Author != user.Channel)
            {
                return Unauthorized();
            }

            video.Shown = false;
            await context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Like a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/video/like/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPut("like")]
        public async Task<ActionResult> LikeVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User.FindAsync(postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var userVideoReaction = await context.UserVideoReaction
                .Include(uvr => uvr.User)
                .Include(uvr => uvr.Video)
                .FirstOrDefaultAsync(uvr => uvr.User == user && uvr.Video == video);

            if (userVideoReaction == null)
            {
                userVideoReaction = new UserVideoReaction { User = user, Video = video, Like = true };
            }
            else
            {
                userVideoReaction.Like = true;
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Undo like a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/video/undolike/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPost("undolike")]
        public async Task<ActionResult> UndoLikeVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User.FindAsync(postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var userVideoReaction = await context.UserVideoReaction
                .Include(uvr => uvr.User)
                .Include(uvr => uvr.Video)
                .FirstOrDefaultAsync(uvr => uvr.User == user && uvr.Video == video && uvr.Like);

            if (userVideoReaction != null)
            {
                context.UserVideoReaction.Remove(userVideoReaction);
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Dislike a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/video/dislike/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPut("dislike")]
        public async Task<ActionResult> DislikeVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User.FindAsync(postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var userVideoReaction = await context.UserVideoReaction
                .Include(uvr => uvr.User)
                .Include(uvr => uvr.Video)
                .FirstOrDefaultAsync(uvr => uvr.User == user && uvr.Video == video);

            if (userVideoReaction == null)
            {
                userVideoReaction = new UserVideoReaction { User = user, Video = video, Like = false };
            }
            else
            {
                userVideoReaction.Like = false;
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Undo dislike a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/video/undodislike/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPost("undodislike")]
        public async Task<ActionResult> UndoDislikeVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User.FindAsync(postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var userVideoReaction = await context.UserVideoReaction
                .Include(uvr => uvr.User)
                .Include(uvr => uvr.Video)
                .FirstOrDefaultAsync(uvr => uvr.User == user && uvr.Video == video && !uvr.Like);

            if (userVideoReaction != null)
            {
                context.UserVideoReaction.Remove(userVideoReaction);
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Add a video to WatchLater
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/video/addtowatchlater/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPost("addtowatchlater")]
        public async Task<ActionResult> AddToWatchLater(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User
                .Include(u => u.WatchLater)
                .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var videoFromWatchLater = user.WatchLater.FirstOrDefault(uv => uv.Video == video);

            if (videoFromWatchLater == null)
            {
                await context.UserVideoWatchLater.AddAsync(new UserVideoWatchLater { User = user, Video = video });
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Report a video
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/video/reportvideo/2
        ///     {
        ///         "Content-Type": "application/json",
        ///         "body": {
        ///             "UserId": 0,
        ///             "UserSecret": "",
        ///             "Text": ""
        ///         }
        ///     }
        /// </remarks>
        [HttpPost("reportvideo")]
        public async Task<ActionResult> ReportVideo(int id, [FromBody] PostVideoDto postVideoDto)
        {
            var user = await context.User
               .Include(u => u.UserVideoReports)
               .FirstOrDefaultAsync(u => u.Id == postVideoDto.UserId);

            if (user == null || user.Secret != Guid.Parse(postVideoDto.UserSecret))
            {
                return Unauthorized();
            }

            var video = await context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var prevVideoReport = user.UserVideoReports.FirstOrDefault(uv => uv.Video == video);

            if (prevVideoReport == null)
            {
                await context.UserVideoReport.AddAsync(new UserVideoReport { User = user, Video = video, Reason = postVideoDto.Text });
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Get a list of recommended videos
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/videos/recommendation?channelId=1
        /// 
        /// </remarks>
        /// <param name="channelId">OPIONAL. Case of null, latest videos across channels are returned.</param>
        [HttpGet("recommendation")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetRecommendedVideos([FromQuery] int? channelId)
        {
            if (channelId == null)
            {
                var videos = await context.Video
                    .Include(v => v.Author)
                    .Take(10)
                    .Select(v => mapper.Map<VideoDto>(v))
                    .ToListAsync();

                return videos;
            }
            else
            {
                var videos = await context.Video
                      .Include(v => v.Author)
                      .Where(v => v.Author.Id == channelId)
                      .Take(10)
                      .Select(v => mapper.Map<VideoDto>(v))
                      .ToListAsync();

                if (videos.Count != 10)
                {
                    var extraVideos = await context.Video
                        .Include(v => v.Author)
                        .Take(10 - videos.Count)
                        .Select(v => mapper.Map<VideoDto>(v))
                        .ToListAsync();

                    videos.AddRange(extraVideos);

                }

                return videos;
            }
        }

        private bool FileIsImage(IFormFile file) => file.ContentType.ToLower().IndexOf("image") != -1;
    }
}
