using System;
using System.Collections.Generic;

namespace PlcInterface.Sandbox.Interactive;

/// <summary>
/// A base implementation of a <see cref="IApplicationCommand"/>.
/// </summary>
internal abstract class CommandBase : IApplicationCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase"/> class.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    protected CommandBase(string name)
    {
        Name = name;
        HelpText = string.Empty;
        ValidParameters = Array.Empty<string>();
    }

    /// <inheritdoc/>
    public string HelpText
    {
        get;
        protected set;
    }

    /// <inheritdoc/>
    public string Name
    {
        get;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> ValidParameters
    {
        get;
        protected set;
    }

    /// <inheritdoc/>
    public abstract Response Execute(string[] parameters);
}