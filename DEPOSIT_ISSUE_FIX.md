# 🔧 Deposit Issue Fix - User 814752

## ❌ **Current Issue**
Deposit operation failing for user 814752 with error: "Balance operation failed"

## 🔍 **Debugging Steps**

### **Step 1: Build with Enhanced Debugging**
```cmd
build.bat
# Should now succeed with enhanced error logging
```

### **Step 2: Test User Validation**
```bash
# Test if user 814752 exists and has proper rights
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Test", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance/test

# This will show:
# - If user exists
# - User's rights and group
# - Account accessibility
# - Specific recommendations
```

### **Step 3: Check User Information**
```bash
# Get user details
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/user/814752

# Look for:
# - "rights" field (should be > 0)
# - "group" field (should be active group)
```

### **Step 4: Check Account Information**
```bash
# Get account details
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/account/814752

# Verify account exists and current balance
```

## 🎯 **Most Likely Causes**

### **1. User Rights Issue (Most Common)**
```json
// If you see this in user info:
{
  "rights": 0  // ← Problem: No trading rights
}
```
**Solution:** Contact your broker to enable trading rights for user 814752.

### **2. Group Restrictions**
```json
// If user is in restricted group:
{
  "group": "real\\Archive"  // ← Problem: Archived/disabled group
}
```
**Solution:** Move user to active trading group or check group permissions.

### **3. Manager Account Permissions**
Your manager account might lack dealer permissions.
**Solution:** Ensure manager account has "Dealer" rights in MT5 Admin.

## 🔧 **Enhanced Error Handling**

I've added comprehensive error handling that will now provide specific error messages instead of generic "Balance operation failed":

### **Before (Generic Error)**
```json
{
  "success": false,
  "error": "Balance operation failed"
}
```

### **After (Specific Errors)**
```json
{
  "success": false,
  "error": "User with login 814752 not found"
}
```

**Or:**
```json
{
  "success": false,
  "error": "User 814752 has no trading rights (rights = 0)"
}
```

## 🧪 **Test Different Scenarios**

### **Test 1: Different Operation Types**
```bash
# Try type 1 (Credit)
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Credit test", "type": 1}' \
     http://YOUR_STATIC_IP:8080/api/balance

# Try type 0 (Balance adjustment)
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Balance test", "type": 0}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

### **Test 2: Different User**
```bash
# Try with a different user (if you know one that works)
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": DIFFERENT_LOGIN, "amount": 10, "comment": "Test", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

### **Test 3: Smaller Amount**
```bash
# Try with smaller amount
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 1, "comment": "Small test", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance
```

## 📊 **Console Logging**

When you run the balance operation, you'll now see detailed logs in the Web API console:
```
[2025-09-19 06:00:54] Balance operation attempt:
  Login: 814752
  Amount: 10
  Type: 2
  Comment: Deposit via API
  User found: John Doe (real\Executive)
  Result: FAILED
```

## 🎯 **Quick Diagnosis**

Run this command to get immediate diagnosis:
```bash
curl -X POST -H "X-API-Key: YOUR_KEY" \
     -H "Content-Type: application/json" \
     -d '{"login": 814752, "amount": 10, "comment": "Test", "type": 2}' \
     http://YOUR_STATIC_IP:8080/api/balance/test
```

The response will tell you exactly what's wrong and provide specific recommendations to fix it.

## 🚀 **Expected Resolution**

After running the test, you'll get one of these scenarios:

1. **User doesn't exist** → Check login ID
2. **User has no rights** → Contact broker to enable trading rights
3. **Group restrictions** → Move to active group or check permissions
4. **Manager permissions** → Ensure manager has dealer rights
5. **API connection** → Verify MT5 server connection

The enhanced debugging will pinpoint the exact issue! 🔍🚀