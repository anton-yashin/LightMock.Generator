namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ABasicMethod
    {
        protected abstract void DoSomething(int p);
        public abstract int GetSomething();
    }

    //public interface IP2P_ABasicMethod
    //{
    //    void DoSomething(int p);
    //}

    //public partial class BasicMethod : ABasicMethod { }

    //partial class BasicMethod : IP2P_ABasicMethod
    //{
    //    private readonly IInvocationContext<LightMock.Generator.Tests.AbstractClass.ABasicMethod> context;
    //    private readonly IInvocationContext<IP2P_ABasicMethod> protectedContext;

    //    public BasicMethod(IInvocationContext<LightMock.Generator.Tests.AbstractClass.ABasicMethod> context, IInvocationContext<IP2P_ABasicMethod> protectedContext)
    //    {
    //        this.context = context;
    //        this.protectedContext = protectedContext;
    //    }

    //    void LightMock.Generator.Tests.AbstractClass.IP2P_ABasicMethod.DoSomething(int p) { protectedContext.Invoke(f => f.DoSomething(p)); }
    //    override protected void DoSomething(int p) { protectedContext.Invoke(f => f.DoSomething(p)); }
    //    override public int GetSomething() { return context.Invoke(f => f.GetSomething()); }

    //}
}
