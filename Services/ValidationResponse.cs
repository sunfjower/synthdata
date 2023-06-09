namespace MultipleDataGenerator.Services
{
    public class ValidationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public ValidationResponse(string message, bool success) 
        {
            Success = success;
            Message = message;
        }
    }
}
