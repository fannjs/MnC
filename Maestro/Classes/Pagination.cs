using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.Classes
{
    public class Pagination<T>
    {
        public int Count;
        public T List;

        public Pagination(int Count, T List)
        {
            this.Count = Count;
            this.List = List;
        }
        public void setCount(int Count)
        {
            this.Count = Count;
        }
        public void setList(T List)
        {
            this.List = List;
        }
        public int GetCount()
        {
            return Count;
        }
        public T GetList()
        {
            return List;
        }

    }


}