# Test debug endpoint
try {
    Write-Host "Testing debug endpoint..."
    $response = Invoke-RestMethod -Uri 'http://localhost:5275/api/machines/debug/10.224.142.245' -Method Get
    Write-Host "Response received:"
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Error: $_"
}