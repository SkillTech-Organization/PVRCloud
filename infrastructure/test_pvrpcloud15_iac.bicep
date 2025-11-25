// Parameters
@description('The name of the resource group')
param resourceGroupName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-rsgrp-01'

@description('The location for the resource group')
param location string = 'westeurope'

// Note: Resource groups are created at subscription scope, not within a Bicep template
// This is a placeholder file. Resource group creation is handled in the GitHub Actions workflow

// If you need to deploy resources within the resource group, add them here
// For example:
// resource example 'Microsoft.Resources/deployments@2021-04-01' = {
//   name: 'exampleDeployment'
//   properties: {
//     mode: 'Incremental'
//   }
// }

// Outputs
output resourceGroupName string = resourceGroupName
output location string = location