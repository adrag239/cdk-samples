using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Constructs;

namespace Fargate
{
    public class FargateStack : Stack
    {
        internal FargateStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Create VPC
            var vpc = new Vpc(this, "SampleVpc", new VpcProps
            {
                MaxAzs = 2
            });

            // Create ECS cluster
            var cluster = new Cluster(this, "SampleCluster", new ClusterProps
            {
                Vpc = vpc
            });

            var taskDef = new FargateTaskDefinition(this, "FargateTaskDefinition");

            // Build container image from local assets
            var containerOptions = new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromAsset("webapp")
            };

            var portMapping = new PortMapping()
            {
                ContainerPort = 80,
                HostPort = 80
            };

            taskDef
                .AddContainer("Container", containerOptions)
                .AddPortMappings(portMapping);

            // Create Fargate Service behind Application Load Balancer
            new ApplicationLoadBalancedFargateService(this, "DotnetFargateSampleApp", new ApplicationLoadBalancedFargateServiceProps()
            {
                Cluster = cluster,
                MemoryLimitMiB = 512,
                Cpu = 256,
                TaskDefinition = taskDef
            });
        }
    }
}
