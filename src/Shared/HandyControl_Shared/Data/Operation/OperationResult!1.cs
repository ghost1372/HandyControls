namespace HandyControl.Data
{
    /// <summary>
    ///     Represents the return result information of an operation
    /// </summary>
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        ///     Operation result
        /// </summary>
        public ResultType ResultType { get; set; }

        /// <summary>
        ///     Return data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///     Operation message (including data such as the cause of the error)
        /// </summary>
        public string Message { get; set; }
    }
}
