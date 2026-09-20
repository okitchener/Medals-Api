@description('The Azure region for the resources')
param location string

@description('Resource tags')
param tags object

@description('Unique token based on subscription and environment name')
param resourceToken string

var abbrs = loadJsonContent('./abbreviations.json')
var appServicePlanName = '${abbrs.appServicePlan}${resourceToken}'
var appServiceName = '${abbrs.appService}${resourceToken}'

// App Service Plan (Linux, Free F1 tier - avoids quota requirements)
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// App Service Web App (Linux .NET 8)
resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  tags: union(tags, { 'azd-service-name': 'web' })
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: false
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'ConnectionStrings__DefaultSQLiteConnection'
          value: 'Data Source=/home/data/appdb.sqlite'
        }
      ]
    }
    httpsOnly: true
  }
}

output SERVICE_WEB_NAME string = appService.name
output SERVICE_WEB_URI string = 'https://${appService.properties.defaultHostName}'
