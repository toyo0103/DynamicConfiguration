using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamicConfigLab.DynamicConfiguration.PullingMode.Interfaces;
using DynamicConfigLab.DynamicConfiguration.PullingMode.Models;

namespace DynamicConfigLab.DynamicConfiguration.PullingMode;

public class ConfigurationRepository(IAmazonDynamoDB dynamoDb) : IConfigurationRepository
{
     private const string TableName = "service_configuration";

     public async Task CreateOrUpdateConfigAsync(CreateOrUpdateConfigRequest request)
    {
        var pk = $"service#{request.ServiceName}";
        var sk = $"config#{request.Scope}#{request.ConfigName}";

        var item = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new() { S = pk },
            ["sk"] = new() { S = sk },
            ["configValue"] = new() { S = request.ConfigValue },
            ["updatedAt"] = new() { N = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
        };

        var putItemRequest = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        await dynamoDb.PutItemAsync(putItemRequest);
    }

    public async Task<bool> DeleteConfigAsync(string serviceName, string scope, string configName)
    {
        var pk = $"service#{serviceName}";
        var sk = $"config#{scope}#{configName}";

        // soft delete
        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new() { S = pk } },
                { "sk", new() { S = sk } }
            },
            
            UpdateExpression = "SET #updatedAt = :updatedAt, #deletedAt = :deletedAt",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#updatedAt", "updatedAt" },
                { "#deletedAt", "deletedAt" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":updatedAt", new() { N = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() } },
                { ":deletedAt", new() { N = DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds().ToString() } }
            },
            ConditionExpression = "attribute_exists(pk)",
            ReturnValues = "UPDATED_NEW"
        };
        
        try
        {
            await dynamoDb.UpdateItemAsync(request);
            return true;
        }
        catch (ConditionalCheckFailedException)
        {
            return false;
        }
    }
    
    public async Task<Dictionary<string, EffectiveConfigValue>> GetEffectiveConfigurationAsync(string serviceName, string scope)
    {
        var globalConfigs = await QueryScopeAsync(serviceName, "global");
        var scopeConfigs = new List<ServiceConfiguration>();
        if (!string.Equals(scope, "global", StringComparison.OrdinalIgnoreCase))
        {
            scopeConfigs = await QueryScopeAsync(serviceName, scope);
        }

        var effectiveConfig = new Dictionary<string, EffectiveConfigValue>();
        foreach (var config in globalConfigs)
        {
            var configName = config.sk.Split('#').Last();
            effectiveConfig[configName] = new EffectiveConfigValue
            {
                Value = config.configValue,
                SourceScope = "global",
                DeletedAt = config.deletedAt
            };
        }
        foreach (var config in scopeConfigs)
        {
            var configName = config.sk.Split('#').Last();
            effectiveConfig[configName] = new EffectiveConfigValue
            {
                Value = config.configValue,
                SourceScope = scope,
                DeletedAt = config.deletedAt
            };
        }

        return effectiveConfig;
    }

    private async Task<List<ServiceConfiguration>> QueryScopeAsync(string serviceName, string scope)
    {
        var pk = $"service#{serviceName}";
        var skPrefix = $"config#{scope}#";

        var request = new QueryRequest
        {
            TableName = TableName,
            KeyConditionExpression = "pk = :pk and begins_with(sk, :sk_prefix)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":pk", new AttributeValue { S = pk }},
                {":sk_prefix", new AttributeValue { S = skPrefix }}
            }
        };
        
        var response = await dynamoDb.QueryAsync(request);
        var results = response.Items
            .Select(item =>
            {
                // Always include TTL/deletedAt metadata
                long? deletedAt = null;
                if (item.TryGetValue("deletedAt", out var delAttr) && long.TryParse(delAttr.N, out var dt))
                {
                    deletedAt = dt;
                }
                return new ServiceConfiguration
                {
                    pk = item["pk"].S,
                    sk = item["sk"].S,
                    configValue = item["configValue"].S,
                    updatedAt = long.Parse(item["updatedAt"].N),
                    deletedAt = deletedAt
                };
            })
            .ToList();
        return results;
    }
}