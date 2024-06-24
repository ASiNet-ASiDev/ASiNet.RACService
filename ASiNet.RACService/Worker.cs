using System.Net.Sockets;
using ASiNet.RACService.Network;
using ASiNet.RACService.Primitives;

namespace ASiNet.RACService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    private Config _config = null!; 
    private TcpListener _listener = null!;

    private List<RAClient> _clients = [];

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _config = Config.Read();

        _listener = new(System.Net.IPAddress.Any, _config.Port);
        _listener.Start();
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(stoppingToken);
                _clients.Add(new(client, _config));

                _clients.RemoveAll(client => { if(!client.IsConnected) { client.Dispose(); return true; } else return false; });
            }
            catch
            {

            }
        }
    }
}
