import http.client
import json

# Test registration endpoint
conn = http.client.HTTPConnection("localhost:5275")

data = {
    "macAddress": "74:56:3C:2E:79:E4",
    "ip": "10.224.142.245",
    "machineName": "TestMachine",
    "appVersion": "1.0.0"
}

try:
    headers = {'Content-Type': 'application/json'}
    conn.request("POST", "/api/machines/register", json.dumps(data), headers)
    response = conn.getresponse()
    response_data = response.read().decode()
    print(f"Status: {response.status}")
    print(f"Response: {response_data}")
except Exception as e:
    print(f"Error: {e}")
finally:
    conn.close()