using System;
using System.Linq;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands
{
    /// <summary>
    /// A <see cref="IApplicationCommand" /> implementation that connects to a plc.
    /// </summary>
    internal abstract class PlcCommandBase : CommandBase
    {
        private readonly IPlcConnection plcConnection;
        private readonly IReadWrite readWrite;
        private readonly ISymbolHandler symbolHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlcCommandBase"/> class.
        /// </summary>
        /// <param name="commandName">The name of this command.</param>
        /// <param name="plcConnection">A <see cref="IPlcConnection"/>.</param>
        /// <param name="readWrite">A <see cref="IReadWrite"/>.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/>.</param>
        protected PlcCommandBase(string commandName, IPlcConnection plcConnection, IReadWrite readWrite, ISymbolHandler symbolHandler)
            : base(commandName)
        {
            HelpTekst = commandName + ": Common plc actions";
            VallidParameters = new[] { "connect", "disconnect", "read", "write", "dump", "toggle" };
            this.plcConnection = plcConnection;
            this.readWrite = readWrite;
            this.symbolHandler = symbolHandler;
        }

        /// <inheritdoc />
        public override Response Execute(string[] parameters)
        {
            if (parameters.Length <= 0)
            {
                return new Response(HelpTekst);
            }

            if (string.Equals(parameters[0], VallidParameters[0], StringComparison.OrdinalIgnoreCase))
            {
                _ = plcConnection.ConnectAsync();
                return new Response("Connecting to the plc");
            }

            if (string.Equals(parameters[0], VallidParameters[1], StringComparison.OrdinalIgnoreCase))
            {
                _ = plcConnection.DisconnectAsync();
                return new Response("Disconnecting from the plc");
            }

            if (string.Equals(parameters[0], VallidParameters[2], StringComparison.OrdinalIgnoreCase) && parameters.Length == 2)
            {
                var value = readWrite.Read(parameters[1]).ToString();
                if (value == null)
                {
                    return new Response($"Failed reading value from {parameters[1]}");
                }

                return new Response(value);
            }

            if (string.Equals(parameters[0], VallidParameters[3], StringComparison.OrdinalIgnoreCase))
            {
                readWrite.Write(parameters[1], true);
                return new Response("Value written to PLC");
            }

            if (string.Equals(parameters[0], VallidParameters[4], StringComparison.OrdinalIgnoreCase))
            {
                var tags = string.Join(Environment.NewLine, symbolHandler.AllSymbols.Select(x => x.Name));
                return new Response(tags);
            }

            if (string.Equals(parameters[0], VallidParameters[5], StringComparison.OrdinalIgnoreCase))
            {
                readWrite.ToggleBool(parameters[1]);
                return new Response("Value toggled");
            }

            return new Response("Invallid parameter");
        }
    }
}