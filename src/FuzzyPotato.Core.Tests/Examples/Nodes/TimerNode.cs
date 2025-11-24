// -----------------------------------------------------------------------
// <copyright file="TimerNode.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

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
