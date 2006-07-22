using System;

namespace PMS.Metadata
{
    public enum Auto
    {
        Create = 1,
        Retrieve = 1 << 1,
        Update = 1 << 2,
        Delete = 1 << 3
    }
}
