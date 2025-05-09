using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

DaprSidecarOptions sidecarOptionsUser = new()
{
	AppId = "user-sidecar",
	DaprGrpcPort = 8085,
	DaprHttpPort = 3500,
	MetricsPort = 9090
};
DaprSidecarOptions sidecarOptionsDocument = new()
{
	AppId = "document-sidecar",
	DaprGrpcPort = 8086,
	DaprHttpPort = 3501,
	MetricsPort = 9091
};
DaprSidecarOptions sidecarOptionsAcademy = new()
{
	AppId = "academy-sidecar",
	DaprGrpcPort = 8087,
	DaprHttpPort = 3502,
	MetricsPort = 9092
};
DaprSidecarOptions sidecarOptionsAnalyse = new()
{
	AppId = "analyse-sidecar",
	DaprGrpcPort = 8088,
	DaprHttpPort = 3503,
	MetricsPort = 9093
};
DaprSidecarOptions sidecarOptionsMedia = new()
{
	AppId = "media-sidecar",
	DaprGrpcPort = 8089,
	DaprHttpPort = 3504,
	MetricsPort = 9094
};
var userService = builder.AddProject("userservice", "../UserService-Microservice/Application/Application.csproj")
						.WithDaprSidecar(sidecarOptionsUser)
						.WithExternalHttpEndpoints();

var documentService = builder.AddProject("documentservice", "../DocumentService-Microservice/Application/Application.csproj")
						.WithDaprSidecar(sidecarOptionsDocument)
						.WithExternalHttpEndpoints();

var gateway = builder.AddProject("gateway", "../highschool-apigateway/Yarp.APIGateway/Yarp.APIGateway.csproj")
						.WithExternalHttpEndpoints();

var mediaService = builder.AddProject("mediaservice", "../MediaService-Microservice/src/Application/Application.csproj")
						.WithDaprSidecar(sidecarOptionsMedia)
						.WithExternalHttpEndpoints();

var notificationService = builder.AddProject("notificationservice", "../NotificationService-Microservice/Application/Application.csproj")
						.WithExternalHttpEndpoints();

var academyHubService = builder.AddProject("academyhubservice", "../AcademyHubService/Application/Application.csproj")
						.WithDaprSidecar(sidecarOptionsAcademy)
						.WithExternalHttpEndpoints();

var analyseService = builder.AddProject("analyseservice", "../AnalyseService-Microservice/src/Application/Application.csproj")
						.WithDaprSidecar(sidecarOptionsAnalyse)
						.WithExternalHttpEndpoints();

builder.Build().Run();
