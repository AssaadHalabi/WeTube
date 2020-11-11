﻿using System.Collections.Generic;

namespace YouTubeClone.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        public List<PlaylistVideo> Videos { get; set; }

        public string Name { get; set; }

        public Channel Channel { get; set; }
    }
}
