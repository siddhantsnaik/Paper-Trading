var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OnPaper>("onpaper");

builder.Build().Run();
