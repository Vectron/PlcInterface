namespace PlcInterface.Sandbox.Interactive
{
    /// <summary>
    /// A response from doing a action.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
        /// <param name="message">The message that has to be responded.</param>
        public Response(string message)
            => Message = message;

        /// <summary>
        /// Gets a empty response.
        /// </summary>
        public static Response Empty => new(string.Empty);

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string Message
        {
            get;
        }
    }
}