using Smeedee.DomainModel.Charting;

namespace Smeedee.Integration.Database.DomainModel.Charting
{
    public interface IChartStorage
    {
        void Save(Chart chart);
    }
}
