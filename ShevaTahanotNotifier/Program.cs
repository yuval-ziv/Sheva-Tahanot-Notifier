using ShevaTahanotNotifier;
using ShevaTahanotNotifier.ExtensionMethods;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
WebApplication app = await ShevaTahanotNotifierConfigurator.ConfigureAsync(builder);
await app.RunAsync();