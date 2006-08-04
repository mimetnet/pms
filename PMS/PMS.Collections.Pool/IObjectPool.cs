﻿using System;

namespace PMS.Collections.Pool
{
    interface IObjectPool
    {
        object Borrow();
        bool Remove(object obj);
        bool Return(object obj);

        void CleanObject(ref object obj);
        bool Open();
        void Close();

        int Available { get; }
        int Count { get; }
        int Max { get; set; }
    }
}