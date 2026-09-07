using Microsoft.EntityFrameworkCore;
using GreenTime.Models;


namespace GreenTime.Context;

public class GreenTimeDbContext : DbContext
{
    public GreenTimeDbContext(DbContextOptions<GreenTimeDbContext> options)
    : base(options)
    {

    }

    //DbSet'ler 
    public DbSet<User> Users { get; set; }
    public DbSet<CuratedPlaylist> CurratedPlaylists { get; set; }
    public DbSet<CuratedVideo> CuratedVideos { get; set; }
}