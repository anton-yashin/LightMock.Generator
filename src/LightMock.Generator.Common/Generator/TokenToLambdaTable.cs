/******************************************************************************
    MIT License

    Copyright (c) 2021 Anton Yashin

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*******************************************************************************
    https://github.com/anton-yashin/
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Linq;
using System.Reflection;

namespace LightMock.Generator
{
    sealed class TokenToLambdaTable
    {
        static TokenToLambdaTable? __instance;
        public static TokenToLambdaTable Instance => LazyInitializer.EnsureInitialized(ref __instance!);

        private readonly object tableLock = new();
        private IDictionary<string, LambdaExpression> table;
        private readonly Type LambdaTokenInterfaceType;

        public TokenToLambdaTable()
        {
            LambdaTokenInterfaceType = typeof(ILambdaToken);
            var table = new Dictionary<string, LambdaExpression>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                LoadAssemblyTo(table, assembly);
            this.table = table;
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
        }

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            lock (tableLock)
                LoadAssemblyTo(table, args.LoadedAssembly);
        }

        void LoadAssemblyTo(IDictionary<string, LambdaExpression> to, Assembly assembly)
        {
            foreach (var instance in from type in assembly.GetTypes()
                                     where LambdaTokenInterfaceType.IsAssignableFrom(type) && LambdaTokenInterfaceType != type
                                     select (ILambdaToken)Activator.CreateInstance(type))
            {
                if (to.ContainsKey(instance.Key))
                    throw new InvalidOperationException($"non unique token {instance.Key} was found");
                to.Add(instance.Key, instance.Value);
            }
        }

        public LambdaExpression Exchange(string token)
        {
            lock (tableLock)
            {
                if (table.TryGetValue(token, out LambdaExpression lambda) == false)
                    throw new ArgumentException($"expression for provided token [{token}] is not found");
                return lambda;
            }
        }
    }
}
