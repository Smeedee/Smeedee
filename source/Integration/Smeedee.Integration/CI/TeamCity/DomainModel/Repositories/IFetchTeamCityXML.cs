namespace Smeedee.Integration.CI.TeamCity.DomainModel.Repositories
{
    public interface IFetchTeamCityXml
    {
        string GetTeamCityProjects(); // Domene: Server, TeamCity: Projects
        string GetTeamCityBuildConfigurations(string projectHref); // Domene: Projects, TeamCity: Configuration
        string GetTeamCityBuilds(string buildConfigHref); // Domene/TeamCity: Liste med builds (status, id)
        string GetBuildInfo(string buildId); // Info om build (dato)
        string GetChanges(string buildId); // Informasjon om changes for et build
        string GetChangesetInfo(string changesetHref);
    }
}