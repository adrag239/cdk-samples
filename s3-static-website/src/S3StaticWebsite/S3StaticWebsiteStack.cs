using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Constructs;

namespace S3StaticWebsite
{
    public class S3StaticWebsiteStack : Stack
    {
        internal S3StaticWebsiteStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Create bucket to host a static website; the absence of the BucketName
            // property means the CDK will auto-name the resource
            var bucket = new Bucket(this, "WebsiteBucket", new BucketProps
            {
                PublicReadAccess = true,
                BucketName = "cdk-samples-test-bucket",
                WebsiteIndexDocument = "index.html"
            });

            // deploy the site
            new BucketDeployment(this, "WebsiteBucketDeployment", new BucketDeploymentProps
            {
                DestinationBucket = bucket,
                Sources = new[] { Source.Asset("./site-contents") }
            });

            // emit the url of the website for convenience
            new CfnOutput(this, "BucketWebsiteUrl", new CfnOutputProps
            {
                Value = bucket.BucketWebsiteUrl
            });
        }
    }
}
