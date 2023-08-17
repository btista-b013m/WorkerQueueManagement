
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerQueueManagement.Models;
using ServiceReference;
using Microsoft.AspNetCore.Hosting;
using WorkerQueueManagement;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkerQueueManagement.Utils;

public class Program

{
    private static IModel channel = null;

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo
        .File("./Logs/WorkerQueueManagement.out", Serilog.Events.LogEventLevel.Debug, "{Message:lj}{NewLine}", encoding: Encoding.UTF8)
        .CreateLogger();

        AppLog logApp = new AppLog();
        logApp.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff"));
        Utilerias.ImprimirLog(logApp, 0, "Iniciando servicio Worker", "Debug");


        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true);
        IConfiguration Configuration = builder.Build();
        Console.WriteLine(Configuration.GetValue<string>("Logging:LogLevel:Default"));
        Console.WriteLine(Configuration.GetValue<string>("RabbitConfiguration:UserName"));
        ConnectionFactory connectionFactory = new ConnectionFactory();
        connectionFactory.Port = (Configuration.GetValue<int>("RabbitConfiguration:Port")); ;
        connectionFactory.HostName = (Configuration.GetValue<string>("RabbitConfiguration:HostName")); ;
        connectionFactory.UserName = (Configuration.GetValue<string>("RabbitConfiguration:UserName")); ;
        connectionFactory.Password = (Configuration.GetValue<string>("RabbitConfiguration:Password")); ;
        IConnection conexion = connectionFactory.CreateConnection();
        channel = conexion.CreateModel();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += ConsumerMessageReceived;
        var consumerTag = channel.BasicConsume("kalum.queue.aspirante", true, consumer);
        Console.WriteLine("Presione una tecla para finalizar la lectura de los mensajes");
        Console.ReadKey();

    }

    // public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    // WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Configure();

    public static async void ConsumerMessageReceived(object? sender, BasicDeliverEventArgs e)
    {
        string message = Encoding.UTF8.GetString(e.Body.ToArray());
        //CandidateRequest request = JsonSerializer.Deserialize<CandidateRequest>(message);
        //await Task.Delay(100);
        //Console.WriteLine($"Recibiendo solicitud de {request.Apellidos}{request.Nombres}{request.Direccion}");
        AspiranteRequest request = JsonSerializer.Deserialize<AspiranteRequest>(message);
        AspiranteResponse response = await ClientWebServiceAspirante(request);
        Console.WriteLine($"Se registro la solicitud con el numero de expediente ${response.NoExpediente}");

        AppLog appLog = new AppLog();
        appLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff")); 
        Utilerias.ImprimirLog(appLog, 201, $"Se registro la solicitud con el numero de expediente {response.NoExpediente}", "Information");
    }

    public static async Task<AspiranteResponse> ClientWebServiceAspirante(AspiranteRequest request)
    {
        AspiranteResponse aspiranteResponse = null;
        var client = new EnrollmentServiceClient(EnrollmentServiceClient.EndpointConfiguration.BasicHttpBinding_IEnrollmentService_soap,
         "http://localhost:5034/EnrollmentService.asmx");

        var response = await client.CandidateRecordProcessAsync(request);
        aspiranteResponse = new AspiranteResponse()
        {
            StatusCode = response.Body.CandidateRecordProcessResult.StatusCode,
            Message = response.Body.CandidateRecordProcessResult.Message,
            NoExpediente = response.Body.CandidateRecordProcessResult.NoExpediente
        };
        return aspiranteResponse;
    }
}

