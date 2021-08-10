using System.Collections.Generic;

namespace WebApplication3.Helper
{
    public class ReturnData<T>
    {
        public int Code;
        public string Message;
        public List<T> Data;

        public ReturnData(int  code,string message,List<T> data)
        {
            Message = message;
            Code = code;
            Data = data;

        }

       

    }
}