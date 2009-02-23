using System;

namespace PMS.Collections.Pool
{
    interface IObjectPool<T>
    {
        T Borrow();
        bool Return(T obj);
        bool Remove(T obj);

        bool Open();
        void Close();

        int Count { get; }
        int Maximum { get; }
        int Minimum { get; }
    }
}