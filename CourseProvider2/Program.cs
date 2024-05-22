using CourseProvider.Infrastructure.Data.Contexts;
using CourseProvider.Infrastructure.GraphQL.Mutations;
using CourseProvider.Infrastructure.GraphQL.ObjectTypes;
using CourseProvider.Infrastructure.GraphQL.Queries;
using CourseProvider.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    { //REGISTRERING AV COSMOS DB
      //AddPooledDbContextFactory<DataContext>: Detta sätter upp en pool av DataContext-instanser, där DataContext är min anpassade DbContext-klass
      //x => x.UseCosmos(...): Detta konfigurerar varje DataContext-instans att använda Cosmos DB-leverantören med den angivna URI:n och databasnamnet, vilka hämtas från miljövariabler.
      // Konfigurera Application Insights


        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddPooledDbContextFactory<DataContext>(x =>
        {
            x.UseCosmos(
               Environment.GetEnvironmentVariable("COSMOS_URI")!, Environment.GetEnvironmentVariable("COSMOS_DBNAME")!)
               .UseLazyLoadingProxies(); //hämtar in alla undertabeller automatiskt
        });

        //Reg service
        services.AddScoped<ICourseService, CourseService>();

        //**GRAPGQL**
        //registrering av GraphQL-funktioner
        services.AddGraphQLFunction()
            .AddQueryType<CourseQuery>()
            .AddMutationType<CourseMutation>()
            .AddType<CourseType>();

        //**DATABAS**
        //populera db och skapa den om den inte finns, och skapa containern
        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
        using var context = dbContextFactory.CreateDbContext();
        context.Database.EnsureCreated(); //ser till att skapa db om den inte redan finns

    })
    .Build();

host.Run();
