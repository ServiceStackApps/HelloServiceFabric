using Funq;
using ServiceStack;

namespace StatelessSelfHost
{
    [Route("/hello/")]
    [Route("/hello/{Name}")]
    public class Hello : IReturn<HelloResponse>
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }

    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }
    }

    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("ServiceFabric SelfHost", typeof(MyServices).Assembly) {}

        public override void Configure(Container container)
        {
        }
    }
}