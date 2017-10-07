namespace ModApi
{
    public interface IBuilder<out T> where T : IModObject
    {
        T BuildAndRegister();
    }
}