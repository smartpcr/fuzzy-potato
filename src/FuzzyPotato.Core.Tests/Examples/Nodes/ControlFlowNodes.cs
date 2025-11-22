// -----------------------------------------------------------------------
// <copyright file="ControlFlowNodes.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Node that performs conditional branching based on a boolean expression.
    /// Routes to TrueBranch or FalseBranch ports based on condition evaluation.
    /// </summary>
    [JsonConverter(typeof(IfElseNodeJsonConverter))]
    public class IfElseNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "if-else-node";

        /// <summary>
        /// Gets or sets the C# boolean expression to evaluate.
        /// Expression has access to workflow variables via ExecutionState.
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the node to execute when condition is true.
        /// </summary>
        public string? TrueNodeId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the node to execute when condition is false.
        /// </summary>
        public string? FalseNodeId { get; set; }
    }

    /// <summary>
    /// Node that iterates over a collection and executes child nodes for each item.
    /// Sets workflow variables with current item and iteration index.
    /// </summary>
    [JsonConverter(typeof(ForEachNodeJsonConverter))]
    public class ForEachNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "foreach-node";

        /// <summary>
        /// Gets or sets the C# expression that returns an IEnumerable collection.
        /// </summary>
        public string CollectionExpression { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the variable name for the current item in each iteration.
        /// Defaults to "item".
        /// </summary>
        public string ItemVariableName { get; set; } = "item";

        /// <summary>
        /// Gets or sets a value indicating whether to continue iteration on item error.
        /// </summary>
        public bool ContinueOnError { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of concurrent iterations (for parallel foreach).
        /// Default 1 means sequential execution.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = 1;
    }

    /// <summary>
    /// Node that iterates while a condition is true with feedback loop architecture.
    /// Re-evaluates condition after each iteration based on updated workflow state.
    /// </summary>
    [JsonConverter(typeof(WhileNodeJsonConverter))]
    public class WhileNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "while-node";

        /// <summary>
        /// Gets or sets the C# boolean expression to evaluate before each iteration.
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum number of iterations to prevent infinite loops.
        /// Defaults to 1000.
        /// </summary>
        public int MaxIterations { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether to break the loop on error.
        /// </summary>
        public bool BreakOnError { get; set; } = true;
    }

    /// <summary>
    /// Node that performs multi-way branching based on expression result matching.
    /// Routes to specific port based on which case matches, or Default port if no match.
    /// </summary>
    [JsonConverter(typeof(SwitchNodeJsonConverter))]
    public class SwitchNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "switch-node";

        /// <summary>
        /// Gets or sets the C# expression to evaluate.
        /// Result is converted to string for case matching.
        /// </summary>
        public string Expression { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the case mappings.
        /// Key: Value to match (e.g., "success", "error").
        /// Value: Port name to route to (if empty, uses key as port name).
        /// </summary>
        public Dictionary<string, string> Cases { get; set; } = new();

        /// <summary>
        /// Gets or sets the default port name when no case matches.
        /// Defaults to "Default".
        /// </summary>
        public string DefaultPort { get; set; } = "Default";

        /// <summary>
        /// Gets or sets a value indicating whether case matching is case-sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; } = true;
    }

    /// <summary>
    /// Node that triggers workflow execution based on a cron schedule.
    /// Uses NCrontab for cron expression parsing.
    /// </summary>
    [JsonConverter(typeof(TimerNodeJsonConverter))]
    public class TimerNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "timer-node";

        /// <summary>
        /// Gets or sets the cron schedule expression.
        /// Example: "0 2 * * *" triggers at 2 AM daily.
        /// Format: minute hour day month day-of-week.
        /// </summary>
        public string Schedule { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to trigger immediately on first run.
        /// If false, waits for first scheduled occurrence.
        /// </summary>
        public bool TriggerOnStart { get; set; } = false;

        /// <summary>
        /// Gets or sets the timezone for schedule evaluation.
        /// Defaults to UTC.
        /// </summary>
        public string TimeZone { get; set; } = "UTC";

        /// <summary>
        /// Gets or sets a value indicating whether the timer is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}
