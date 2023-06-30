using Microsoft.EntityFrameworkCore;
using ProServ.Server.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProServ.ServerTests.Contexts
{
    public class UserDbContextFactory : IDbContextFactory<ProServDbContext>
    {
        private readonly DbContextOptions<ProServDbContext> _options;

        public UserDbContextFactory(DbContextOptions<ProServDbContext> options)
        {
            _options = options;
        }

        public ProServDbContext CreateDbContext()
        {
            return new ProServDbContext(_options);
        }
    }

}
