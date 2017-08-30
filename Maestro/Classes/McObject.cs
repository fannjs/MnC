using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.Classes
{
    public class McObject<T>
    {
        public bool Status;
        public string Message;
        public T Object;

        public McObject(bool Status, string Message, T Object)
        {
            this.Status = Status;
            this.Message = Message;
            this.Object = Object;
        }
        public McObject(bool Status, string Message)
        {
            this.Status = Status;
            this.Message = Message;
        }

        public Boolean isSuccessful()
        {
            return Status;
        }

        public void setStatus(bool Status)
        {
            this.Status = Status;
        }

        public String getMessage()
        {
            return Message;
        }

        public void setMessage(string Message)
        {
            this.Message = Message;
        }

        public T getObject()
        {
            return Object;
        }

        public void setObject(T Object)
        {
            this.Object = Object;
        }
    }
}