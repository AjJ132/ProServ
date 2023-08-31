using System;
namespace ProServ.Shared.Models.Util
{
    public class ResponseWrapper<T>
    {
        public string Id { get; set; }
        public List<T> Values { get; set; }
    }

}

