namespace Gravinium.Jilwer.Core
{
    /// <summary>
    /// The Error enum is the standard error value passed from methods in Jilwer.
    /// The value can be checked before attempting to use a returned value or before
    /// preceding after a method call.
    /// </summary>
    public enum Error
    {
        /// <summary>
        /// Represents no error was returned.
        /// </summary>
        None,
        /// <summary>
        /// The index used on an array was not in bounds (Smaller than 0 or larger than the length).
        /// </summary>
        IndexOutOfBounds,
        /// <summary>
        /// The type used is not valid as input (e.g. int != string).
        /// </summary>
        InvalidType,
    }
}