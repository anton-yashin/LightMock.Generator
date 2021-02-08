using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAbstractClassWithConstructors
    {
        public AAbstractClassWithConstructors(int param1, IEnumerable<object> param2)
        { }

        protected AAbstractClassWithConstructors(Task param1)
        { }


        public abstract void DoSomething();
    }
}
