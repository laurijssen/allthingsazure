using System;
using System.Net;

using Microsoft.Azure.Cosmos;

using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Authentication;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;
using System.Text.Json;

class Program
{
    private const string EndpointUri = "https://coresqltest.documents.azure.com:443/";
    private const string Key = "";

    private CosmosClient client = null!;
    private Database database = null!;
    private Container container = null!;

    static void Main(string[] args)
    {
        try
        {
            Program demo = new Program();

            demo.StartDemo();
        }
        catch (CosmosException ce)
        {
            Exception baseException = ce.GetBaseException();
            Console.WriteLine($"{ce.StatusCode} error occurred");
        }
    }

    class PreOrder
    {
        public string OrderNumber { get; set; } = null!;
        public string DealerNumber { get; set; } = null!;
    }

    class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return name.ToLower();
        }
    }


    private void StartDemo()
    {
        MongoClient _client;
        IMongoDatabase  _database;
        IMongoCollection<BsonDocument> _collection;

        var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

        InitClassmap<PreOrder>();

        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(""));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

        _client = new MongoClient(settings);

        Console.WriteLine(_client.Settings.Server.Host);

        _database = _client.GetDatabase("rops");
        _collection = _database.GetCollection<BsonDocument>("preorders");
        
        var filter = BsonDocument.Parse("{}");

        var result = _collection.Find(filter, null);
        var l = new List<PreOrder>();
        foreach (var r in result.ToEnumerable())
        {
            l.Add(BsonSerializer.Deserialize<PreOrder>(r));
        }

        PreOrder p = new() {
            OrderNumber = "123445",
            DealerNumber = "54321"
        };

        try
        {
            var options = new JsonSerializerOptions { IncludeFields = true };
            options.PropertyNamingPolicy = new LowerCaseNamingPolicy();

            var js = JsonSerializer.Serialize(p, options);

            _collection.InsertOne(BsonDocument.Parse(js));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        /*
        string databaseName = "DemoDB";

        SendMessageToConsoleAndWait($"Creating database {databaseName}...");

        client = new CosmosClient(EndpointUri, Key);
        database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

        string containerName = "persons";

        SendMessageToConsoleAndWait($"Creating collection demo{containerName}...");

        container = await this.database.CreateContainerIfNotExistsAsync(containerName, "/LastName");

        Person person = new Person
        {
            Id = "Person.1",
            FirstName = "Santiago",
            LastName = "Fernandez",
            Devices = new Device[]
            {
                new Device { OperatingSystem = "iOS", CameraMegaPixels = 7, Ram = 16, Usage = "Personal"},
                new Device { OperatingSystem = "Android", CameraMegaPixels = 12, Ram = 64, Usage = "Work"}
            },
            Gender = "Male",
            Address = new Address
            {
                City = "Seville",
                Country = "Spain",
                PostalCode = "28973",
                Street = "Diagonal",
                State = "Andalucia"
            },
            IsRegistered = true
        };         

        try
        {
            var resp = await container.ReadItemAsync<Person>(person.Id, new PartitionKey(person.LastName));

            var p = resp.Resource;

            Console.WriteLine($"found {p.FirstName}");
        }
        catch (CosmosException dce)
        {
            if (dce.StatusCode == HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync<Person>(person, new PartitionKey(person.LastName));

                SendMessageToConsoleAndWait($"inserted person");
            }
        }
        */
    }

void InitClassmap<T>()
{
    if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
    {
        var map = BsonClassMap.RegisterClassMap<T>(cm =>
        {
            cm.AutoMap();
        });

        foreach (var member in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            string name = member.Name;

            map.GetMemberMap(name).SetElementName(name.ToLower());
        }

        foreach (var member in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            string name = member.Name;

            var memberMap = map.GetMemberMap(name);

            if (memberMap != null)
                memberMap.SetElementName(name.ToLower());
        }
    }
}    

    private void SendMessageToConsoleAndWait(string message)
    {
        Console.WriteLine(message);
        //Console.WriteLine("Press any key to continue...");
        //Console.ReadKey();
    }    
}

public class Person
{
    //[JsonProperty(PropertyName="id")]
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public Device[] Devices { get; set; } = null!;

    public Address Address { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public bool IsRegistered { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class Device
{
    public int Ram { get; set; }
    public string OperatingSystem { get; set; } = null!;
    public int CameraMegaPixels  { get; set; }
    public string Usage { get; set; } = null!;
}

public class Address
{
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string Street { get; set; } = null!;
}
