using Amazon.CDK;

namespace S3StaticWebsite
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new S3StaticWebsiteStack(app, "S3StaticWebsiteStack");

            app.Synth();
        }
    }
}
