
param location string
param environment string

@description('App Service Plan SKU')
@allowed([
  'F1'
  'B1'
  'B2'
  'S1'
  'S2'
  'P1v2'
  'P2v2'
  'P0v3'
])
param appServicePlanSku string = 'P0v3'

@description('The name of the App Service Plan')
param appServicePlanName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-apse-01'

@description('The name of the App Service')
param appServiceName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-apsp-01'

@description('The name of the Application Insights')
param appInsightsName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-appins-01'

@description('The name of the Log Analytics Workspace')
param logAnalyticsName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-logan-01'

@description('The name of the Storage Account for Blob Storage')
param storageAccountName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-stac-01'

@description('The name of the Blob Storage container')
param blobStorageName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-blbstrg-01'

@description('The name of the Automation Account')
param automationAccountName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-auac-01'

@description('The name of the Runbook START')
param runBookStartName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-rubo-start-01'

@description('The name of the Runbook STOP')
param runBookStopName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-rubo-stop-01'




// Log Analytics Workspace (required for Application Insights)
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Application Insights
// resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
//   name: appInsightsName
//   scope: resourceGroup.id
//   location: location
//   kind: 'web'
//   properties: {
//     Application_Type: 'web'
//     WorkspaceResourceId: logAnalytics.id
//   }
// }

// resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
//   name: storageAccountName
//   location: location
//   sku: {
//     name: storageAccountsku
//   }
//   kind: 'StorageV2'
//   tags: {
//     ObjectName: storageAccountName
//   }
//   properties: {
//     minimumTlsVersion: 'TLS1_2'
//     allowBlobPublicAccess: false
//     networkAcls: {
//       defaultAction: 'Deny'
//     }
//     supportsHttpsTrafficOnly: true
//   }
// }
