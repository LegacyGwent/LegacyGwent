namespace Cynthia.Card
{
    public interface IRepository
    {
        string Name { get; }
        IDatabase Database { get; }
        IRepository<TModel> As<TModel>() where TModel : IModel;
    }
}