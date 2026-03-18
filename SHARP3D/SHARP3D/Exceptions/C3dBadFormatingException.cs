using SHARP3D.Utils.Enum;


namespace SHARP3D.Exceptions
{
    /// <summary>
    /// Exception thrown when an C3D file is badly formatted.
    /// </summary>
    public class C3dBadFormatingException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the C3dBadFormatingException class with a specified error message.
        /// </summary>
        /// <param name="message">A string that provide more deatils on where/why the Exception occured.</param>
        public C3dBadFormatingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the C3dBadFormatingException class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is
        /// specified.</param>
        public C3dBadFormatingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an C3D file is badly formatted.
    /// </summary>
    public class C3dIncompatiblePointUsedValuesException : C3dBadFormatingException
    {

        /// <summary>
        /// Initializes a new instance of the C3dIncompatiblePointUsedValuesException class with a specified error message.
        /// </summary>
        /// <param name="message">A string that provide more deatils on where/why the Exception occured.</param>
        public C3dIncompatiblePointUsedValuesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the C3dIncompatiblePointUsedValuesException class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is
        /// specified.</param>
        public C3dIncompatiblePointUsedValuesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
