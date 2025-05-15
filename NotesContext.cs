using Microsoft.EntityFrameworkCore;

public class NotesContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }
    public NotesContext(DbContextOptions<NotesContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favourite>()
            .HasKey(f => new { f.UserId, f.NoteId }); 
        
        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.UserId);
        base.OnModelCreating(modelBuilder);
    }
}