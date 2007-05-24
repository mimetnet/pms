using System;

namespace PMS.Collections.Pool
{
    interface IObjectPool
    {
        object Borrow();

        bool Return(object obj);

        bool Remove(object obj);

        void CleanObject(ref object obj);

        bool Open();
        void Close();

        int Count { get; }
        int Max { get; }
        int Min { get; }
    }
}
