// Deploy at subscription scope to create resource group
targetScope  = 'subscription'

// Parameters
@description('The environment (dev or prod)')
param environment string = 'dev'

@description('The location for the resource group')
param location string = 'westeurope'

@description('The name of the resource group')
param resourceGroupName string = 'sktc-prtx-pvcld-pvrpcldapi15-tst-rsgrp-01'

// Resource Group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

// Deploy resources into the resource group using a module
module resources 'test_pvrpcloud15_resources_iac.bicep' = {
  name: 'resourcesDeployment'
  scope: resourceGroup
  params: {
    location: location
    environment: environment
  }
}

// Outputs
output resourceGroupName string = resourceGroup.name
output resourceGroupId string = resourceGroup.id
output location string = resourceGroup.location
output environment string = environment
