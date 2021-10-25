using System;
using System.Collections.Generic;

namespace PlcInterface.Sandbox.Interactive;

/// <summary>
/// A base implementation of a <see cref="IApplicationCommand" />.
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
        HelpTekst = string.Empty;
        VallidParameters = Array.Empty<string>();
    }

    /// <inheritdoc />
    public string HelpTekst
    {
        get;
        protected set;
    }

    /// <inheritdoc />
    public string Name
    {
        get;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> VallidParameters
    {
        get;
        protected set;
    }

    /// <inheritdoc />
    public abstract Response Execute(string[] parameters);
}