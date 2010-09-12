namespace Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories
{
    public interface IFetchHudsonXml
    {
        string GetProjects();
        string GetBuilds(string projectName);
        string GetBuild(string projectName, string buildId);
        string GetUrl();
    }
}
