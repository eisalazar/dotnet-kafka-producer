using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StreamProcessor.Services;

[assembly: HostingStartup(typeof(StreamProcessor.Startup))]

namespace StreamProcessor
{
  public class Startup : IHostingStartup
  {
    // This method gets called by the runtime. Use this method to add servicses to the container.
    public void Configure(IWebHostBuilder builder)
    {
      builder.ConfigureServices((ctx, c) =>
      {
        c.AddHostedService<ProducerService>();
      });
    }
  }
}
