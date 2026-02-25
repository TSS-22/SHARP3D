namespace SHARP3D.Exceptions
{
    public class ParameterNotFoundException : ArgumentException
    {

        /// <summary>
        /// Initializes a new instance of the ParameterNotFoundException class with a specified error message.
        /// </summary>
        /// <param name="message">A string that provide more deatils on where/why the Exception occured.</param>
        public ParameterNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParameterNotFoundException class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is
        /// specified.</param>
        public ParameterNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
