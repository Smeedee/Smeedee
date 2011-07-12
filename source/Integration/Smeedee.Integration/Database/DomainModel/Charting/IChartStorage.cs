using Smeedee.DomainModel.Charting;

namespace Smeedee.Integration.Database.DomainModel.Charting
{
    interface IChartStorage
    {
        void Save(Chart chart);
    }
}
