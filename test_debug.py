import http.client
import json

# Test debug endpoint
conn = http.client.HTTPConnection("localhost:5275")

try:
    conn.request("GET", "/api/machines/debug/10.224.142.245")
    response = conn.getresponse()
    data = response.read().decode()
    print(f"Status: {response.status}")
    print(f"Response: {data}")
except Exception as e:
    print(f"Error: {e}")
finally:
    conn.close()