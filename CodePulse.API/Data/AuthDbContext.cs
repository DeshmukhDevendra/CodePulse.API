using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "df954f53-bd68-48b2-b7d8-e962c08cc9bc";
            var writerRoleId = "6bc2baac-966f-4e2d-8127-dd465408befe";

            //create reader and writer role
            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id= readerRoleId,
                    Name="Reader",
                    NormalizedName="Reader".ToUpper(),
                    ConcurrencyStamp = readerRoleId
                },
                new IdentityRole() 
                {
                    Id= writerRoleId,
                    Name="Writer",
                    NormalizedName="Writer".ToUpper(),
                    ConcurrencyStamp = writerRoleId
                }
            };
            
            //Seed the roles
            builder.Entity<IdentityRole>().HasData(roles);

            //Create admin user
            var adminUserId = "e6ae1d1f-c79c-4e05-b8d1-d069ffd92b4e";
            var admin = new IdentityUser()
            {
                Id = adminUserId,
                UserName = "admin@IndexOutOfBound.com",
                Email = "admin@IndexOutOfBound.com",
                NormalizedEmail = "admin@IndexOutOfBound.com".ToUpper(),
                NormalizedUserName = "admin@IndexOutOfBound.com".ToUpper()
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@1234");

            builder.Entity<IdentityUser>().HasData(admin);

            //Give roles to admin

            var adminRoles = new List<IdentityUserRole<string>>() 
            {
                new ()
                {
                    UserId = adminUserId,
                    RoleId = readerRoleId
                },
                new ()
                {
                    UserId= adminUserId,
                    RoleId= writerRoleId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }
    }
}
