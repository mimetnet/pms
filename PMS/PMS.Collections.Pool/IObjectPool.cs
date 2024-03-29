﻿using System;

namespace PMS.Collections.Pool
{
    public interface IObjectPool<T> where T : class
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
