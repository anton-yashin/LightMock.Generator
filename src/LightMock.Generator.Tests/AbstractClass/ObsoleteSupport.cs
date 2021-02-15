using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AObsoleteSupport
    {
        [Obsolete]
        public virtual void Foo() { }
        [Obsolete]
        public virtual int Bar { get; set; }
        [Obsolete]
        protected virtual void Baz() { }
        [Obsolete]
        protected virtual int Quux { get; set; }
    }
}
