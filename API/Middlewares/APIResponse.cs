namespace API.Middlewares
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
