namespace GLFW
{
    /// <summary>
    ///     Base exception class for GLFW related errors.
    /// </summary>
    internal class GLFWException : System.Exception
    {
        #region Constants
        // no use in allocating a string every time there's an error, so like the rest
        // of Red, we use static strings.
        private static string notInitialized = "Not Initialized";
        private static string noCurrentContext = "No Current Context";
        private static string invalidEnum = "Invalid Enum";
        private static string invalidValue = "Invalid Value";
        private static string outOfMemory = "Out Of Memory";
        private static string apiUnavailable = "Api Unavailable";
        private static string versionUnavailable = "Version Unavailable";
        private static string platformError = "Platform Error";
        private static string formatUnavailable = "Format Unavailable";
        private static string noWindowContext = "No Window Context";
        private static string unknownError = "Unknown Error";
        #endregion
        #region Methods

        /// <summary>
        ///     Generic error messages if only an error code is supplied as an argument to the constructor.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <returns>Error message.</returns>
        public static string GetErrorMessage(ErrorCode code)
        {
            switch (code)
            {
                case ErrorCode.NotInitialized: return notInitialized;
                case ErrorCode.NoCurrentContext: return noCurrentContext;
                case ErrorCode.InvalidEnum: return invalidEnum;
                case ErrorCode.InvalidValue: return invalidValue;
                case ErrorCode.OutOfMemory: return outOfMemory;
                case ErrorCode.ApiUnavailable: return apiUnavailable;
                case ErrorCode.VersionUnavailable: return versionUnavailable;
                case ErrorCode.PlatformError: return platformError;
                case ErrorCode.FormatUnavailable: return formatUnavailable;
                case ErrorCode.NoWindowContext: return noWindowContext;
                default: return unknownError;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Exception" /> class.
        /// </summary>
        /// <param name="error">The error code to create a generic message from.</param>
        public GLFWException(ErrorCode error) : base(GetErrorMessage(error))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Exception" /> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GLFWException(string message) : base(message)
        {
        }

        #endregion
    }
}