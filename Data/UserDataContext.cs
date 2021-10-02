using Microsoft.EntityFrameworkCore;

using CloudPlatform.Core.Data;

namespace CloudPlatform.User.Data {

  public class UserDataContext : DataContext<UserDataContext> {
    public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

    public DbSet<Models.User> Users { get; set; }
  }
}
