using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using System.Collections.Generic;

namespace RandomWriter
{
    public class RandomWriterStack : Stack
    {
        internal RandomWriterStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            var randomWriter = new RandomWriter(this, "RandomWriter");

            new Rule(this, "Trigger", new RuleProps()
            {
                Description = "Triggers a RandomWrite every minute",
                Schedule = Schedule.Rate(Duration.Minutes(1)),
                Targets = new[] { randomWriter }
            });
        }
    }

    internal sealed class RandomWriter : Construct, IRuleTarget
    {
        private IFunction Function { get; }

        public RandomWriter(Construct scope, string id) : base(scope, id)
        {
            // Create DynamoDB table
            var table = new Table(this, "Table", new TableProps
            {
                PartitionKey = new Attribute
                {
                    Name = "ID",
                    Type = AttributeType.STRING
                }
            });

            Function = new Function(this, "Lambda", new FunctionProps
            {
                Runtime = Runtime.NODEJS_14_X,
                Handler = "index.handler",
                Code = Code.FromAsset("lambda"),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", table.TableName }
                }
            });

            // give Lambda function access to write to DynamoDB table
            table.GrantReadWriteData(Function);
        }

        public IRuleTargetConfig Bind(IRule rule, string id = null)
        {
            return new LambdaFunction(Function).Bind(rule, id);
        }
    }

}
