using System;

namespace SmartTutor.External.Promise
{
    public interface IResolvable<T>
    {
        void Resolve(T value);
    }
}
