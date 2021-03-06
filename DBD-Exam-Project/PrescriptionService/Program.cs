using System.Text.Json.Serialization;
using AutoMapper;
using lib.Configurations;
using lib.DTO;
using lib.Models;
using PrescriptionService.Data;
using PrescriptionService.Data.Repositories;
using PrescriptionService.Data.Storage;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisConfig = builder.Configuration.GetSection(RedisCacheConfig.ConfigKey);
RedisCacheConfig settings = redisConfig.Get<RedisCacheConfig>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(settings.EndPoints));

builder.Services.Configure<RedisCacheConfig>(redisConfig);

builder.Services.AddScoped<IRedisCache, RedisCache>();
builder.Services.AddScoped<IPrescriptionStorage, PrescriptionStorage>();
builder.Services.AddScoped<IPatientStorage, PatientStorage>();
builder.Services.AddScoped<IDoctorStorage, DoctorStorage>();
builder.Services.AddScoped<IPharmaceutStorage, PharmaceutStorage>();
builder.Services.AddScoped<IMedicineStorage, MedicineStorage>();
builder.Services.AddScoped<IPharmacyStorage, PharmacyStorage>();

builder.Services.AddNpgsql<PostgresContext>(builder.Configuration.GetConnectionString("postgres"));
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAsyncRepository<Doctor>, DoctorRepository>();
builder.Services.AddScoped<IAsyncRepository<Pharmaceut>, PharmaceutRepository>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<IAsyncRepository<Pharmacy>, PharmacyRepository>();

builder.Services.AddSingleton<IMapper>(
    new Mapper(
        new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Prescription, PrescriptionDto>()
                    .ForMember(dist => dist.Patient, opt => opt.MapFrom(src => src.PrescribedToNavigation))
                    .ForMember(dist => dist.Doctor, opt => opt.MapFrom(src => src.PrescribedByNavigation))
                    .ForMember(dist => dist.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                    .ForPath(dist => dist.Patient.Id, opt => opt.MapFrom(src => src.PrescribedTo))
                    .ForPath(dist => dist.Doctor.Id, opt => opt.MapFrom(src => src.PrescribedBy))
                    .ForMember(dist => dist.IsFulfilled, opt => opt.MapFrom(src => src.LastAdministeredBy != null))
                    .ReverseMap();
                cfg.CreateMap<Patient, PersonDto>()
                    .ForMember(dist => dist.Type, opt => opt.MapFrom(src => PersonType.Patient))
                    .ForMember(dist => dist.FirstName, opt => opt.MapFrom(src => src.PersonalData.FirstName))
                    .ForMember(dist => dist.LastName, opt => opt.MapFrom(src => src.PersonalData.LastName))
                    .ForMember(dist => dist.Email, opt => opt.MapFrom(src => src.PersonalData.Email))
                    .ForMember(dist => dist.CprNumber, opt => opt.MapFrom(src => src.Cpr))
                    .ForMember(dist => dist.Address, opt => opt.MapFrom(src => src.PersonalData.Address))
                    .ReverseMap();
                cfg.CreateMap<Doctor, PersonDto>()
                    .ForMember(dist => dist.Type, opt => opt.MapFrom(src => PersonType.Doctor))
                    .ForMember(dist => dist.FirstName, opt => opt.MapFrom(src => src.PersonalData.FirstName))
                    .ForMember(dist => dist.LastName, opt => opt.MapFrom(src => src.PersonalData.LastName))
                    .ForMember(dist => dist.Email, opt => opt.MapFrom(src => src.PersonalData.Email))
                    .ForMember(dist => dist.Address, opt => opt.MapFrom(src => src.PersonalData.Address))
                    .ReverseMap();
                cfg.CreateMap<Pharmaceut, PersonDto>()
                    .ForMember(dist => dist.Type, opt => opt.MapFrom(src => PersonType.Pharmaceut))
                    .ForMember(dist => dist.FirstName, opt => opt.MapFrom(src => src.PersonalData.FirstName))
                    .ForMember(dist => dist.LastName, opt => opt.MapFrom(src => src.PersonalData.LastName))
                    .ForMember(dist => dist.Email, opt => opt.MapFrom(src => src.PersonalData.Email))
                    .ForMember(dist => dist.Address, opt => opt.MapFrom(src => src.PersonalData.Address))
                    .ForMember(dist=>dist.PharmacyName,opt=>opt.MapFrom(src=>src.Pharmacy.PharmacyName))
                    .ReverseMap();
                cfg.CreateMap<Address, AddressDto>()
                    .ReverseMap();
                cfg.CreateMap<Pharmacy, PharmacyDto>()
                    .ForMember(dist => dist.Name, opt => opt.MapFrom(src => src.PharmacyName))
                    .ReverseMap();
                cfg.CreateMap<Medicine, MedicineDto>()
                    .ReverseMap();
                cfg.CreateMap<PrescriptionCreationDto, Prescription>()
                    .ForMember(dist => dist.PrescribedToCpr, opt => opt.MapFrom(src => src.PatientCprNumber))
                    .ForMember(dist => dist.PrescribedBy, opt => opt.MapFrom(src => src.DoctorId))
                    .ForMember(dist => dist.Medicine, opt => opt.MapFrom(src => src.MedicineName))
                    .ReverseMap();
                cfg.CreateMap<string, Medicine>()
                    .ForCtorParam("name", opt => opt.MapFrom(src => src))
                    .ReverseMap()
                    .ConvertUsing(src => src.Name);
            })));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});


app.UseAuthorization();

app.MapControllers();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();
