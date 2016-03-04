using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Build.Client;

namespace bdversion
{
    class Program
    {
        public static IList<KeyValuePair<string, Uri>> GetBuildDefinitionListFromProject(TfsConfigurationServer configurationServer, Guid collectionId, string projectName)
        {
            List<IBuildDefinition> buildDefinitionList = null;
            List<KeyValuePair<string, Uri>> buildDefinitionInfoList = null;

            try
            {
                buildDefinitionInfoList = new List<KeyValuePair<string, Uri>>();
                TfsTeamProjectCollection tfsProjectCollection =
                configurationServer.GetTeamProjectCollection(collectionId);
                tfsProjectCollection.Authenticate();
                var buildServer = (IBuildServer)tfsProjectCollection.GetService(typeof(IBuildServer));
                buildDefinitionList = new List<IBuildDefinition>(buildServer.QueryBuildDefinitions(projectName));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (buildDefinitionList != null && buildDefinitionList.Count > 0)
            {
                foreach (IBuildDefinition builddef in buildDefinitionList)
                {
                    buildDefinitionInfoList.Add(new KeyValuePair<string, Uri>(builddef.Name, builddef.Uri));
                }
            }
            return buildDefinitionInfoList;
        }

        static void Main(string[] args)
        {
            Uri tfsUri = new Uri("http://tfstta.int.thomson.com:8080/tfs");

            TfsConfigurationServer server = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);

            // Get the catalog of team project collections
            ReadOnlyCollection <CatalogNode> collectionNodes = server.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

            // List the team project collections
            foreach (CatalogNode collectionNode in collectionNodes)
            {
                // Use the InstanceId property to get the team project collection
                Guid collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection teamProjectCollection = server.GetTeamProjectCollection(collectionId);

                // Print the name of the team project collection
                Console.WriteLine("Collection: " + teamProjectCollection.Name);

                // Get a catalog of team projects for the collection
                ReadOnlyCollection<CatalogNode> projectNodes = collectionNode.QueryChildren(
                    new[] { CatalogResourceTypes.TeamProject },
                    false, CatalogQueryOptions.None);

                // List the team projects in the collection
                foreach (CatalogNode projectNode in projectNodes)
                {
                    Console.WriteLine(" Team Project: " + projectNode.Resource.DisplayName);

                    var list = GetBuildDefinitionListFromProject(server,
                        new Guid(collectionNode.Resource.Properties["instanceId"]),
                        projectNode.Resource.DisplayName);


                    foreach( var pair in list) 
                    {
                        Console.WriteLine("\t" + pair.Key + ":" + pair.Value);
                    }



                }
            }
        }
    }
}
