using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAssertNoOtherCalls_Throws
    {
        public abstract string GetOnly { get; }
        public abstract string SetOnly { set; }
        public abstract string GetAndSet { get; set; }
        public abstract string Function(string a);
        public abstract void Method(string a);

        protected abstract string ProtectedGetOnly { get; }
        protected abstract string ProtectedSetOnly { set; }
        protected abstract string ProtectedGetAndSet { get; set; }
        protected abstract string this[string index] { get; set; }
        protected abstract string ProtectedFunction(string a);
        protected abstract void ProtectedMethod(string a);

        public string InvokeProtectedGetOnly => ProtectedGetOnly;
        public string InvokeProtectedSetOnly { set { ProtectedSetOnly = value; } }
        public string InvokeProtectedGetAndSet { get => ProtectedGetAndSet; set { ProtectedGetAndSet = value; } }
        public string InvokeIndexerGet(string index) => this[index];
        public string InvokeIndexerSet(string index, string value) => this[index] = value;
        public string InvokeProtectedFunction(string a) => ProtectedFunction(a);
        public void InvokeProtectedMethod(string a) => ProtectedMethod(a);
    }

}
