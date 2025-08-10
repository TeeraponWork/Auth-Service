using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "Auth Service API",
                Version = description.ApiVersion.ToString(),
                Description =
                    "Auth Service สำหรับจัดการการยืนยันตัวตนและสิทธิ์การเข้าถึงระบบ (Authentication/Authorization). " +
                    "รองรับการลงทะเบียนผู้ใช้ (Register), เข้าสู่ระบบ (Login) เพื่อรับ JWT, " +
                    "หมุนเวียน Refresh Token (Token Rotation), และโครงสร้างบทบาท/สิทธิ์ (Roles/Claims). " +
                    "ทุก endpoint ที่ต้องการสิทธิ์ให้ส่ง `Authorization: Bearer <JWT>` ใน Header.",
                Contact = new OpenApiContact
                {
                    Name = "Auth Service Team",
                    Email = "support@authservice.local",
                    Url = new Uri("https://authservice.local/docs")
                },
                License = new OpenApiLicense
                {
                    Name = "Internal Use Only",
                    Url = new Uri("https://authservice.local/license")
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " เวอร์ชันนี้ถูกยกเลิกการใช้งาน (deprecated).";
            }

            return info;
        }
    }
}
