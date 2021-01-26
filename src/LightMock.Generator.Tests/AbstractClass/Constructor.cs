using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AConstructor
    {
        public int? P1 { get; }
        public long? P2 { get; }
        public object? P3 { get; }

        public IEnumerable<object>? Objects { get; }

        public AConstructor(int p1, long p2, object p3) 
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public AConstructor() { }

        public AConstructor(IEnumerable<object> objects) 
        {
            Objects = objects;
        }

        public abstract void DoSomething();
        protected abstract void ProtectedDoSomething();

        public void InvokeProtectedDoSomething() => ProtectedDoSomething();
    }
}
