using Amazon.CDK;
using Amazon.CDK.AWS.ElasticBeanstalk;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace EbDotnetLinux
{
    public class EbDotnetLinuxStack : Stack
    {
        internal EbDotnetLinuxStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var ebInstanceRole = new Role(this, "SampleApp-Role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com"),
                ManagedPolicies = new[] { ManagedPolicy.FromAwsManagedPolicyName("AWSElasticBeanstalkWebTier") }
            });

            var instanceProfile = new CfnInstanceProfile(this, "SampleApp-InstanceProfile", new CfnInstanceProfileProps
            {
                InstanceProfileName = "SampleApp-InstanceProfile",
                Roles = new[] { ebInstanceRole.RoleName }
            });

            var optionSettingProperties = new CfnEnvironment.OptionSettingProperty[] {
                  new CfnEnvironment.OptionSettingProperty {
                    Namespace = "aws:autoscaling:launchconfiguration",
                    OptionName = "InstanceType",
                    Value = "t3.small",
                  },
                  new CfnEnvironment.OptionSettingProperty {
                    Namespace = "aws:autoscaling:launchconfiguration",
                    OptionName = "IamInstanceProfile",
                    Value = instanceProfile.InstanceProfileName
                  }
            };

            var app = new CfnApplication(this, "SampleApp-App", new CfnApplicationProps
            {
                ApplicationName = "SampleApp"
            });

            var env = new CfnEnvironment(this, "SampleApp-Env", new CfnEnvironmentProps
            {
                EnvironmentName = "SampleAppEnvironment",
                ApplicationName = app.ApplicationName,
                // please check the latest platform name at https://docs.aws.amazon.com/elasticbeanstalk/latest/platforms/platforms-supported.html
                SolutionStackName = "64bit Amazon Linux 2 v2.2.8 running .NET Core",
                OptionSettings = optionSettingProperties
            });

            // to ensure the application is created before the environment
            env.AddDependsOn(app);
        }
    }
}
