namespace Org.Gravinium.Jilwer.Runtime.Core.Collections
{
    public interface ICollection
    {
        int Length();
        void Add(object item);
        object Get(int index);
        void Remove(int index);
    }
}