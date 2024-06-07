using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PosterRepository : GenericRepository<Poster>, IPosterRepository
    {
        public PosterRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
