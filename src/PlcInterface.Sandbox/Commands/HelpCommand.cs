using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands
{
    /// <summary>
    /// A <see cref="IApplicationCommand" /> that prints a help text.
    /// </summary>
    internal sealed class HelpCommand : CommandBase
    {
        private const string CommandName = "help";
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand" /> class.
        /// </summary>
        /// <param name="serviceProvider">A <see cref="IServiceProvider" /> implementation.</param>
        public HelpCommand(IServiceProvider serviceProvider)
            : base(CommandName)
        {
            HelpTekst = CommandName + ": prints this information";
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public override Response Execute(string[] parameters)
        {
            var builder = new StringBuilder();
            var consoleCommands = serviceProvider.GetServices<IApplicationCommand>();
            foreach (var comm in consoleCommands)
            {
                _ = builder.AppendLine(comm.HelpTekst);
            }

            return new Response(builder.ToString());
        }
    }
}