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
    public class CatalogyRepository : GenericRepository<Category>, ICatalogyRepository
    {
        public CatalogyRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
