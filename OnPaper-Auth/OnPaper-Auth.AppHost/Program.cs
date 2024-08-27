var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OnPaper_Auth>("onpaper-auth");

builder.Build().Run();
