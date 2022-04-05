using LightMock.Generator.Tests.TestAbstractions;
using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Friends.FriendAssembly.test.cs")]

namespace LightMock.Generator.Tests.Friends
{
    abstract class Bar
    {
        protected internal abstract void Foo();
    }

    public class SameAssembly : ITestScript<IFoo>
    {
        public IMock<IFoo> Context => throw new NotImplementedException();

        public IFoo MockObject => throw new NotImplementedException();

        public int DoRun()
        {
            var mock = new Mock<Bar>();
            return 42;
        }
    }

}
