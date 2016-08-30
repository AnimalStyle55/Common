using System;

namespace Common.ReusableGenericCRUD
{
    public interface IKeyed
    {
        Guid Id { get; set; }
    }
}