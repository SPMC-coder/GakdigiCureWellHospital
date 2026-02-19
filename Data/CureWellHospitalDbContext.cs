using Microsoft.EntityFrameworkCore;

namespace CureWellHospital.Data
{
    public class CureWellHospitalDbContext : DbContext
    {
        public CureWellHospitalDbContext(DbContextOptions<CureWellHospitalDbContext> options)
            : base(options)
        {
        }
    }
}
