using Bitrate_Calculator.Data;
using Newtonsoft.Json;

string json = @"
            {
                ""Device"": ""Arista"",
                ""Model"": ""X-Video"",
                ""NIC"": [{
                    ""Description"": ""Linksys ABR"",
                    ""MAC"": ""14:91:82:3C:D6:7D"",
                    ""Timestamp"": ""2020-03-23T18:25:43.511Z"",
                    ""Rx"": ""3698574500"",
                    ""Tx"": ""122558800""
                }]
            }";

try
{
    // Deserialize JSON
    var deviceData = JsonConvert.DeserializeObject<DeviceData>(json);

    if (deviceData?.NIC == null || deviceData.NIC.Count == 0)
    {
        Console.WriteLine("No network interfaces found.");
        return;
    }

    // Dictionary to store previous readings for each NIC
    var previousReadings = new Dictionary<string, (long Rx, long Tx, DateTime Timestamp)>();

    // Initialize previous readings
    foreach (var nic in deviceData.NIC)
    {
        previousReadings[nic.MAC] = (nic.Rx, nic.Tx, nic.Timestamp);
    }

    Console.WriteLine($"Monitoring {deviceData.Device} {deviceData.Model}...\n");

    while (true)  // Simulating continuous polling
    {
        await Task.Delay(500); // Simulating 2Hz polling rate (async)

        // Simulate fetching new JSON data (in a real scenario, fetch updated JSON)
        var newData = SimulateNewData(previousReadings);

        foreach (var nic in newData.NIC)
        {
            if (previousReadings.TryGetValue(nic.MAC, out var previous))
            {
                double elapsedSeconds = (nic.Timestamp - previous.Timestamp).TotalSeconds;
                if (elapsedSeconds > 0)
                {
                    double rxBitrate = ((nic.Rx - previous.Rx) * 8 / elapsedSeconds) / 1000000;
                    double txBitrate = ((nic.Tx - previous.Tx) * 8 / elapsedSeconds) / 1000000;

                    Console.WriteLine($"[{nic.Timestamp:HH:mm:ss}] NIC {nic.Description} ({nic.MAC}):");
                    Console.WriteLine($"  Rx: {rxBitrate:F2} Mbps | Tx: {txBitrate:F2} Mbps");

                    // Update previous values
                    previousReadings[nic.MAC] = (nic.Rx, nic.Tx, nic.Timestamp);
                }
            }
        }
    }
} catch (JsonException ex){
    Console.WriteLine($"JSON parsing error: {ex.Message}");
} catch (Exception ex){
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
        

static DeviceData SimulateNewData(Dictionary<string, (long Rx, long Tx, DateTime Timestamp)> previousReadings)
{
    var random = new Random();
    var nics = new List<NetworkInterface>();

    foreach (var (mac, (previousRx, previousTx, previousTimestamp)) in previousReadings)
    {
        nics.Add(new NetworkInterface
        {
            Description = "Linksys ABR", // Simulated description
            MAC = mac,
            Timestamp = previousTimestamp.AddSeconds(0.5),
            Rx = previousRx + random.Next(1000000, 5000000), // Simulated Rx increase
            Tx = previousTx + random.Next(500000, 2000000)   // Simulated Tx increase
        });
    }

    return new DeviceData
    {
        Device = "Arista",
        Model = "X-Video",
        NIC = nics
    };
}