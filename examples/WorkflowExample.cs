// -----------------------------------------------------------------------
// <copyright file="WorkflowExample.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FuzzyPotato.Core.Models.Examples;
    using FuzzyPotato.Core.Serialization;

    /// <summary>
    /// Example demonstrating workflow serialization and deserialization.
    /// </summary>
    public class WorkflowExample
    {
        /// <summary>
        /// Runs the workflow example.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task RunAsync()
        {
            // Types are automatically discovered - no manual registration needed

            // Create a sample workflow
            var workflow = CreateDataProcessingWorkflow();

            // Demonstrate JSON serialization
            await DemonstrateJsonSerializationAsync(workflow);

            // Demonstrate YAML serialization
            await DemonstrateYamlSerializationAsync(workflow);

            // Demonstrate node execution
            await DemonstrateNodeExecutionAsync(workflow);
        }

        private static WorkflowDefinition CreateDataProcessingWorkflow()
        {
            var workflow = new WorkflowDefinition
            {
                Id = "data-processing-workflow",
                Name = "Data Processing Workflow",
                Description = "Fetches data from API, processes it, and stores results",
                Version = "1.0.0",
                StartNodeId = "fetch-data",
                Nodes = new List<NodeDefinition>
                {
                    new HttpRequestNode
                    {
                        Id = "fetch-data",
                        Name = "Fetch Data from API",
                        Description = "Retrieve data from external API",
                        PositionX = 100,
                        PositionY = 100,
                        Method = "GET",
                        Url = "https://api.example.com/data",
                        Headers = new Dictionary<string, string>
                        {
                            ["Authorization"] = "Bearer your-token-here",
                            ["Content-Type"] = "application/json",
                        },
                        TimeoutMs = 30000,
                    },
                    new IfElseNode
                    {
                        Id = "check-response",
                        Name = "Check API Response",
                        Description = "Validate the API response",
                        PositionX = 300,
                        PositionY = 100,
                        Condition = "response.StatusCode == 200 && response.Data != null",
                        TrueNodeId = "process-data",
                        FalseNodeId = "error-handler",
                    },
                    new CSharpNode
                    {
                        Id = "process-data",
                        Name = "Process Data",
                        Description = "Transform and validate the data",
                        PositionX = 500,
                        PositionY = 50,
                        Code = @"
var items = JsonSerializer.Deserialize<List<DataItem>>(response.Data);
var validItems = items.Where(x => x.IsValid).ToList();
var processedCount = validItems.Count;
Console.WriteLine($""Processed {processedCount} valid items"");
return validItems;",
                        Usings = new List<string>
                        {
                            "System",
                            "System.Linq",
                            "System.Text.Json",
                            "System.Collections.Generic",
                        },
                        References = new List<string>
                        {
                            "System.Text.Json",
                        },
                        TimeoutMs = 10000,
                    },
                    new WhileLoopNode
                    {
                        Id = "batch-processor",
                        Name = "Batch Process Items",
                        Description = "Process items in batches",
                        PositionX = 700,
                        PositionY = 50,
                        Condition = "batchIndex < totalBatches",
                        MaxIterations = 100,
                        LoopBodyStartNodeId = "process-batch",
                        BreakOnError = true,
                    },
                    new CSharpNode
                    {
                        Id = "process-batch",
                        Name = "Process Single Batch",
                        Description = "Process a batch of items",
                        PositionX = 850,
                        PositionY = 50,
                        Code = @"
var batch = validItems.Skip(batchIndex * batchSize).Take(batchSize);
foreach (var item in batch)
{
    // Process item
    ProcessItem(item);
}
batchIndex++;",
                        Usings = new List<string> { "System", "System.Linq" },
                        TimeoutMs = 5000,
                    },
                    new DelayNode
                    {
                        Id = "rate-limiter",
                        Name = "Rate Limiter",
                        Description = "Delay between batches to avoid rate limits",
                        PositionX = 1000,
                        PositionY = 50,
                        DelayMs = 1000,
                    },
                    new PowerShellScriptNode
                    {
                        Id = "error-handler",
                        Name = "Error Handler",
                        Description = "Log and handle errors",
                        PositionX = 500,
                        PositionY = 150,
                        Script = @"
param($errorMessage, $context)
Write-Error ""Workflow error: $errorMessage""
Send-AlertEmail -Message $errorMessage -Context $context
",
                        ExecutionPolicy = "RemoteSigned",
                        Parameters = new Dictionary<string, object>
                        {
                            ["errorMessage"] = "API request failed",
                            ["context"] = "data-processing-workflow",
                        },
                    },
                },
                Connections = new List<WorkflowConnection>
                {
                    new() { SourceNodeId = "fetch-data", TargetNodeId = "check-response", Label = "response" },
                    new() { SourceNodeId = "check-response", TargetNodeId = "process-data", Label = "success", SourcePort = "true" },
                    new() { SourceNodeId = "check-response", TargetNodeId = "error-handler", Label = "error", SourcePort = "false" },
                    new() { SourceNodeId = "process-data", TargetNodeId = "batch-processor", Label = "validated-data" },
                    new() { SourceNodeId = "batch-processor", TargetNodeId = "process-batch", Label = "continue" },
                    new() { SourceNodeId = "process-batch", TargetNodeId = "rate-limiter", Label = "batch-complete" },
                },
                Variables = new Dictionary<string, object>
                {
                    ["apiBaseUrl"] = "https://api.example.com",
                    ["batchSize"] = 100,
                    ["maxRetries"] = 3,
                    ["timeout"] = 30000,
                },
            };

            Console.WriteLine($"✓ Created workflow: {workflow.Name}");
            Console.WriteLine($"  - {workflow.Nodes.Count} nodes");
            Console.WriteLine($"  - {workflow.Connections.Count} connections\n");

            return workflow;
        }

        private static async Task DemonstrateJsonSerializationAsync(WorkflowDefinition workflow)
        {
            Console.WriteLine("=== JSON Serialization Demo ===\n");

            var jsonSerializer = new FuzzyJsonSerializer();

            // Serialize to JSON
            var json = jsonSerializer.SerializeObject(workflow);
            Console.WriteLine("Serialized workflow to JSON:");
            Console.WriteLine(json.Substring(0, Math.Min(500, json.Length)) + "...\n");

            // Deserialize from JSON
            var deserialized = jsonSerializer.DeserializeObject<WorkflowDefinition>(json);
            Console.WriteLine($"✓ Deserialized workflow: {deserialized?.Name}");
            Console.WriteLine($"  - {deserialized?.Nodes.Count} nodes restored");
            Console.WriteLine($"  - {deserialized?.Connections.Count} connections restored\n");

            // Save to file
            var jsonFile = "workflow.json";
            await jsonSerializer.SerializeObjectToFileAsync(jsonFile, workflow);
            Console.WriteLine($"✓ Saved workflow to {jsonFile}\n");

            // Load from file
            var loaded = await jsonSerializer.DeserializeObjectFromFileAsync<WorkflowDefinition>(jsonFile);
            Console.WriteLine($"✓ Loaded workflow from {jsonFile}: {loaded?.Name}\n");
        }

        private static async Task DemonstrateYamlSerializationAsync(WorkflowDefinition workflow)
        {
            Console.WriteLine("=== YAML Serialization Demo ===\n");

            var yamlSerializer = new FuzzyYamlSerializer();

            // Serialize to YAML
            var yaml = yamlSerializer.SerializeObject(workflow);
            Console.WriteLine("Serialized workflow to YAML:");
            Console.WriteLine(yaml.Substring(0, Math.Min(500, yaml.Length)) + "...\n");

            // Deserialize from YAML
            var deserialized = yamlSerializer.DeserializeObject<WorkflowDefinition>(yaml);
            Console.WriteLine($"✓ Deserialized workflow: {deserialized?.Name}");
            Console.WriteLine($"  - {deserialized?.Nodes.Count} nodes restored");
            Console.WriteLine($"  - {deserialized?.Connections.Count} connections restored\n");

            // Save to file
            var yamlFile = "workflow.yaml";
            await yamlSerializer.SerializeObjectToFileAsync(yamlFile, workflow);
            Console.WriteLine($"✓ Saved workflow to {yamlFile}\n");
        }

        private static async Task DemonstrateNodeExecutionAsync(WorkflowDefinition workflow)
        {
            Console.WriteLine("=== Node Execution Demo ===\n");

            var factory = new NodeFactory();
            var executableNodes = factory.CreateWorkflowNodes(workflow);

            Console.WriteLine($"✓ Created {executableNodes.Count} executable nodes\n");

            // Execute a sample node
            var httpNode = executableNodes["fetch-data"];
            var context = new WorkflowExecutionContext();

            Console.WriteLine($"Executing node: {httpNode.NodeId}");
            var result = await httpNode.ExecuteAsync(context);

            Console.WriteLine($"✓ Execution result:");
            Console.WriteLine($"  - Success: {result.Success}");
            Console.WriteLine($"  - Output: {result.Output}");
            Console.WriteLine($"\nExecution trace:");
            foreach (var trace in context.ExecutionTrace)
            {
                Console.WriteLine($"  - {trace}");
            }
        }
    }
}
