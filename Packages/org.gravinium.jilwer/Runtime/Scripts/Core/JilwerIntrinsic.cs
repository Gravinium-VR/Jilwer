using System;

namespace Gravinium.Jilwer.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JilwerIntrinsic : Attribute
    {
        public IntrinsicType RequiredContext { get; }

        public JilwerIntrinsic(IntrinsicType requiredContext)
        {
            RequiredContext = requiredContext;
        }
    }

    public enum IntrinsicType
    {
        Runtime,
    }
}