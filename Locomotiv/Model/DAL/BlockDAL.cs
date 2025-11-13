using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class BlockDAL : IBlockDAL
    {
        private readonly ApplicationDbContext _context;

        public BlockDAL(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
