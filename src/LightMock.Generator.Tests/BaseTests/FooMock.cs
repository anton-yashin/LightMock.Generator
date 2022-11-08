using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.BaseTests
{
    public class FooMock : IFoo
    {
        private readonly IInvocationContext<IFoo> context;

        public FooMock(IInvocationContext<IFoo> context)
        {
            this.context = context;
        }

        public string? Execute(string value)
        {
            return context.Invoke(f => f.Execute(value));
        }

        public void Execute(int first)
        {
            context.Invoke(f => f.Execute(first));
        }

        public void Execute(int first, int second)
        {
            context.Invoke(f => f.Execute(first, second));
        }

        public void Execute(int first, int second, int third)
        {
            context.Invoke(f => f.Execute(first, second, third));
        }

        public void Execute(int first, int second, int third, int fourth)
        {
            context.Invoke(f => f.Execute(first, second, third, fourth));
        }

        public void Execute(int first, int second, int third, int fourth, int fifth)
        {
            context.Invoke(f => f.Execute(first, second, third, fourth, fifth));
        }
        public void Execute(int first, int second, int third, int fourth, int fifth, int sixth)
        {
            context.Invoke(f => f.Execute(first, second, third, fourth, fifth, sixth));
        }

        public string? Execute()
        {
            return context.Invoke(f => f.Execute());
        }

        public Task<string> ExecuteAsync(string value)
        {
            return context.Invoke(f => f.ExecuteAsync(value))!;
        }

        public string? Execute(string first, string second)
        {
            return context.Invoke(f => f.Execute(first, second));
        }

        public string? Execute(string first, string second, string third)
        {
            return context.Invoke(f => f.Execute(first, second, third));
        }

        public string? Execute(string first, string second, string third, string fourth)
        {
            return context.Invoke(f => f.Execute(first, second, third, fourth));
        }

        public Task<string> ExecuteAsync()
        {
            return context.Invoke(f => f.ExecuteAsync())!;
        }

        public Task<string> ExecuteAsync(string first, string second)
        {
            return context.Invoke(f => f.ExecuteAsync(first, second))!;
        }

        public Task<string> ExecuteAsync(string first, string second, string third)
        {
            return context.Invoke(f => f.ExecuteAsync(first, second, third))!;
        }

        public Task<string> ExecuteAsync(string first, string second, string third, string fourth)
        {
            return context.Invoke(f => f.ExecuteAsync(first, second, third, fourth))!;
        }

        public byte[]? Execute(byte[] array)
        {
            return context.Invoke((f => f.Execute(array)));
        }

        public string OutMethod(out string @string, out int @int)
        {
            string __ref_string = default!;
            int __ref_int = default!;
            var refValues = new Dictionary<string, object>();
            var result = context.Invoke(f => f.OutMethod(out __ref_string, out __ref_int), refValues);
            @string = ArgumentHelper.Unpack<string>(refValues, nameof(@string));
            @int = ArgumentHelper.Unpack<int>(refValues, nameof(@int));
            return result!;
        }

        public int RefMethod(ref string @string, ref int @int)
        {
            string __ref_string = @string;
            int __ref_int = @int;
            var refValues = new Dictionary<string, object>();
            var result = context.Invoke(f => f.RefMethod(ref __ref_string, ref __ref_int), refValues);
            @string = ArgumentHelper.Unpack(refValues, nameof(@string), @string);
            @int = ArgumentHelper.Unpack(refValues, nameof(@int), @int);
            return result;
        }
    }
}