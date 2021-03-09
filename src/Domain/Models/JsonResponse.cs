using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class JsonResponse<T>
    {
        public JsonResponse(bool success = true)
        {
            Success = success;
        }

        public JsonResponse(T data)
        {
            Data = data;
        }

        public JsonResponse(string message, bool success = false)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
