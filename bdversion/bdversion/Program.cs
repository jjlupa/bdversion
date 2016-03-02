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
        static void Main(string[] args)
        {
            Uri tfsUri = new Uri("http://tfstta.int.thomson.com:8080/tfs");

            TfsConfigurationServer server = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);

            TeamFoundationServer test = new TeamFoundationServer(tfsUri);
            IBuildServer ibs = (IBuildServer)test.GetService(typeof(IBuildServer));
            IBuildDefinition ibd = ibs.GetBuildDefinition("CSTax", "USTaxSampleServices");

            TfsTeamProjectCollection tfstpc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://tfstta.int.thomson.com:8080/tfs/DefaultCollection" ) );

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
                }
            }
        }
    }
}
