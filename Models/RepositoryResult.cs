namespace Onyx.Models
{
    /// <summary>
    /// A generic object to move the result from repository to the controller.
    /// Data object should be filled with a correspondint data only.
    /// All string representations, errors and messages should go to Message prop.
    /// HttpCode can be used in StatusCode(code, ...) object in controller to return
    /// appropriate response from API.
    /// </summary>
    /// <typeparam name="T">Type of data value.</typeparam>
    public class RepositoryResult<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int HttpCode { get; set; }
    }
}
