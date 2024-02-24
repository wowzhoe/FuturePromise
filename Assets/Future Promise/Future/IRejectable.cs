using System;

namespace SmartTutor.External.Promise
{
    public interface IRejectable
    {
        void Reject(Exception ex);
    }
}
