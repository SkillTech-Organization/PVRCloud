# PVRCloud

## Project board
https://github.com/orgs/SkillTech-Organization/projects/3/views/1

## Repositories
### PVRCloud WebAPI: https://github.com/SkillTech-Organization/PVRCloud.git

## Branching strategy
- branch for development: *develop*
- branch for customer test: *stage*
- branch for live environment: *main*

## Environments and access
- development: *https://prvcloudwebapitest.azurewebsites.net/*
- customer test:
- live:

--- 

# From the Knowledge base

## Definitions Per Environment

### Prod - main branch (?) - *CURRENT STATE*

| Item              | Description                                             |
| ----------------- | ------------------------------------------------------- |
| Account           | ???                                                     |
| Management Group  | 8875ae16-2b24-4357-95a4-62e9df84fe06                    |
| Subscription      | 702fab27-7b08-4bcd-a29e-4c15e902dca2                    |
| Resource Group    | PRVPCloudResourceGroup                                  |
| Tenant ID         | 8875ae16-2b24-4357-95a4-62e9df84fe06                    |

### Resource Items

| Resource Type     | Resource Name               | Description                    | Tags                     |
| ----------------- | --------------------------- | ------------------------------ | ------------------------ |
| Managed Identity  | PRVPCloudWebAPIT-id-ae66    | RBAC role to deployment        | -                        |
| App Service       | prvpcloudwebapitest         | Application service            | -                        |
| App Service Plan  | PVRPCloudPlan               | Application service plan       | -                        |
| Storage Account   | pvrpcloudstoragetest        | Storage account                | -                        |
| Storage Container | $logs                       | App Insight logs               | -                        |
| Storage Container | azure-webjobs-dashboard     | Webjob dashboard data          | -                        |
| Storage Container | calculations                | result data                    | -                        |
| Storage Container | map                         | map data                       | -                        |
| Storage Container | parameters                  | parameter set of the app       | -                        |
| Storage Queue     | pmapcalcinputmsgs           | request, input data            | -                        |
| Storage Queue     | pmapcalcinputmsgs-poison    | request, input data - DLQ      | -                        |
| Storage Queue     | pmapcalcinputmsgsdev        | request, input data TEST       | -                        |
| Storage Queue     | pmapcalcinputmsgsdev-poison | request, input data TEST - DLQ | -                        |
| Storage Queue     | pmapcalcoutputmsgs          | response, output data          | -                        |
| Storage Queue     | pmapcalcoutputmsgsdev       | response, output data TEST     | -                        |

---

## planned resource and components
### for test
sktc-prtx-pvcld-prvpcldapi15-tst-rsgrp-01
sktc-prtx-pvcld-prvpcldapi15-tst-apsp-01
sktc-prtx-pvcld-prvpcldapi15-tst-apse-01
sktc-prtx-pvcld-prvpcldapi15-tst-apis-01
sktc-prtx-pvcld-prvpcldapi15-tst-mgid-01
sktc-prtx-pvcld-prvpcldapi15-tst-auac-01
sktc-prtx-pvcld-prvpcldapi15start-tst-rubo-01
sktc-prtx-pvcld-prvpcldapi15stop-tst-rubo-01

### for prod
sktc-prtx-pvcld-prvpcldapi15-prd-rsgrp-01
sktc-prtx-pvcld-prvpcldapi15-prd-apsp-01
sktc-prtx-pvcld-prvpcldapi15-prd-apse-01
sktc-prtx-pvcld-prvpcldapi15-prd-apis-01
sktc-prtx-pvcld-prvpcldapi15-prd-mgid-01
sktc-prtx-pvcld-prvpcldapi15-prd-auac-01
sktc-prtx-pvcld-prvpcldapi15start-prd-rubo-01
sktc-prtx-pvcld-prvpcldapi15stop-prd-rubo-01