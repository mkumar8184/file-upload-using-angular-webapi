

using AttachmentDomain.Entity;
using AttachmentUtility;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;


namespace FileUploadWebApi.ServiceResolver
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection ServiceResolver(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddCors();
            services.AddDbContext<AttachmentExampleContext>(options =>
             options.UseSqlServer(configuration["AppConfig:AttachmentContext"]));
            services.AddScoped<IAttachmentService, AttachmentService>();
            return services;
          
        }
    }
}
