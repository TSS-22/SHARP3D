namespace SHARP3D.Exceptions
{
    /// <summary>
    /// Exception thrown when a force plate doesn't have calibration matrix due to its type.
    /// </summary>
    public class NoCalibrationMatrixForForceplateType : Exception
    {

        /// <summary>
        /// Initializes a new instance of the C3dBadFormatingException class with a specified error message.
        /// </summary>
        /// <param name="message">A string that provide more deatils on where/why the Exception occured.</param>
        public NoCalibrationMatrixForForceplateType(string message)
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
        public NoCalibrationMatrixForForceplateType(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
