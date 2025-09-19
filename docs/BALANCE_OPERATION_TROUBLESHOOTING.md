# 🔧 Balance Operation Troubleshooting Guide

## ❌ **Issue: Deposit Not Working**

You're getting this error when trying to deposit:
```json
{
    "success": false,
    "error": "Balance operation failed",
    "timestamp": "2025-09-19T06:00:54.3298986Z"
}
```

## 🔍 **Possible Causes & Solutions**

### **1. User Rights/Permissions Issue**
The most common cause is insufficient user rights for balance operations.

**Check user rights:**
```bash
# First, get user information to check rights
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/user/814752

# Look for "rights" field in response
```

### **2. Manager Account Permissions**
Your manager account might not have permission to perform balance operations.

**Check manager permissions:**
- Ensure your manager account has "Dealer" rights
- Verify the manager can perform financial operations
- Check if there are group-specific restrictions

### **3. Group-Specific Restrictions**
Some groups may have restrictions on balance operations.

**Check group settings:**
- Verify the user's group allows balance modifications
- Check if there are minimum/maximum deposit limits
- Ensure the group is not read-only or archived

## 🧪 **Debugging Steps**

### **Step 1: Test User Validation**
```bash
# Use the new test endpoint to validate everything
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Test", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance/test

# This will show:
# - If the user exists
# - User's group and rights
# - Account information
# - Connection status
```

### **Step 2: Check User Details**
```bash
# Get detailed user information
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/user/814752

# Look for:
# - "rights" field (should be > 0 for trading rights)
# - "group" field (check if group allows operations)
```

### **Step 3: Check Account Status**
```bash
# Get account information
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/account/814752

# Verify:
# - Account exists and is accessible
# - Current balance
# - Currency information
```

### **Step 4: Try Different Parameters**

#### **Try Different Operation Type**
```bash
# Try type 1 instead of type 2
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Deposit test", "type": 1}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

#### **Try Smaller Amount**
```bash
# Try smaller amount
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 1, "comment": "Small deposit test", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

#### **Try Different Comment**
```bash
# Try without special characters
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Deposit", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

## 🔧 **Enhanced Error Logging**

I've added detailed logging to help debug the issue. When you run the balance operation, check the console output of your MT5WebAPI server for detailed logs:

```
[2025-09-19 06:00:54] Balance operation attempt:
  Login: 814752
  Amount: 10
  Type: 2
  Comment: Deposit via API
  User found: John Doe (real\Executive)
  Result: FAILED
```

## 🎯 **Common Solutions**

### **Solution 1: Check User Rights**
```csharp
// User rights bitmask values:
// 1 = Client can trade
// 2 = Client can change password  
// 4 = Client can use OTP
// 8 = Client can use API
// 16 = Client can use reports
// etc.

// User needs trading rights (typically rights >= 1)
```

### **Solution 2: Verify Manager Permissions**
```
Your manager account needs:
- "Dealer" permissions in MT5 Admin
- "Balance operations" enabled
- Access to the user's group
```

### **Solution 3: Check Group Configuration**
```
In MT5 Admin, verify:
- Group allows balance operations
- No minimum deposit restrictions
- Group is not archived or disabled
```

### **Solution 4: Alternative Balance Method**
If the standard method doesn't work, try using a different approach:

```bash
# Try with explicit deposit flag
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Manual deposit", "type": 0}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

## 📊 **Expected Working Response**

When the deposit works correctly, you should see:
```json
{
    "success": true,
    "data": {
        "message": "Balance operation successful",
        "login": 814752,
        "amount": 10,
        "comment": "Deposit via API",
        "type": 2,
        "timestamp": "2025-09-19T06:00:54Z"
    },
    "timestamp": "2025-09-19T06:00:54Z"
}
```

## 🚀 **Next Steps**

1. **Build the solution** (with enhanced logging)
2. **Start the Web API server** 
3. **Test user validation:**
   ```bash
   curl -X POST -H "X-API-Key: YOUR_KEY" \
        -H "Content-Type: application/json" \
        -d '{"login": 814752, "amount": 10, "comment": "Test", "type": 2}' \
        http://YOUR_STATIC_IP:8080/api/balance/test
   ```
4. **Check the detailed response** to identify the specific issue
5. **Try the suggested solutions** based on the test results

The enhanced logging and test endpoint will help identify exactly why the balance operation is failing! 🔍🚀