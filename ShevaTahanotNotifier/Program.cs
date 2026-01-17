using ShevaTahanotNotifier.ExtensionMethods;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.RegisterShevaTahanotNotifier();
WebApplication app = builder.Build();
await app.MapShevaTahanotNotifier();
await app.RunAsync();