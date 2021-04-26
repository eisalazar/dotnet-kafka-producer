using Infra.Full.Configuration;
using Infra.WebHost;
using System;
using System.Collections.Generic;

namespace StreamProcessor
{
  public class Program : IServiceStatus
  {
    public ServiceState CurrentState { get; set; }

    public string ServiceName => "PlatformApiService";

    public string ServiceDisplayName => "Platform Api Service";

    public string ServiceDescription => "Platform Api Service";

    public static void Main(string[] args)
    {
      WebHostWrapper.Run(args, new Program());
    }

    public void AfterStart()
    {
    }

    public void AfterStop()
    {
    }

    public void BeforeStart()
    {
    }

    public void BeforeStop()
    {
    }
  }
}
