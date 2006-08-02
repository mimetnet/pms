using System;

namespace PMS.Collections.Pool
{
    interface IObjectPool
    {
        bool Add(object obj);
        object Borrow();
        bool Remove(object obj);
        bool Return(object obj);

        void CleanObject(ref object obj);
        void Close();

        int Available { get; }
        int Count { get; }
        int Max { get; set; }
    }
}
