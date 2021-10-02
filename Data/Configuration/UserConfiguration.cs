using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CloudPlatform.Core.Data;

namespace CloudPlatform.User.Data {
  public class UserConfiguration : EntityTypeConfiguration<Models.User> {
    public override void Configure(EntityTypeBuilder<Models.User> builder) {
      base.ConfigureEntityTable(builder);
    }
  }
}
