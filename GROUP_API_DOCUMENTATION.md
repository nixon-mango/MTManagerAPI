# Group Management API

The MT5 Web API now includes comprehensive group management capabilities. You can create, read, and update trading groups through REST API endpoints.

## Available Endpoints

### 1. Create Group
**POST** `/api/groups`

Creates a new trading group with the specified configuration.

#### Request Body
```json
{
  "name": "real\\MyNewGroup",
  "description": "Custom trading group for VIP clients",
  "company": "My Trading Company",
  "currency": "USD",
  "leverage": 200,
  "depositMin": 1000,
  "depositMax": 5000000,
  "marginCall": 70,
  "marginStopOut": 40,
  "commission": 0.0,
  "commissType": 0,
  "rights": 67,
  "timeout": 60,
  "newsMode": 2,
  "reportsMode": 1,
  "emailFrom": "support@mycompany.com",
  "supportPage": "https://support.mycompany.com",
  "supportEmail": "support@mycompany.com",
  "defaultDeposit": 0,
  "defaultCredit": 0,
  "archivePeriod": 90,
  "archiveMaxRecords": 100000,
  "isDemo": false
}
```

#### Required Fields
- `name`: Group name (must include category like "real\\", "demo\\", etc.)

#### Optional Fields
All other fields are optional and will use intelligent defaults based on the group name and type.

#### Response
```json
{
  "success": true,
  "data": {
    "message": "Group created successfully",
    "group": {
      "name": "real\\MyNewGroup",
      "description": "Custom trading group for VIP clients",
      "company": "My Trading Company",
      "currency": "USD",
      "leverage": 200,
      "margin_call": 70,
      "margin_stop_out": 40,
      "commission": 0,
      "commission_type": 0,
      "rights": 67,
      "is_demo": false,
      "user_count": 0,
      "last_update": "2024-01-15T10:30:00Z"
    },
    "created_at": "2024-01-15T10:30:00Z",
    "group_type": "real"
  }
}
```

### 2. Get All Groups
**GET** `/api/groups`

Retrieves all available trading groups with summary statistics.

#### Response
```json
{
  "success": true,
  "data": {
    "groups": [
      {
        "name": "real\\Executive",
        "description": "VIP trading group: real\\Executive",
        "company": "MT5 Trading Company",
        "currency": "USD",
        "leverage": 200,
        "margin_call": 70,
        "margin_stop_out": 40,
        "commission": 0,
        "commission_type": 0,
        "rights": 67,
        "is_demo": false,
        "user_count": 15,
        "last_update": "2024-01-15T10:00:00Z"
      }
    ],
    "summary": {
      "total_groups": 25,
      "real_groups": 16,
      "demo_groups": 8,
      "manager_groups": 1,
      "total_users": 1250,
      "groups_by_type": [
        {"type": "real", "count": 16},
        {"type": "demo", "count": 8},
        {"type": "manager", "count": 1}
      ],
      "last_update": "2024-01-15T10:30:00Z"
    }
  }
}
```

### 3. Get Specific Group
**GET** `/api/groups/{groupName}`

Retrieves detailed information about a specific group.

#### Response
```json
{
  "success": true,
  "data": {
    "group": {
      "name": "real\\Executive",
      "description": "VIP trading group",
      "company": "MT5 Trading Company",
      "currency": "USD",
      "leverage": 200,
      "margin_call": 70,
      "margin_stop_out": 40,
      "commission": 0,
      "commission_type": 0,
      "rights": 67,
      "is_demo": false,
      "user_count": 15,
      "last_update": "2024-01-15T10:00:00Z"
    },
    "users": [
      {
        "login": 12345,
        "name": "John Doe",
        "email": "john@example.com",
        "country": "US",
        "registration": "2024-01-01T00:00:00Z"
      }
    ],
    "statistics": {
      "total_users": 15,
      "active_users": 12,
      "countries": ["US", "UK", "DE", "FR"],
      "registration_trend": [
        {"month": "2024-01", "count": 5},
        {"month": "2024-02", "count": 10}
      ]
    }
  }
}
```

### 4. Update Group
**PUT** `/api/groups/{groupName}`

Updates an existing group's configuration.

#### Request Body
```json
{
  "description": "Updated description",
  "leverage": 300,
  "marginCall": 75,
  "marginStopOut": 45,
  "commission": 5.0
}
```

#### Response
```json
{
  "success": true,
  "data": {
    "message": "Group updated successfully",
    "group_name": "real\\MyGroup",
    "updated_properties": {
      "description": {
        "from": "Old description",
        "to": "Updated description"
      },
      "leverage": {
        "from": 200,
        "to": 300
      },
      "margin_call": {
        "from": 70,
        "to": 75
      }
    },
    "timestamp": "2024-01-15T10:35:00Z"
  }
}
```

## Group Name Conventions

Group names should follow MT5 conventions with category prefixes:

- **Real Groups**: `real\\GroupName` (e.g., `real\\VIP`, `real\\Executive`)
- **Demo Groups**: `demo\\GroupName` (e.g., `demo\\Standard`, `demo\\Test`)
- **Manager Groups**: `managers\\GroupName` (e.g., `managers\\administrators`)

## Default Values

When creating groups, the API applies intelligent defaults based on the group name:

### Demo Groups (contains "demo")
- Leverage: 500
- Deposit Min: 0
- Default Deposit: 10000
- Commission: 0.0
- Rights: 71 (demo rights)

### VIP Groups (contains "vip" or "executive")
- Leverage: 200
- Margin Call: 70%
- Margin Stop Out: 40%
- Commission: 0.0

### Manager Groups (contains "manager")
- Rights: 127 (full rights)
- Timeout: 0 (no timeout)

### Standard Groups
- Leverage: 100
- Margin Call: 80%
- Margin Stop Out: 50%
- Commission: 7.0 (per lot)
- Rights: 67 (standard trading rights)

## Error Responses

### Invalid Group Name
```json
{
  "success": false,
  "error": "Invalid group name format. Use format like 'real\\GroupName' or 'demo\\GroupName'"
}
```

### Group Already Exists
```json
{
  "success": false,
  "error": "Group 'real\\MyGroup' already exists"
}
```

### Group Not Found
```json
{
  "success": false,
  "error": "Group 'real\\NonExistent' not found"
}
```

## cURL Examples

### Create a New VIP Group
```bash
curl -X POST http://localhost:8080/api/groups \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "name": "real\\VIP_Premium",
    "description": "Premium VIP trading group",
    "leverage": 300,
    "marginCall": 60,
    "marginStopOut": 30,
    "commission": 0.0,
    "depositMin": 10000
  }'
```

### Create a Demo Group
```bash
curl -X POST http://localhost:8080/api/groups \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "name": "demo\\TestGroup",
    "description": "Demo group for testing",
    "leverage": 500
  }'
```

### Get All Groups
```bash
curl -X GET http://localhost:8080/api/groups \
  -H "X-API-Key: your-api-key"
```

### Update Group Settings
```bash
curl -X PUT http://localhost:8080/api/groups/real\\MyGroup \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "leverage": 400,
    "marginCall": 65,
    "commission": 3.0
  }'
```

## Notes

1. **Authentication**: All endpoints require API key authentication if security is enabled.
2. **Group Creation**: The API creates logical group configurations. Actual MT5 server integration may require additional setup.
3. **Validation**: Group names are validated to ensure proper format and prevent duplicates.
4. **Defaults**: The API applies intelligent defaults based on group naming conventions.
5. **Flexibility**: All group properties can be customized during creation or updated later.

## Integration with MT5 Manager API

This API provides a RESTful interface to MT5 group management. The underlying implementation:

- Uses MT5 Manager API for server communication
- Provides intelligent defaults and validation
- Supports all major group configuration options
- Maintains compatibility with existing MT5 group structures