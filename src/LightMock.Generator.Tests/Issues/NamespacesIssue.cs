using System;

namespace Playground
{
    public interface IFoo
    {
        void Bar();
    }

    public interface IFoo<T>
    {
        void Bar();
    }

    public abstract class AFoo
    {
        public abstract void Bar();
    }

    public abstract class AFoo<T>
    {
        public abstract void Bar();
    }


}
