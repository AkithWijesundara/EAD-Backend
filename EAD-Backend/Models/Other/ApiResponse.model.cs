using System.Text.Json.Serialization;

namespace EAD_Backend.OtherModels
{
    public class ApiResponse<T>
    {
        private object data;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Success {get; set;}
        public string Message { get; set; }
        public T Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; set; } = new();
       

        //Default constructor
        public ApiResponse() {}

        public ApiResponse(bool success, string message, T data, List<String> errors=null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        // Constructor with desired parameter order
        public ApiResponse(string message, T data = default )
        {
            Message = message;
            Data = data;
           
        }


    }
}
