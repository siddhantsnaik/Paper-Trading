var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OnPaper_Trade>("onpaper-trade");

builder.Build().Run();
