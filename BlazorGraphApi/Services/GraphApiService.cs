using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using Graph = Microsoft.Graph;

namespace BlazorGraphApi.Services
{
    public class GraphApiService
    {
        readonly ITokenAcquisition tokenAcquisition;
        readonly WebOptions webOptions;

        public GraphApiService(ITokenAcquisition tokenAcquisition,
                                  IOptions<WebOptions> webOptionValue)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.webOptions = webOptionValue.Value;
        }

        public async Task<Graph.User> GetProfileAsync()
        {
            // Initialize the GraphServiceClient. 
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserRead });
            var me = await graphClient.Me.Request().GetAsync();
            return me;
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, webOptions.GraphApiUrl);
        }
    }
}
