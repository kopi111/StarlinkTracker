# FortiAuthenticator Setup Guide
## 15-Minute Daily Quota for FortiWiFi-40F

---

## Step 1: Download FortiAuthenticator (FREE)

### Option A: Virtual Machine (Recommended)
1. Go to: https://support.fortinet.com
2. Login with your Fortinet account (create free account if needed)
3. Navigate to: **Support → Firmware Download**
4. Search for: **FortiAuthenticator**
5. Download: **FortiAuthenticator VM** (Latest version - 6.5.x or 6.6.x)
6. Choose your hypervisor:
   - **VMware** (ESXi, Workstation, Fusion)
   - **Hyper-V** (Windows Server)
   - **KVM** (Linux)

### Option B: Physical Appliance
- If you have budget, buy FortiAuthenticator hardware
- Models: FAC-100F, FAC-200F, FAC-400E, etc.

### Free License
- FortiAuthenticator is **FREE** for basic use
- Supports up to 100 users without license
- Perfect for your use case

---

## Step 2: Install FortiAuthenticator VM

### For VMware:
1. Extract the downloaded OVF file
2. Open VMware (Workstation/ESXi)
3. Click: **File → Open** (or Import)
4. Select the FortiAuthenticator OVF file
5. Configure VM settings:
   - **RAM**: 2GB minimum (4GB recommended)
   - **CPU**: 2 cores minimum
   - **Network**: Bridged or same network as FortiWiFi-40F
6. Power on the VM

### For Hyper-V:
1. Extract the VHD file
2. Create new VM in Hyper-V Manager
3. Attach the FortiAuthenticator VHD
4. Configure network adapter
5. Start the VM

---

## Step 3: Initial FortiAuthenticator Configuration

### Access Console:
1. Open VM console
2. Login with default credentials:
   - **Username**: `admin`
   - **Password**: (blank - just press Enter)

3. You'll be prompted to change password:
   ```
   New Password: [create strong password]
   Confirm Password: [repeat password]
   ```

### Configure Network:
```bash
# View current network config
get system interface

# Configure IP address (adjust to your network)
config system interface
edit port1
set mode static
set ip 192.168.1.50/24
set allowaccess ping https ssh
next
end

# Set default gateway
config system dns
set primary 8.8.8.8
set secondary 8.8.4.4
end

config router static
edit 1
set device port1
set gateway 192.168.1.1
next
end
```

4. Test connectivity:
```bash
execute ping 8.8.8.8
```

---

## Step 4: Access FortiAuthenticator Web GUI

1. Open browser on your computer
2. Go to: `https://192.168.1.50` (use the IP you configured)
3. Accept security warning (self-signed certificate)
4. Login:
   - **Username**: `admin`
   - **Password**: [password you set]

---

## Step 5: Configure Time-Based Quotas

### 5.1: Create User Group with 15-Minute Quota

1. Go to: **Authentication → User Groups**
2. Click: **Create New**
3. Configure:
   - **Name**: `15min-Daily-Users`
   - **Type**: `Firewall`

4. Go to: **Quota** tab
5. Enable quota:
   - **Enable Quota**: ✓ Yes
   - **Quota Type**: `Time`
   - **Quota Value**: `900` (seconds = 15 minutes)
   - **Quota Period**: `Daily`
   - **Reset Time**: `00:00` (midnight)

6. Click: **OK**

### 5.2: Create Guest User Template (Auto-Registration)

1. Go to: **Authentication → Guest Management**
2. Click: **User Groups** tab
3. Click: **Create New**
4. Configure:
   - **Name**: `Guest-Template`
   - **Member of groups**: Select `15min-Daily-Users`
   - **Auto-create users**: ✓ Yes
   - **Based on**: `MAC Address`

5. Click: **OK**

---

## Step 6: Configure RADIUS Server

### 6.1: Enable RADIUS Service

1. Go to: **System → Network → Interfaces**
2. Select: `port1`
3. Enable: **RADIUS** service
4. Click: **OK**

### 6.2: Create RADIUS Client (Your FortiWiFi-40F)

1. Go to: **Authentication → RADIUS Service → Clients**
2. Click: **Create New**
3. Configure:
   - **Name**: `FortiWiFi-40F`
   - **Client Address/Netmask**: `192.168.1.1/32` (your FortiWiFi IP)
   - **Secret**: Create strong secret (e.g., `Fortinet123!@#`)
   - **Vendor**: `Fortinet`

4. Click: **OK**

### 6.3: Configure RADIUS Accounting
1. Go to: **Authentication → RADIUS Service → Settings**
2. Enable:
   - **Accounting**: ✓ Yes
   - **Track Session Time**: ✓ Yes
   - **Enforce Quotas**: ✓ Yes

3. Click: **Apply**

---

## Step 7: Configure FortiWiFi-40F

### 7.1: Add FortiAuthenticator as RADIUS Server

SSH to your FortiWiFi-40F and run:

```bash
config user radius
edit "FortiAuthenticator"
set server "192.168.1.50"
set secret "Fortinet123!@#"
set radius-port 1812
set auth-type auto
set acct-interim-interval 60
set acct-all-servers enable
next
end
```

### 7.2: Create User Group

```bash
config user group
edit "Starlink-Quota-Users"
set member "FortiAuthenticator"
next
end
```

### 7.3: Create Firewall Address

```bash
config firewall address
edit "Starlink-Network"
set subnet 192.168.1.0 255.255.255.0
next
end
```

### 7.4: Create Firewall Policy with Authentication

```bash
config firewall policy
edit 100
set name "Starlink-15min-Quota"
set srcintf "internal"
set dstintf "wan1"
set srcaddr "Starlink-Network"
set dstaddr "all"
set action accept
set schedule "always"
set service "ALL"
set nat enable
set groups "Starlink-Quota-Users"
set auth-path enable
set logtraffic all
set comments "15 minute daily quota per device"
next
end
```

### 7.5: Enable Captive Portal (Optional)

```bash
config system settings
set auth-https-port 1003
set auth-http-port 1000
end

config authentication rule
edit 1
set name "Starlink-Auth"
set status enable
set srcintf "internal"
set srcaddr "Starlink-Network"
set ip-based enable
set active-auth-method web-form
next
end
```

---

## Step 8: Configure Web Portal (Captive Portal)

### On FortiWiFi-40F:

```bash
config system replacemsg http "auth-success"
set msg-type html
set buffer "
<html>
<head><title>Access Granted - JCF Network</title></head>
<body style='font-family: Arial; text-align: center; padding: 50px;'>
<h1 style='color: #28a745;'>✓ Access Granted</h1>
<p>You have <strong>15 minutes</strong> of internet access today.</p>
<p>Your session will end automatically after 15 minutes or at midnight.</p>
<hr>
<p style='color: #666; font-size: 12px;'>Jamaica Constabulary Force Network</p>
</body>
</html>
"
next
end

config system replacemsg http "auth-reject"
set msg-type html
set buffer "
<html>
<head><title>Quota Exceeded - JCF Network</title></head>
<body style='font-family: Arial; text-align: center; padding: 50px;'>
<h1 style='color: #dc3545;'>⏱ Daily Quota Exceeded</h1>
<p>You have used your 15-minute daily internet allowance.</p>
<p>Access will be restored tomorrow at midnight (00:00).</p>
<hr>
<p style='color: #666; font-size: 12px;'>Jamaica Constabulary Force Network</p>
</body>
</html>
"
next
end
```

---

## Step 9: Testing

### 9.1: Connect a Test Device
1. Connect a device to your Starlink network
2. Open web browser
3. Try to access any website
4. You should see captive portal (if enabled)
5. Click "Accept" or "Continue"

### 9.2: Monitor on FortiAuthenticator
1. Go to: **Monitor → Authentication**
2. Go to: **Monitor → Active Users**
3. You should see the device listed with remaining time

### 9.3: Monitor on FortiWiFi-40F
```bash
# View authenticated users
diagnose firewall auth list

# View active sessions
diagnose sys session list

# Check RADIUS authentication
diagnose debug application radiusd -1
diagnose debug enable
```

---

## Step 10: Verification

### Check User Quota:
1. On FortiAuthenticator GUI:
   - Go to: **Monitor → Active Users**
   - See remaining time for each user

2. On FortiWiFi-40F CLI:
```bash
diagnose firewall auth list
```

### View Logs:
1. FortiAuthenticator: **Log & Report → Authentication**
2. FortiWiFi-40F: **Log & Report → Forward Traffic**

---

## Troubleshooting

### Issue 1: Device Not Redirected to Portal
**Solution:**
```bash
# On FortiWiFi-40F, ensure authentication is enabled
show authentication rule

# Check if auth-path is enabled in policy
show firewall policy 100
```

### Issue 2: RADIUS Authentication Fails
**Solution:**
1. Verify FortiAuthenticator IP is reachable:
   ```bash
   execute ping 192.168.1.50
   ```

2. Check RADIUS secret matches:
   ```bash
   # On FortiWiFi-40F
   show user radius FortiAuthenticator
   ```

3. Check FortiAuthenticator logs:
   - **Log & Report → RADIUS → Authentication**

### Issue 3: Quota Not Enforcing
**Solution:**
1. On FortiAuthenticator:
   - **Authentication → RADIUS Service → Settings**
   - Ensure "Enforce Quotas" is enabled

2. Check user is in quota group:
   - **Monitor → Active Users**
   - Verify user shows quota information

### Issue 4: Time Not Counting Down
**Solution:**
1. Ensure accounting is working:
   ```bash
   # On FortiWiFi-40F
   show user radius FortiAuthenticator
   ```
   - Verify `acct-all-servers` is enabled

2. Check interim updates:
   - Set to 60 seconds for real-time tracking

---

## Monitoring & Management

### View All Users and Quotas:
**FortiAuthenticator GUI:**
- **Monitor → Active Users**

### View Quota Usage Per User:
**FortiAuthenticator GUI:**
- **Log & Report → Authentication → Quota**

### Reset User Quota Manually (Emergency):
**FortiAuthenticator CLI:**
```bash
# Reset specific user
execute user-quota clear <username>

# Reset all users
execute user-quota clear-all
```

### View RADIUS Logs:
**FortiAuthenticator GUI:**
- **Log & Report → RADIUS**

---

## Maintenance

### Daily Tasks:
- Check **Monitor → Active Users** for active sessions
- Review **Log & Report** for quota violations

### Weekly Tasks:
- Review quota usage trends
- Check for MAC address spoofing attempts

### Monthly Tasks:
- Backup FortiAuthenticator configuration
- Update FortiAuthenticator firmware if available

---

## Backup Configuration

### Backup FortiAuthenticator:
1. Go to: **System → Maintenance → Backup**
2. Click: **Backup**
3. Save the file securely

### Backup FortiWiFi-40F:
```bash
execute backup config ftp <filename> <ftp-server> <username> <password>
```

Or via GUI: **System → Configuration → Backup**

---

## Network Diagram

```
[Internet]
    |
    ↓
[FortiWiFi-40F] ← RADIUS Auth → [FortiAuthenticator VM]
192.168.1.1                      192.168.1.50
    |
    ↓
[Starlink Network]
192.168.1.0/24
    |
    ↓
[Devices] ← 15min quota per device
```

---

## Summary

✅ **What You Get:**
- Automatic 15-minute quota per device
- Quota tracked by MAC address
- Daily automatic reset at midnight
- Web portal for user notification
- Real-time monitoring
- Detailed usage logs

✅ **Key Features:**
- No manual IP configuration needed
- Works with any device
- Scales to 100+ devices (free tier)
- Professional enterprise solution
- Centralized management

---

## Cost
- **FortiAuthenticator VM**: FREE (up to 100 users)
- **No additional license needed**
- **Total Cost**: $0

---

## Support
- FortiAuthenticator Documentation: https://docs.fortinet.com/product/fortiauthenticator
- FortiGate Documentation: https://docs.fortinet.com/product/fortigate
- Fortinet Community Forum: https://community.fortinet.com

---

## Quick Reference Commands

### FortiWiFi-40F Commands:
```bash
# View authenticated users
diagnose firewall auth list

# Clear authentication for specific IP
diagnose firewall auth clear <ip-address>

# Test RADIUS connectivity
execute radius-test <user> <password> FortiAuthenticator
```

### FortiAuthenticator Commands:
```bash
# View active sessions
diagnose debug radius show session

# Clear user quota
execute user-quota clear <mac-address>

# View quota statistics
diagnose debug application radiusd
```

---

**Your 15-minute quota system is now complete!** 🎉
