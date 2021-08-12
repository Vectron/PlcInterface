using System.Collections.Generic;

namespace PlcInterface.Sandbox.Interactive
{
    /// <summary>
    /// A interface describing a command that can be executed.
    /// </summary>
    public interface IApplicationCommand
    {
        /// <summary>
        /// Gets tekst explaining this <see cref="IApplicationCommand" />.
        /// </summary>
        string HelpTekst
        {
            get;
        }

        /// <summary>
        /// Gets the name of this <see cref="IApplicationCommand" />.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}" /> with vallid parameters.
        /// </summary>
        IReadOnlyList<string> VallidParameters
        {
            get;
        }

        /// <summary>
        /// Execute this command.
        /// </summary>
        /// <param name="parameters">Collection of parameters for this command.</param>
        /// <returns>A <see cref="Response" /> with the command result.</returns>
        Response Execute(string[] parameters);
    }
}