# Hello ServiceFabric

This project contains a Hello World example of running a ServiceStack Self Hosted Service 
inside [Microsoft's Service Fabric platform](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-overview).

Service Fabric is a distributed systems platform that enables wrapping your self-hosted Services
inside a managed package, allowing them to be managed like Containers where they can be 
scaled on a cluster managed independently using Service Fabric's orchestration tooling.

![](https://raw.githubusercontent.com/ServiceStack/docs/master/docs/images/release-notes/service-fabric-overview.png)

## [Create clusters for Service Fabric anywhere](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-overview#create-clusters-for-service-fabric-anywhere) 

You can create clusters for Service Fabric in many environments, including Azure or on premises, 
on Windows Server, or on Linux. In addition, the development environment in the SDK is 
identical to the production environment, and no emulators are involved. 

For more information on creating clusters on-premises, read 
[creating a cluster on Windows Server or Linux](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-deploy-anywhere) 
or for Azure creating a cluster 
[via the Azure portal](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cluster-creation-via-portal).

## Notes

Service Fabric requires running VS.NET in **Admin Mode** in order to launch/debug your Web App 
in a local Service Fabric cluster. 

The endpoint for your Self-Hosted Service is configured in **PackageRoot\ServiceManifest.xml**
which defaults to:

```xml
<Resources>
<Endpoints>
    <!-- This endpoint is used by the communication listener to obtain the port on which to 
        listen. Please note that if your service is partitioned, this port is shared with 
        replicas of different partitions that are placed in your code. -->
    <!--<Endpoint Name="ServiceEndpoint" />-->
    <Endpoint Name="ServiceEndpoint" Type="Input" Protocol="http" Port="8281" />
</Endpoints>
</Resources>
```

After running your Web App in VS.NET, you can check that your ServiceStack Service is working 
by navigating to [http://localhost:8281/metadata](http://localhost:8281/metadata).

### Changes from Service Fabric Application VS.NET template

The main difference of this project from the default Service Fabric default VS.NET 
**Service Fabric Application** Template is the `SelfHostCommunicationListener.cs` adapter class 
which implements Service Fabric's `ICommunicationListener` interface for managing the life-cycle 
of a ServiceStack Self-Host Service. 

When creating a new Service Fabric Self Hosted Service from VS.NET template you can just copy
into your project:

 - [SelfHostCommunicationListener.cs](https://github.com/ServiceStackApps/HelloServiceFabric/blob/master/src/StatelessSelfHost/SelfHostCommunicationListener.cs)

Then change your `StatelessSelfHost` to use the class configured with your ServiceStack AppHost, e.g:

```csharp
protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
{
    var endpoints = Context.CodePackageActivationContext.GetEndpoints()
        .Where(endpoint => endpoint.Protocol == EndpointProtocol.Http || endpoint.Protocol == EndpointProtocol.Https)
        .Select(endpoint => endpoint.Name);

    return endpoints.Select(endpoint => new ServiceInstanceListener(
        serviceContext => new SelfHostCommunicationListener(
            new AppHost(), serviceContext, ServiceEventSource.Current, endpoint), endpoint));
}
```