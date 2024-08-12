using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Onyx.DbContext
{
    public class UserIdentitiesContext : IdentityDbContext
    {
        public UserIdentitiesContext(DbContextOptions<UserIdentitiesContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Creating default roles for users
            var readerId = "a9d874e2-a943-4ce9-a921-49a4f0a6da1d";
            var writerId = "98084f71-61ed-4aec-a6d6-09b99263866f";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerId,
                    ConcurrencyStamp = readerId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = writerId,
                    ConcurrencyStamp = writerId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
