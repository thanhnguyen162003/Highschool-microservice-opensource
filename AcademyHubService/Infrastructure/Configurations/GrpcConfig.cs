//using Application;
//using Domain.Models.Settings;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infrastructure.Configurations
//{
//    public static class GrpcConfig
//    {
//        public static void AddGrpcConfig(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddGrpc();
//            var grpcSettings = configuration.GetSection("GrpcSetting").Get<GrpcSetting>();
//            services.AddGrpcClient<AcademicServiceRpc.AcademicServiceRpcClient>(opt =>
//            {
//                opt.Address = new Uri(grpcSettings?.UserService ?? "");
//            });
//        }
//    }
//}
