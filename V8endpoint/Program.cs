using Npgsql;
using NpgsqlTypes;
using TheEndpoint;

var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSaga");
endpointConfiguration.EnableInstallers();
var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
var dialect = persistence.SqlDialect<SqlDialect.PostgreSql>();
dialect.JsonBParameterModifier(
    modifier: parameter =>
    {
        var npgsqlParameter = (NpgsqlParameter)parameter;
        npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
    });
persistence.ConnectionBuilder(() => new NpgsqlConnection("Server=localhost;Port=5432;Database=db_user;User Id=db_user;Password=your_password;"));
endpointConfiguration.UseTransport<LearningTransport>();
endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

endpointConfiguration.Pipeline.Register(new FixTimeoutMessageVersionBehavior(), "Fix TimeoutMessage versions behavior");

var endpointInstance = await Endpoint.Start(endpointConfiguration);

Console.WriteLine($"Press any key to stop the endpoint.");
Console.ReadLine();

await endpointInstance.Stop();