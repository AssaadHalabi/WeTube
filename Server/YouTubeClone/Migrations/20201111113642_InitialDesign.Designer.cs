// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YouTubeClone.Data;

namespace YouTubeClone.Migrations
{
    [DbContext(typeof(YouTubeContext))]
    [Migration("20201111113642_InitialDesign")]
    partial class InitialDesign
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("YouTubeClone.Models.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Channel");
                });

            modelBuilder.Entity("YouTubeClone.Models.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("ChannelId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("YouTubeClone.Models.PlaylistVideo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("PlaylistId")
                        .HasColumnType("int");

                    b.Property<int?>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlaylistId");

                    b.HasIndex("VideoId");

                    b.ToTable("PlaylistVideo");
                });

            modelBuilder.Entity("YouTubeClone.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("ChannelId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashedPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Secret")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserChannelSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("ChannelId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("UserChannelSubscription");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserCommentReaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CommentId")
                        .HasColumnType("int");

                    b.Property<bool>("Like")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCommentReaction");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserCommentReply", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CommentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCommentReply");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("VideoId");

                    b.ToTable("UserVideoComment");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoReaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Like")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("VideoId");

                    b.ToTable("UserVideoReaction");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("VideoId");

                    b.ToTable("UserVideoReport");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("VideoId");

                    b.ToTable("UserVideoViews");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoWatchLater", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("VideoId");

                    b.ToTable("UserVideoWatchLater");
                });

            modelBuilder.Entity("YouTubeClone.Models.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Featured")
                        .HasColumnType("bit");

                    b.Property<bool>("Shown")
                        .HasColumnType("bit");

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.Playlist", b =>
                {
                    b.HasOne("YouTubeClone.Models.Channel", "Channel")
                        .WithMany("Playlists")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("YouTubeClone.Models.PlaylistVideo", b =>
                {
                    b.HasOne("YouTubeClone.Models.Playlist", "Playlist")
                        .WithMany("Videos")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("YouTubeClone.Models.Video", "Video")
                        .WithMany("Playlists")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Playlist");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.User", b =>
                {
                    b.HasOne("YouTubeClone.Models.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserChannelSubscription", b =>
                {
                    b.HasOne("YouTubeClone.Models.Channel", "Channel")
                        .WithMany("Subscribers")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Channel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserCommentReaction", b =>
                {
                    b.HasOne("YouTubeClone.Models.UserVideoComment", "Comment")
                        .WithMany("UserCommentReactions")
                        .HasForeignKey("CommentId");

                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("UserCommentReactions")
                        .HasForeignKey("UserId");

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserCommentReply", b =>
                {
                    b.HasOne("YouTubeClone.Models.UserVideoComment", "Comment")
                        .WithMany("UserCommentReplies")
                        .HasForeignKey("CommentId");

                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("UserCommentReplies")
                        .HasForeignKey("UserId");

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoComment", b =>
                {
                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("UserVideoComments")
                        .HasForeignKey("UserId");

                    b.HasOne("YouTubeClone.Models.Video", "Video")
                        .WithMany("UserVideoComments")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoReaction", b =>
                {
                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("UserVideoReactions")
                        .HasForeignKey("UserId");

                    b.HasOne("YouTubeClone.Models.Video", "Video")
                        .WithMany("UserVideoReactions")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoReport", b =>
                {
                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("UserVideoReports")
                        .HasForeignKey("UserId");

                    b.HasOne("YouTubeClone.Models.Video", "Video")
                        .WithMany("UserVideoReports")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoView", b =>
                {
                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("UserVideoViews")
                        .HasForeignKey("UserId");

                    b.HasOne("YouTubeClone.Models.Video", "Video")
                        .WithMany("UserVideoViews")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoWatchLater", b =>
                {
                    b.HasOne("YouTubeClone.Models.User", "User")
                        .WithMany("WatchLater")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("YouTubeClone.Models.Video", "Video")
                        .WithMany("UserVideoWatchLater")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeClone.Models.Video", b =>
                {
                    b.HasOne("YouTubeClone.Models.Channel", "Author")
                        .WithMany("Videos")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Author");
                });

            modelBuilder.Entity("YouTubeClone.Models.Channel", b =>
                {
                    b.Navigation("Playlists");

                    b.Navigation("Subscribers");

                    b.Navigation("Videos");
                });

            modelBuilder.Entity("YouTubeClone.Models.Playlist", b =>
                {
                    b.Navigation("Videos");
                });

            modelBuilder.Entity("YouTubeClone.Models.User", b =>
                {
                    b.Navigation("Subscriptions");

                    b.Navigation("UserCommentReactions");

                    b.Navigation("UserCommentReplies");

                    b.Navigation("UserVideoComments");

                    b.Navigation("UserVideoReactions");

                    b.Navigation("UserVideoReports");

                    b.Navigation("UserVideoViews");

                    b.Navigation("WatchLater");
                });

            modelBuilder.Entity("YouTubeClone.Models.UserVideoComment", b =>
                {
                    b.Navigation("UserCommentReactions");

                    b.Navigation("UserCommentReplies");
                });

            modelBuilder.Entity("YouTubeClone.Models.Video", b =>
                {
                    b.Navigation("Playlists");

                    b.Navigation("UserVideoComments");

                    b.Navigation("UserVideoReactions");

                    b.Navigation("UserVideoReports");

                    b.Navigation("UserVideoViews");

                    b.Navigation("UserVideoWatchLater");
                });
#pragma warning restore 612, 618
        }
    }
}
