using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CourseProvider2.Functions;

public class GraphQL(ILogger<GraphQL> logger, IGraphQLRequestExecutor graphQLRequestExecutor)
{
    //En logginsats för att logga information, varningar och fel
    //En instans som exekverar GraphQL-förfrågningar
    private readonly ILogger<GraphQL> _logger = logger;
    private readonly IGraphQLRequestExecutor _graphQLRequestExecutor = graphQLRequestExecutor;


    //Metoden skapar en Azure function som hanterar POST-förfrågningar till rutten graphql och använder en 
    //IGraphQLRequestExecutor för att exekvera GraphQL-förfrågningarna. 
    //Loggning hanteras genom en ILogger<GraphQL> instans

    [Function("GraphQL")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "graphql")] HttpRequest req)
    {


        // Utför GraphQL-förfrågningen om _graphQLRequestExecutor är inte null
        return await _graphQLRequestExecutor.ExecuteAsync(req);

    }
}