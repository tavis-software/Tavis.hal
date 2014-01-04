namespace Tavis
{
    public interface IHalProperty
    {
        string Name { get; }
        object GetContent();
        object GetValue(string path = null);
    }
}