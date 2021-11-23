using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Test
{
    public class CategoriaUnitTestController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }
        public static string connectioString = "DataSource=:memory:";

        static CategoriaUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectioString)
                .EnableSensitiveDataLogging()
                .Options;
        }

        public CategoriaUnitTestController()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            db.Seed(context);

            _repository = new UnitOfWork(context);
        }
    }
}