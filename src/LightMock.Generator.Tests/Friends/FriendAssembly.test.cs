using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Friends
{
    public class FriendAssembly : ITestScript<IFoo>
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
