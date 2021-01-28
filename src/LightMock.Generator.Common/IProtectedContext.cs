using System.ComponentModel;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IProtectedContext<T> where T : class
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        object ProtectedContext { get; }
    }


}
