
namespace APD.Integration.CI.Hudson.DomainModel.Repositories
{
    public interface IRequestProxy
    {
        T Execute<T>(string endPoint);
    }
}
