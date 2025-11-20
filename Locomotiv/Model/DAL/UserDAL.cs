using System.Collections.Generic;
using System.Linq;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using Locomotiv.Model;

namespace Locomotiv.Model.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly ApplicationDbContext _context;

        public UserDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        public User GetUserWithStation(int id)
        {
            using var context = new ApplicationDbContext();

            return context.Users
                .Include(u => u.Station)
                    .ThenInclude(s => s.Trains)
                .Include(u => u.Station)
                    .ThenInclude(s => s.Voies)
                .Include(u => u.Station)
                    .ThenInclude(s => s.Signaux)
                .FirstOrDefault(u => u.Id == id);
        }

        public User? FindByUsernameAndPassword(string u, string p)
        {
            return _context.Users
                .Include(user => user.Station)
                .FirstOrDefault(u2 => u2.Username == u && u2.Password == p);
        }

    }
}
